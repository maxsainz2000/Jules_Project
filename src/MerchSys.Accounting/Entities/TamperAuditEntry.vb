Imports System

Namespace Entities

    ''' <summary>
    ''' Append-only audit record for receipt tampering incidents.
    ''' This entity deliberately does not implement IAuditedEntity and has no IsDeleted / ModifiedBy / ModifiedAt
    ''' because it must be immutable and append-only for audit integrity.
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