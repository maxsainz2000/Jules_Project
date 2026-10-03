Imports System.Text
Imports System.Text.Json
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities

Namespace Services

    Public Class ReceiptService
        Implements IReceiptService

        Private ReadOnly _dbContext As POSDbContext

        Public Sub New(dbContext As POSDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function GenerateReceiptAsync(transactionId As Integer) As Task(Of OfficialReceipt) Implements IReceiptService.GenerateReceiptAsync
            Dim transaction = Await _dbContext.SalesTransactions _
                .Include(Function(t) t.Lines) _
                .Include(Function(t) t.Receipt) _
                .FirstOrDefaultAsync(Function(t) t.Id = transactionId)

            If transaction Is Nothing Then
                Throw New ArgumentException($"Transaction with ID {transactionId} not found.")
            End If

            If transaction.Receipt IsNot Nothing Then
                Throw New InvalidOperationException("Transaction already has a receipt.")
            End If

            Dim currentYear = DateTime.UtcNow.Year
            Dim yearPrefix = $"OR-{currentYear}-"

            ' Fetch latest receipt number for the current year
            Dim maxReceiptString = Await _dbContext.OfficialReceipts _
                .Where(Function(r) r.ReceiptNumber.StartsWith(yearPrefix)) _
                .Select(Function(r) r.ReceiptNumber) _
                .MaxAsync()

            Dim nextNumber = 1
            If maxReceiptString IsNot Nothing Then
                Dim maxNum = Integer.Parse(maxReceiptString.Substring(yearPrefix.Length))
                nextNumber = maxNum + 1
            End If

            Dim receiptNumber = $"{yearPrefix}{nextNumber.ToString("D4")}"

            Dim receipt = New OfficialReceipt With {
                .ReceiptNumber = receiptNumber,
                .SerializedLineItems = JsonSerializer.Serialize(transaction.Lines.Select(Function(l) New With { l.ProductName, l.UnitPrice, l.Quantity, l.LineTotal }))
            }

            transaction.Receipt = receipt
            _dbContext.OfficialReceipts.Add(receipt)
            Await _dbContext.SaveChangesAsync()

            Return receipt
        End Function

        Public Async Function GetReceiptAsync(receiptId As Integer) As Task(Of OfficialReceipt) Implements IReceiptService.GetReceiptAsync
            Return Await _dbContext.OfficialReceipts.FirstOrDefaultAsync(Function(r) r.Id = receiptId)
        End Function

        Public Async Function GetReceiptByTransactionAsync(transactionId As Integer) As Task(Of OfficialReceipt) Implements IReceiptService.GetReceiptByTransactionAsync
            Dim transaction = Await _dbContext.SalesTransactions _
                .Include(Function(t) t.Receipt) _
                .FirstOrDefaultAsync(Function(t) t.Id = transactionId)

            Return transaction?.Receipt
        End Function

        Public Async Function PrintReceiptAsync(receiptId As Integer) As Task(Of String) Implements IReceiptService.PrintReceiptAsync
            ' Get receipt with transaction and lines
            Dim transaction = Await _dbContext.SalesTransactions _
                .Include(Function(t) t.Receipt) _
                .Include(Function(t) t.Lines) _
                .FirstOrDefaultAsync(Function(t) t.Receipt.Id = receiptId)

            If transaction Is Nothing Then
                Throw New ArgumentException($"Receipt with ID {receiptId} not found or has no associated transaction.")
            End If

            Dim sb As New StringBuilder()

            ' 40-character width formatter
            ' Header
            sb.AppendLine(CenterText("VISTA POS MERCHANDISING"))
            sb.AppendLine(CenterText("VAT REG TIN: 123-456-789-000"))
            sb.AppendLine(CenterText("MIN: 123456789"))
            sb.AppendLine(CenterText("PTU: 987654321"))
            sb.AppendLine(StrDup(40, "-"c))

            ' Receipt Info
            sb.AppendLine(PadRightLeft("OR #:", transaction.Receipt.ReceiptNumber, 40))
            sb.AppendLine(PadRightLeft("DATE:", transaction.TransactionDate.ToString("yyyy-MM-dd HH:mm"), 40))
            sb.AppendLine(StrDup(40, "-"c))

            ' Items
            For Each line In transaction.Lines
                sb.AppendLine(PadRightLeft(line.ProductName, "", 40))
                Dim qtyPrice = $"{line.Quantity} x {line.UnitPrice:N2}"
                sb.AppendLine(PadRightLeft("  " & qtyPrice, line.LineTotal.ToString("N2"), 40))
            Next

            sb.AppendLine(StrDup(40, "-"c))

            ' VAT Computation
            Dim totalAmount = transaction.TotalAmount
            Dim vatableSales = Math.Round(totalAmount / 1.12D, 2)
            Dim vatAmount = totalAmount - vatableSales

            sb.AppendLine(PadRightLeft("TOTAL:", totalAmount.ToString("N2"), 40))
            sb.AppendLine(PadRightLeft("VATABLE SALES:", vatableSales.ToString("N2"), 40))
            sb.AppendLine(PadRightLeft("VAT AMOUNT:", vatAmount.ToString("N2"), 40))
            sb.AppendLine(PadRightLeft("VAT EXEMPT:", "0.00", 40))
            sb.AppendLine(PadRightLeft("ZERO RATED:", "0.00", 40))

            sb.AppendLine(StrDup(40, "="c))
            sb.AppendLine(CenterText("THIS DOCUMENT IS NOT VALID FOR"))
            sb.AppendLine(CenterText("CLAIM OF INPUT TAX"))
            sb.AppendLine(CenterText("THANK YOU FOR SHOPPING!"))

            Return sb.ToString()
        End Function

        Private Function CenterText(text As String) As String
            If text.Length >= 40 Then Return text.Substring(0, 40)
            Dim padLeft = (40 + text.Length) \ 2
            Return text.PadLeft(padLeft).PadRight(40)
        End Function

        Private Function PadRightLeft(leftText As String, rightText As String, totalWidth As Integer) As String
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
