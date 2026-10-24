Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class VatConfiguration
        Inherits BaseEntity
        Implements MerchSys.SharedKernel.Interfaces.IAuditedEntity
        Public Property RowVersion As DateTime
        Public Property IsVatRegistered As Boolean
        Public Property VatRate As Decimal = 0.12D
        Public Property NonVatPercentageTaxRate As Decimal = 0.03D
        Public Property EffectiveFrom As DateTime
        Public Property Tin As String
        Public Property BusinessName As String
        Public Property Address As String
    End Class
End Namespace
