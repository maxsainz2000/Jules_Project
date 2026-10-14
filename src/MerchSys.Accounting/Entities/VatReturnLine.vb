Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class VatReturnLine
        Inherits AuditableEntity
        Implements MerchSys.SharedKernel.Interfaces.IAuditedEntity

        Public Property VatReturnId As Integer
        Public Property LineNumber As Integer
        Public Property LineCode As String
        Public Property Description As String
        Public Property Amount As Decimal
        Public Property TaxRate As Decimal
        Public Property TaxAmount As Decimal
        Public Overridable Property VatReturn As VatReturn
    End Class
End Namespace
