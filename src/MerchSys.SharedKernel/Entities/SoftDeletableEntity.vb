Imports MerchSys.SharedKernel.Interfaces

Namespace Entities
    Public MustInherit Class SoftDeletableEntity
        Inherits AuditableEntity
        Implements ISoftDeletable

        Public Property IsDeleted As Boolean Implements ISoftDeletable.IsDeleted
    End Class
End Namespace
