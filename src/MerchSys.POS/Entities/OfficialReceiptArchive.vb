Imports System
Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class OfficialReceiptArchive
        Inherits BaseEntity
        Implements MerchSys.SharedKernel.Interfaces.IAuditedEntity

        Public Property OriginalReceiptId As Integer
        Public Property ReceiptNumber As String
        Public Property TransactionId As Integer
        Public Property ReceiptDate As DateTime
        Public Property SerializedLines As String
        Public Property ArchivedAt As DateTime
        Public Property ArchivedHash As String
    End Class
End Namespace
