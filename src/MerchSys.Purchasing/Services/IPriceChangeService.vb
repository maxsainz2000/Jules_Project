Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Entities

Namespace Services
    Public Interface IPriceChangeService
        Function DetectChangesAsync(goodsReceiptId As Integer) As Task
        Function GetUnacknowledgedAsync() As Task(Of List(Of PriceChangeAlert))
        Function AcknowledgeAsync(alertId As Integer) As Task
        Function GetHistoryForProductAsync(productId As Integer) As Task(Of List(Of PriceChangeAlert))
    End Interface
End Namespace
