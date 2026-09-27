Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class ReorderConfig
        Inherits BaseEntity

        Public Property ProductId As Integer
        Public Property ReorderThreshold As Integer
        Public Property SafetyStock As Integer
        Public Property LeadTimeDays As Integer
        Public Property SeasonalMultiplier As Decimal = 1D
        Public Property PreferredVendorId As Integer?
        Public Property RowVersion As Byte()
    End Class
End Namespace
