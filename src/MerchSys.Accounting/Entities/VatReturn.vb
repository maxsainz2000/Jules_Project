Imports MerchSys.SharedKernel.Entities
Imports MerchSys.Accounting.Enums

Namespace Entities
    Public Class VatReturn
        Inherits AuditableEntity
        Public Property Year As Integer
        Public Property Period As Integer
        Public Property PeriodType As VatReturnPeriodType
        Public Property FormType As VatReturnFormType
        Public Property FilingStatus As VatFilingStatus
        Public Property PeriodStart As DateTime
        Public Property PeriodEnd As DateTime
        Public Property TotalSales As Decimal
        Public Property TotalPurchases As Decimal
        Public Property OutputVatTotal As Decimal
        Public Property InputVatTotal As Decimal
        Public Property NetVatPayable As Decimal
        Public Property FiledAt As DateTime?
        Public Property FiledBy As String
        Public Property RowVersion As Byte()
        Public Overridable Property Lines As ICollection(Of VatReturnLine) = New List(Of VatReturnLine)
    End Class
End Namespace