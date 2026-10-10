Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Extensions.Options
Imports QuestPDF.Fluent
Imports QuestPDF.Helpers
Imports MerchSys.POS.Services.ReceiptFormatting

Namespace Services.ReceiptRendering
    Public Class PdfReceiptRenderer
        Implements IReceiptRenderer

        Private ReadOnly _options As ReceiptPdfOptions

        Public Sub New(options As IOptions(Of ReceiptPdfOptions))
            _options = options.Value
        End Sub

        Public Function RenderAsync(body As ReceiptBody, Optional receiptNumber As String = Nothing, Optional cancellationToken As CancellationToken = Nothing) As Task(Of String) Implements IReceiptRenderer.RenderAsync
            Dim actualReceiptNumber = If(String.IsNullOrWhiteSpace(receiptNumber), "UNKNOWN", receiptNumber)
            Dim fileName = _options.FileNamePattern.Replace("{ReceiptNumber}", actualReceiptNumber)
            Dim outputDir = _options.OutputDirectory

            If Not Directory.Exists(outputDir) Then
                Directory.CreateDirectory(outputDir)
            End If

            Dim fullPath = Path.Combine(outputDir, fileName)

            If File.Exists(fullPath) Then
                Throw New InvalidOperationException($"Receipt PDF file already exists: {fullPath}")
            End If

            Dim opts = _options

            Document.Create(Sub(container)
                                container.Page(Sub(page)
                                                   page.Size(PageSizes.A5)
                                                   page.Margin(20)
                                                   page.Content().Column(Sub(col)
                                                                             For Each line In body.AllLines
                                                                                 col.Item().Text(line).FontFamily(opts.FontFamily).FontSize(opts.FontSizePt)
                                                                             Next
                                                                         End Sub)
                                               End Sub)
                            End Sub).GeneratePdf(fullPath)

            Return Task.FromResult(fullPath)
        End Function
    End Class
End Namespace