Imports System.Linq

Namespace Services

    Public Class StockoutEstimationService
        Implements IStockoutEstimationService

        Private ReadOnly _velocityService As IVelocityService

        Public Sub New(velocityService As IVelocityService)
            _velocityService = velocityService
        End Sub

        Public Async Function EstimateAllAsync() As Task(Of List(Of StockoutEstimateDto)) Implements IStockoutEstimationService.EstimateAllAsync
            Dim velocities = Await _velocityService.GetProductVelocitiesAsync()

            Dim estimates = New List(Of StockoutEstimateDto)()

            For Each vel In velocities
                Dim estimate = CreateEstimate(vel)
                estimates.Add(estimate)
            Next

            Return estimates
        End Function

        Public Async Function EstimateForProductAsync(productId As Integer) As Task(Of StockoutEstimateDto) Implements IStockoutEstimationService.EstimateForProductAsync
            Dim allEstimates = Await EstimateAllAsync()
            Return allEstimates.FirstOrDefault(Function(e) e.ProductId = productId)
        End Function

        Public Async Function GetCriticalProductsAsync() As Task(Of List(Of StockoutEstimateDto)) Implements IStockoutEstimationService.GetCriticalProductsAsync
            Dim allEstimates = Await EstimateAllAsync()
            Return allEstimates.Where(Function(e) e.RiskLevel = StockoutRiskLevel.Critical).ToList()
        End Function

        Private Function CreateEstimate(vel As ProductVelocityDto) As StockoutEstimateDto
            Dim estimatedDays As Integer = 0
            Dim risk As StockoutRiskLevel = StockoutRiskLevel.Low
            Dim stockoutDate As DateTime = DateTime.MaxValue

            If vel.AverageDailySales > 0 Then
                estimatedDays = CInt(Math.Floor(vel.CurrentStock / vel.AverageDailySales))
                If estimatedDays < 0 Then estimatedDays = 0

                stockoutDate = DateTime.Now.AddDays(estimatedDays)

                If estimatedDays <= 7 Then
                    risk = StockoutRiskLevel.Critical
                ElseIf estimatedDays <= 14 Then
                    risk = StockoutRiskLevel.High
                ElseIf estimatedDays <= 30 Then
                    risk = StockoutRiskLevel.Medium
                Else
                    risk = StockoutRiskLevel.Low
                End If
            Else
                ' No sales = no stockout risk
                estimatedDays = Integer.MaxValue
                risk = StockoutRiskLevel.Low
            End If

            Return New StockoutEstimateDto With {
                .ProductId = vel.ProductId,
                .ProductName = vel.ProductName,
                .CurrentStock = vel.CurrentStock,
                .AvgDailySales = vel.AverageDailySales,
                .EstimatedDaysUntilStockout = estimatedDays,
                .RiskLevel = risk,
                .EstimatedStockoutDate = stockoutDate
            }
        End Function

    End Class

End Namespace
