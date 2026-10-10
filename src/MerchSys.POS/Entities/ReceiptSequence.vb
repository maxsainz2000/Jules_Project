Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class ReceiptSequence
        Inherits BaseEntity

        Public Property Year As Integer
        Public Property LastSequenceNumber As Integer
        Public Property RowVersion As Byte()
    End Class
End Namespace