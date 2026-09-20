Imports MerchSys.SharedKernel.Interfaces

Namespace Entities
    Public MustInherit Class AuditableEntity
        Inherits BaseEntity
        Implements IAuditable

        Public Property CreatedAt As DateTime Implements IAuditable.CreatedAt
        Public Property CreatedBy As String Implements IAuditable.CreatedBy
        Public Property ModifiedAt As DateTime? Implements IAuditable.ModifiedAt
        Public Property ModifiedBy As String Implements IAuditable.ModifiedBy
    End Class
End Namespace
