Imports System.Linq
Imports MerchSys.POS.Entities

Namespace Services.ReceiptFormatting
    Public Class BirCompliantReceiptBodyComposer
        Implements IReceiptBodyComposer

        Public Function Compose(receipt As OfficialReceipt, transaction As SalesTransaction, vatConfig As VatConfiguration) As ReceiptBody Implements IReceiptBodyComposer.Compose
            Dim body As New ReceiptBody()

            Dim businessName = If(String.IsNullOrWhiteSpace(vatConfig?.BusinessName), "VISTA POS MERCHANDISING", vatConfig.BusinessName)
            Dim tin = If(String.IsNullOrWhiteSpace(vatConfig?.Tin), "123-456-789-000", vatConfig.Tin)

            ' Header
            body.Header.Add(CenterText(businessName))
            body.Header.Add(CenterText("VAT REG TIN: " & tin))
            If Not String.IsNullOrWhiteSpace(vatConfig?.Address) Then
                body.Header.Add(CenterText(vatConfig.Address))
            End If
            body.Header.Add(StrDup(40, "-"c))

            ' Receipt Info
            body.ReceiptInfo.Add(PadRightLeft("OR #:", receipt.ReceiptNumber, 40))
            body.ReceiptInfo.Add(PadRightLeft("DATE:", transaction.TransactionDate.ToString("yyyy-MM-dd HH:mm"), 40))
            body.ReceiptInfo.Add(StrDup(40, "-"c))

            ' Items
            If transaction.Lines IsNot Nothing Then
                For Each line In transaction.Lines
                    body.Items.Add(PadRightLeft(line.ProductName, "", 40))
                    Dim qtyPrice = $"{line.Quantity} x {line.UnitPrice:N2}"
                    body.Items.Add(PadRightLeft("  " & qtyPrice, line.LineTotal.ToString("N2"), 40))
                Next
            End If
            body.Items.Add(StrDup(40, "-"c))

            ' Totals
            Dim vatAmount = If(transaction.Lines IsNot Nothing AndAlso transaction.Lines.Any(), transaction.Lines.Sum(Function(l) l.OutputVat), transaction.TotalAmount - transaction.VatableSales)

            body.Totals.Add(PadRightLeft("TOTAL:", transaction.TotalAmount.ToString("N2"), 40))
            body.Totals.Add(PadRightLeft("VATABLE SALES:", transaction.VatableSales.ToString("N2"), 40))
            body.Totals.Add(PadRightLeft("VAT AMOUNT:", vatAmount.ToString("N2"), 40))
            body.Totals.Add(PadRightLeft("VAT EXEMPT:", transaction.VatExemptSales.ToString("N2"), 40))
            body.Totals.Add(PadRightLeft("ZERO RATED:", transaction.ZeroRatedSales.ToString("N2"), 40))
            body.Totals.Add(StrDup(40, "="c))

            ' Footer
            If vatConfig IsNot Nothing AndAlso Not vatConfig.IsVatRegistered Then
                body.Footer.Add(CenterText("THIS DOCUMENT IS NOT VALID FOR"))
                body.Footer.Add(CenterText("CLAIM OF INPUT TAX"))
            End If
            body.Footer.Add(CenterText("THANK YOU FOR SHOPPING!"))

            Return body
        End Function

        Private Function CenterText(text As String) As String
            If String.IsNullOrEmpty(text) Then Return ""
            If text.Length >= 40 Then Return text.Substring(0, 40)
            Dim padLeft = (40 + text.Length) \ 2
            Return text.PadLeft(padLeft).PadRight(40)
        End Function

        Private Function PadRightLeft(leftText As String, rightText As String, totalWidth As Integer) As String
            If String.IsNullOrEmpty(leftText) Then leftText = ""
            If String.IsNullOrEmpty(rightText) Then rightText = ""
            If leftText.Length + rightText.Length >= totalWidth Then
                ' Truncate left text if too long
                Dim availableSpace = totalWidth - rightText.Length - 1
                If availableSpace > 0 Then
                    Return leftText.Substring(0, availableSpace) & " " & rightText
                Else
                    Return (leftText & rightText).Substring(0, totalWidth)
                End If
            End If
            Dim spaces = totalWidth - leftText.Length - rightText.Length
            Return leftText & StrDup(spaces, " "c) & rightText
        End Function
    End Class
End Namespace
