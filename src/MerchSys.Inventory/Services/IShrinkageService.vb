Imports System.Threading.Tasks
Imports System.Collections.Generic

Namespace Services

    Public Interface IShrinkageService
        Function RecordShrinkageAsync(productId As Integer, quantity As Integer, reason As String, batchId As Integer?) As Task
        Function GetShrinkageHistoryAsync() As Task(Of List(Of ShrinkageHistoryDto))
        Function GetTotalShrinkageValueAsync() As Task(Of Decimal)
    End Interface

    Public Class ShrinkageHistoryDto
        Public Property Id As Integer
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property Reason As String
        Public Property QuantityLost As Integer
        Public Property UnitCost As Decimal
        Public Property TotalValue As Decimal
        Public Property CreatedAt As DateTime
    End Class

End Namespace
