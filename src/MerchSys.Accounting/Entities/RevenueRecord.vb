Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Entities
    Public Class RevenueRecord
        Inherits AuditableEntity

        Public Property RecordDate As DateTime
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property QuantitySold As Decimal
        Public Property UnitPrice As Decimal
        Public Property TotalRevenue As Decimal
        Public Property UnitCost As Decimal
        Public Property TotalCogs As Decimal
        Public Property GrossProfit As Decimal
        Public Property PaymentMethod As PaymentMethod
        Public Property SourceTransactionId As Integer
        Public Property SourceTransactionLineId As Integer?
    End Class
End Namespace