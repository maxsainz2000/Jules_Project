Imports System
Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class ReceiptIntegrity
        Inherits AuditableEntity
        Implements MerchSys.SharedKernel.Interfaces.IAuditedEntity

        Public Property ReceiptId As Integer
        Public Property OfficialReceipt As OfficialReceipt
        Public Property IntegrityHash As String
        Public Property PreviousHash As String
        Public Property RetentionExpiresAt As DateTime
        Public Property CanonicalPayload As String
        Public Property IsImmutable As Boolean
        Public Property HashAlgorithm As String
    End Class
End Namespace
