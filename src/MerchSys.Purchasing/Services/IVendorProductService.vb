Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports MerchSys.Purchasing.Entities
Imports MerchSys.SharedKernel.Paging

Namespace Services
    Public Class VendorProductDto
        Public Property Id As Integer
        Public Property VendorId As Integer
        Public Property ProductId As Integer
        Public Property UnitCost As Decimal
        Public Property Notes As String
        Public Property ProductName As String ' Populated via MediatR if needed
    End Class

    Public Interface IVendorProductService
        Function GetHistoryAsync(request As PageRequest) As Task(Of PagedResult(Of VendorProduct))
        Function AddCatalogEntryAsync(vendorId As Integer, productId As Integer, unitCost As Decimal, notes As String) As Task(Of VendorProduct)
        Function GetCatalogForVendorAsync(vendorId As Integer) As Task(Of List(Of VendorProductDto))
        Function RemoveCatalogEntryAsync(id As Integer) As Task
    End Interface
End Namespace
