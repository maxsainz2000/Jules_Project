Imports MerchSys.SharedKernel.Enums

Namespace Entities
    Public Class UserAccount
        Inherits AuditableEntity

        Public Property Username As String
        Public Property PasswordHash As String
        Public Property Role As UserRole
        Public Property IsActive As Boolean
        Public Property FailedLoginAttempts As Integer
        Public Property LockedUntil As DateTime?
        Public Property LastPasswordChangeAt As DateTime?

    End Class
End Namespace
