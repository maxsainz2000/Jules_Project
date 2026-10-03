Imports MerchSys.POS.Entities

Namespace Services

    Public Interface ISalesReturnService

        Function ProcessReturnAsync(transactionId As Integer, returnQuantities As Dictionary(Of Integer, Integer), reason As String, restock As Boolean, user As String) As Task(Of SalesReturn)

        Function GetReturnsForTransactionAsync(transactionId As Integer) As Task(Of List(Of SalesReturn))

        Function GetReturnHistoryAsync(startDate As DateTime, endDate As DateTime) As Task(Of List(Of SalesReturn))

    End Interface

End Namespace
