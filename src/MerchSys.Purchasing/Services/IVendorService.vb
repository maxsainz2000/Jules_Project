Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System
Imports MerchSys.Purchasing.Entities
Imports MerchSys.SharedKernel.Paging

Namespace Services
    Public Class CreateVendorDto
        Public Property Name As String
        Public Property LeadTimeDays As Integer
    End Class

    Public Class UpdateVendorDto
        Public Property Id As Integer
        Public Property Name As String
        Public Property LeadTimeDays As Integer
    End Class

    Public Class VendorDetailDto
        Public Property Id As Integer
        Public Property Name As String
        Public Property LeadTimeDays As Integer
        Public Property IsDeleted As Boolean
    End Class

    Public Class VendorPurchaseHistoryDto
        Public Property VendorId As Integer
        Public Property VendorName As String
        Public Property TotalOrders As Integer
        Public Property TotalAmount As Decimal
        Public Property LastOrderDate As DateTime?
    End Class

    Public Interface IVendorService
        Function CreateAsync(dto As CreateVendorDto) As Task(Of VendorDetailDto)
        Function UpdateAsync(id As Integer, dto As UpdateVendorDto) As Task(Of VendorDetailDto)
        Function DeleteAsync(id As Integer) As Task(Of Boolean)
        Function GetByIdAsync(id As Integer) As Task(Of VendorDetailDto)
        Function GetAllAsync() As Task(Of List(Of VendorDetailDto))
        Function SearchAsync(searchTerm As String) As Task(Of List(Of VendorDetailDto))
        Function GetPurchaseHistoryAsync(vendorId As Integer) As Task(Of VendorPurchaseHistoryDto)
        Function GetHistoryAsync(request As PageRequest) As Task(Of PagedResult(Of Vendor))
    End Interface
End Namespace
