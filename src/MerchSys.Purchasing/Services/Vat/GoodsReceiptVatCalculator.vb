Imports MerchSys.Purchasing.Dtos

Namespace Services.Vat

    Public Class GoodsReceiptVatCalculator

        Public Function Calculate(dto As ReceiveGoodsDto) As GoodsReceiptVatBreakdown
            Dim totalAmount As Decimal = 0

            For Each line In dto.Lines
                totalAmount += (line.UnitCost * line.QuantityReceived)
            Next

            ' Option-2 simplification: assume all lines are vatable at 12%
            Dim vatableInput As Decimal = Math.Round(totalAmount / 1.12D, 2)
            Dim inputVat As Decimal = totalAmount - vatableInput

            Dim breakdown As New GoodsReceiptVatBreakdown With {
                .VatableInputs = vatableInput,
                .VatExemptInputs = 0,
                .ZeroRatedInputs = 0,
                .InputVat = inputVat,
                .VendorInvoiceTotal = totalAmount
            }

            Return breakdown
        End Function

    End Class

End Namespace
