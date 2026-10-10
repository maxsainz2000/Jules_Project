Imports System.Threading
Imports System.Threading.Tasks
Imports MerchSys.POS.Services.ReceiptFormatting

Namespace Services.ReceiptRendering
    Public Interface IReceiptRenderer
        Function RenderAsync(body As ReceiptBody, Optional receiptNumber As String = Nothing, Optional cancellationToken As CancellationToken = Nothing) As Task(Of String)
    End Interface
End Namespace