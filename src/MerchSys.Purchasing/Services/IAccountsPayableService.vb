Imports System
Imports System.Collections.Generic
Imports System.Threading.Tasks

Namespace Services
    Public Class AccountsPayableDto
        Public Property Id As Integer
        Public Property VendorId As Integer
        Public Property VendorName As String
        Public Property PurchaseOrderId As Integer?
        Public Property OrderNumber As String
        Public Property InvoiceDate As DateTime
        Public Property DueDate As DateTime
        Public Property TotalAmount As Decimal
        Public Property AmountPaid As Decimal
        Public Property IsPaid As Boolean
    End Class

    Public Class CreateAccountsPayableDto
        Public Property VendorId As Integer
        Public Property PurchaseOrderId As Integer?
        Public Property InvoiceDate As DateTime
        Public Property DueDate As DateTime
        Public Property TotalAmount As Decimal
    End Class

    Public Interface IAccountsPayableService
        Function CreateAsync(dto As CreateAccountsPayableDto) As Task(Of Integer)
        Function RecordPaymentAsync(id As Integer, amount As Decimal) As Task
        Function GetOutstandingAsync() As Task(Of List(Of AccountsPayableDto))
        Function GetOverdueAsync() As Task(Of List(Of AccountsPayableDto))
        Function GetByVendorAsync(vendorId As Integer) As Task(Of List(Of AccountsPayableDto))
        Function GetAllAsync() As Task(Of List(Of AccountsPayableDto))
    End Interface
End Namespace
