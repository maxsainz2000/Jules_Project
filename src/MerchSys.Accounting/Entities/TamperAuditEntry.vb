Imports System

Namespace Entities

    ''' <summary>
    ''' Represents an append-only audit record of a detected receipt tamper event.
    ''' This entity intentionally omits standard audit columns (IsDeleted, ModifiedBy, ModifiedAt)
    ''' and does NOT implement IAuditedEntity because it is strictly an immutable,
    ''' insert-only ledger record for security auditing.
    ''' </summary>
    Public Class TamperAuditEntry

        Public Property Id As Integer
        Public Property ReceiptId As Guid
        Public Property ReceiptNumber As String
        Public Property ExpectedHash As String
        Public Property ActualHash As String
        Public Property TamperKind As String
        Public Property DetectedAt As DateTime
        Public Property DetectedBy As String
        Public Property MachineName As String
        Public Property WindowsUser As String

    End Class

End Namespace
