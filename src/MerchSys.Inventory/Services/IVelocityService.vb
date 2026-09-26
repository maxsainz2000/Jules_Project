Namespace Services

    Public Enum VelocityCategory
        Dead
        Slow
        Moderate
        Fast
    End Enum

    Public Class ProductVelocityDto
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property SKU As String
        Public Property CurrentStock As Integer
        Public Property AverageDailySales As Decimal
        Public Property Category As VelocityCategory
    End Class

    Public Interface IVelocityService
        Function GetProductVelocitiesAsync() As Task(Of List(Of ProductVelocityDto))
    End Interface

End Namespace
