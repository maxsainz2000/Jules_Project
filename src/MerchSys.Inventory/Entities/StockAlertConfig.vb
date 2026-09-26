Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class StockAlertConfig
        Inherits AuditableEntity

        Public Property ExpiryAlertDays As Integer = 30
        Public Property RowVersion As Byte()
    End Class
End Namespace
