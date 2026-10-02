Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Entities
Imports MerchSys.SharedKernel.Paging

Namespace Services
    Public Interface IVendorProductService
        Function GetHistoryAsync(request As PageRequest) As Task(Of PagedResult(Of VendorProduct))
        Function AddCatalogEntryAsync(vendorId As Integer, productId As Integer, unitCost As Decimal, notes As String) As Task(Of VendorProduct)
        Function UpdateCatalogEntryAsync(id As Integer, unitCost As Decimal, notes As String) As Task(Of VendorProduct)
        Function DeleteCatalogEntryAsync(id As Integer) As Task
    End Interface
End Namespace