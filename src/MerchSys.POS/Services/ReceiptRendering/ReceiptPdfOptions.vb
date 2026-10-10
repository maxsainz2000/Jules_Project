Namespace Services.ReceiptRendering
    Public Class ReceiptPdfOptions
        Public Property OutputDirectory As String = "Receipts"
        Public Property FileNamePattern As String = "OR-{ReceiptNumber}.pdf"
        Public Property FontFamily As String = "Courier New"
        Public Property FontSizePt As Single = 9.0F
    End Class
End Namespace