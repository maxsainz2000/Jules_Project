Imports MerchSys.Purchasing.Dtos

Namespace Services.Vat

    Public Class GoodsReceiptVatCalculator

        Public Function Calculate(dto As ReceiveGoodsDto) As GoodsReceiptVatBreakdown
            Dim totalAmount As Decimal = 0
            Dim vatableInput As Decimal = 0
            Dim vatExemptInput As Decimal = 0
            Dim zeroRatedInput As Decimal = 0
            Dim inputVat As Decimal = 0

            For Each line In dto.Lines
                Dim lineTotal = (line.UnitCost * line.QuantityReceived)
                totalAmount += lineTotal

                Select Case line.VatClassification
                    Case SharedKernel.Enums.VatTreatment.Vatable
                        vatableInput += line.VatableSales
                        inputVat += line.VatAmount
                    Case SharedKernel.Enums.VatTreatment.Exempt
                        vatExemptInput += line.VatableSales
                    Case SharedKernel.Enums.VatTreatment.ZeroRated
                        zeroRatedInput += line.VatableSales
                End Select
            Next

            Dim breakdown As New GoodsReceiptVatBreakdown With {
                .VatableInputs = vatableInput,
                .VatExemptInputs = vatExemptInput,
                .ZeroRatedInputs = zeroRatedInput,
                .InputVat = inputVat,
                .VendorInvoiceTotal = totalAmount
            }

            Return breakdown
        End Function

    End Class

End Namespace
