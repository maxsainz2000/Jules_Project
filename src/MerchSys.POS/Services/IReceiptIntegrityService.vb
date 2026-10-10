Imports System
Imports System.Threading.Tasks
Imports MerchSys.POS.Entities

Namespace Services
    Public Class IntegrityValidationResult
        Public Property IsValid As Boolean
        Public Property ExpectedHash As String
        Public Property ActualHash As String
        Public Property ErrorMessage As String
    End Class

    Public Class ChainValidationResult
        Public Property IsValid As Boolean
        Public Property BrokenAtReceiptNumber As String
        Public Property ErrorMessage As String
        Public Property TotalVerified As Integer
    End Class

    Public Interface IReceiptIntegrityService
        Function ComputeAndPersistAsync(receiptId As Integer) As Task(Of ReceiptIntegrity)
        Function ValidateAsync(receiptId As Integer) As Task(Of IntegrityValidationResult)
        Function ValidateChainAsync() As Task(Of ChainValidationResult)
        Function GetNextReceiptNumberAsync(year As Integer) As Task(Of String)
    End Interface
End Namespace