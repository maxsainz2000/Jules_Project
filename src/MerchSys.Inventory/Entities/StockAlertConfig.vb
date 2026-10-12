Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class StockAlertConfig
        Inherits AuditableEntity

        Public Property ProductId As Integer
        Public Property ExpiryAlertDays As Integer = 30
        Public Property RowVersion As DateTime
    End Class
End Namespace
