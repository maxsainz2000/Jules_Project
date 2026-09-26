Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class ShrinkageRecord
        Inherits AuditableEntity

        Public Property Reason As String
        Public Property QuantityLost As Integer
        Public Property UnitCost As Decimal
        Public Property TotalValue As Decimal
    End Class
End Namespace
