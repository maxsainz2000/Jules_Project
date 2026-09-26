Imports System.Threading
Imports MediatR
Imports MerchSys.SharedKernel.Queries
Imports MerchSys.Inventory.Services

Namespace Handlers

    Public Class GetCurrentStockHandler
        Implements IRequestHandler(Of GetCurrentStockQuery, GetCurrentStockResult)

        Private ReadOnly _stockService As IStockService

        Public Sub New(stockService As IStockService)
            _stockService = stockService
        End Sub

        Public Async Function Handle(request As GetCurrentStockQuery, cancellationToken As CancellationToken) As Task(Of GetCurrentStockResult) Implements IRequestHandler(Of GetCurrentStockQuery, GetCurrentStockResult).Handle
            Dim stockLevelsDto = Await _stockService.GetCurrentStockLevelsAsync()

            Dim result = New GetCurrentStockResult()

            For Each dto In stockLevelsDto
                result.StockLevels.Add(New ProductStockLevel With {
                    .ProductId = dto.ProductId,
                    .CurrentQuantity = dto.CurrentQuantity,
                    .ThresholdQuantity = dto.ThresholdQuantity,
                    .IsBelowThreshold = dto.IsBelowThreshold
                })
            Next

            Return result
        End Function
    End Class

End Namespace
