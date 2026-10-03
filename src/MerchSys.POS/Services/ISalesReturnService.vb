Imports System.Threading.Tasks
Imports MerchSys.POS.Entities

Namespace Services
    Public Interface ISalesReturnService
        Function ProcessReturnAsync(transactionId As Integer, returnQuantities As Dictionary(Of Integer, Integer), reason As String, isRestocked As Boolean) As Task(Of SalesReturn)
        Function GetReturnsForTransactionAsync(transactionId As Integer) As Task(Of List(Of SalesReturn))
        Function GetReturnHistoryAsync() As Task(Of List(Of SalesReturn))
    End Interface
End Namespace