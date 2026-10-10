Imports System.Threading
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Enums

Namespace Services
    Public Interface IVatReportingService
        Function GenerateVatReturnAsync(year As Integer, period As Integer, formType As VatReturnFormType, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatReturn)
        Function GetVatReturnAsync(returnId As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatReturn)
        Function ListVatReturnsAsync(Optional year As Integer? = Nothing, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of VatReturn))
        Function FileVatReturnAsync(returnId As Integer, filedBy As String, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatReturn)
        Function AmendVatReturnAsync(returnId As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatReturn)
    End Interface
End Namespace
