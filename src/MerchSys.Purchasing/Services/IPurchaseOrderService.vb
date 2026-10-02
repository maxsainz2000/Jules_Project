Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Entities
Imports MerchSys.SharedKernel.Paging
Imports MerchSys.SharedKernel.Enums
Imports System.Collections.Generic

Namespace Services
    Public Class CreatePOLineDto
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property UnitCost As Decimal
        Public Property Quantity As Integer
        Public Property LineTotal As Decimal
    End Class

    Public Interface IPurchaseOrderService
        Function GetHistoryAsync(request As PageRequest) As Task(Of PagedResult(Of PurchaseOrder))
        Function GetByIdAsync(id As Integer) As Task(Of PurchaseOrder)
        Function GetLinesAsync(id As Integer) As Task(Of List(Of PurchaseOrderLine))
        Function DeleteAsync(id As Integer) As Task
        Function SearchAsync(term As String, status As PurchaseOrderStatus?) As Task(Of List(Of PurchaseOrder))
        Function CreateDraftAsync(vendorId As Integer, Optional notes As String = Nothing, Optional expectedDeliveryDate As DateTime? = Nothing) As Task(Of PurchaseOrder)
        Function UpdateDraftAsync(id As Integer, vendorId As Integer, Optional notes As String = Nothing, Optional expectedDeliveryDate As DateTime? = Nothing) As Task(Of PurchaseOrder)
        Function SubmitAsync(id As Integer) As Task
        Function ReceiveAsync(id As Integer) As Task
        Function VerifyAsync(id As Integer) As Task
        Function CloseAsync(id As Integer) As Task
        Function AddLineAsync(id As Integer, line As CreatePOLineDto) As Task(Of PurchaseOrderLine)
        Function RemoveLineAsync(id As Integer, lineId As Integer) As Task
    End Interface
End Namespace
