Imports System

Namespace Entities

    ''' <summary>
    ''' Represents an append-only audit record for receipt tampering incidents.
    ''' Note: This entity does not implement IAuditedEntity and therefore lacks
    ''' IsDeleted, ModifiedBy, and ModifiedAt properties, as it is an immutable log entry.
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
