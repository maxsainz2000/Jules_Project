Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Entities
Imports MerchSys.SharedKernel.Paging

Namespace Services
    Public Class CreatePOLineDto
        Public Property ProductName As String
        Public Property UnitCost As Decimal
        Public Property LineTotal As Decimal
    End Class

    Public Interface IPurchaseOrderService
        Function GetHistoryAsync(request As PageRequest) As Task(Of PagedResult(Of PurchaseOrder))
        Function CreateDraftAsync(Optional notes As String = Nothing, Optional expectedDeliveryDate As DateTime? = Nothing) As Task(Of PurchaseOrder)
        Function UpdateDraftAsync(id As Integer, Optional notes As String = Nothing, Optional expectedDeliveryDate As DateTime? = Nothing) As Task(Of PurchaseOrder)
        Function SubmitAsync(id As Integer) As Task
        Function ReceiveAsync(id As Integer) As Task
        Function VerifyAsync(id As Integer) As Task
        Function CloseAsync(id As Integer) As Task
        Function AddLineAsync(id As Integer, line As CreatePOLineDto) As Task(Of PurchaseOrderLine)
        Function RemoveLineAsync(id As Integer, lineId As Integer) As Task
    End Interface
End Namespace
