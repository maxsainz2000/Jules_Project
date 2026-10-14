Imports System
Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class ReceiptIntegrityArchive
        Inherits BaseEntity
        Implements MerchSys.SharedKernel.Interfaces.IAuditedEntity

        Public Property ReceiptId As Integer
        Public Property ReceiptNumber As String
        Public Property IntegrityHash As String
        Public Property PreviousHash As String
        Public Property RetentionExpiresAt As DateTime
        Public Property CanonicalPayload As String
        Public Property HashAlgorithm As String
        Public Property ArchivedAt As DateTime
        Public Property ArchivedByService As String
    End Class
End Namespace
