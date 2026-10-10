Imports System.Threading
Imports System.Threading.Tasks
Imports MerchSys.POS.Services.ReceiptFormatting

Namespace Services.ReceiptRendering
    Public Class ConsoleReceiptRenderer
        Implements IReceiptRenderer

        Public Function RenderAsync(body As ReceiptBody, Optional receiptNumber As String = Nothing, Optional cancellationToken As CancellationToken = Nothing) As Task(Of String) Implements IReceiptRenderer.RenderAsync
            Return Task.FromResult(body.ToString())
        End Function
    End Class
End Namespace