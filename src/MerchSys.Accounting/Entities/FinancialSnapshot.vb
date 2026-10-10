Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class FinancialSnapshot
        Inherits AuditableEntity

        Public Property SnapshotDate As DateTime
        Public Property TotalAR As Decimal
        Public Property TotalAP As Decimal
        Public Property InventoryValue As Decimal
        Public Property TodayRevenue As Decimal
        Public Property MtdRevenue As Decimal
        Public Property YtdRevenue As Decimal
    End Class
End Namespace