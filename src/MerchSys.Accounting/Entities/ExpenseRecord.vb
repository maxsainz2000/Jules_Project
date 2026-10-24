Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Partial Public Class ExpenseRecord
        Inherits AuditableEntity
        Implements MerchSys.SharedKernel.Interfaces.IAuditedEntity

        Public Property ExpenseDate As DateTime
        Public Property Category As String
        Public Property Description As String
        Public Property Amount As Decimal
        Public Property SourceModule As String
        Public Property SourceReferenceId As Integer?
    End Class
End Namespace
