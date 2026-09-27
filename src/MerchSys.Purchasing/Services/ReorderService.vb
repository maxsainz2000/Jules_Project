Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Purchasing.Entities
Imports System.Threading.Tasks
Imports MediatR
Imports MerchSys.SharedKernel.Queries

Namespace Services
    Public Class ReorderService
        Implements IReorderService

        Private ReadOnly _dbContext As PurchasingDbContext
        Private ReadOnly _mediator As IMediator

        Public Sub New(dbContext As PurchasingDbContext, mediator As IMediator)
            _dbContext = dbContext
            _mediator = mediator
        End Sub

        Public Async Function GenerateSuggestionsAsync() As Task Implements IReorderService.GenerateSuggestionsAsync
            Dim stockResult = Await _mediator.Send(New GetCurrentStockQuery())
            Dim configs = _dbContext.ReorderConfigs.Select(Function(c) New With {
                .ProductId = c.ProductId,
                .ReorderThreshold = c.ReorderThreshold,
                .SafetyStock = c.SafetyStock,
                .LeadTimeDays = c.LeadTimeDays,
                .SeasonalMultiplier = c.SeasonalMultiplier
            }).ToList()

            Dim configsDict = configs.ToDictionary(Function(c) c.ProductId)

            Dim pendingSuggestions = _dbContext.ReorderSuggestions.
                Where(Function(s) s.Status = "Pending").
                Select(Function(s) s.ProductId).
                ToHashSet()

            For Each stockLevel In stockResult.StockLevels
                If Not configsDict.ContainsKey(stockLevel.ProductId) Then Continue For

                Dim config = configsDict(stockLevel.ProductId)
                Dim effectiveThreshold = CInt(Math.Ceiling(config.ReorderThreshold * config.SeasonalMultiplier))
                Dim effectiveSafetyStock = CInt(Math.Ceiling(config.SafetyStock * config.SeasonalMultiplier))

                If stockLevel.CurrentQuantity < effectiveThreshold AndAlso Not pendingSuggestions.Contains(stockLevel.ProductId) Then
                    Dim suggestedQty = (effectiveThreshold - stockLevel.CurrentQuantity) + effectiveSafetyStock

                    Dim suggestion = New ReorderSuggestion With {
                        .ProductId = stockLevel.ProductId,
                        .SuggestedQuantity = suggestedQty,
                        .Status = "Pending",
                        .Reason = $"Current quantity {stockLevel.CurrentQuantity} is below effective threshold {effectiveThreshold}."
                    }
                    _dbContext.ReorderSuggestions.Add(suggestion)
                End If
            Next

            Await _dbContext.SaveChangesAsync()
        End Function

        Public Async Function GetPendingSuggestionsAsync() As Task(Of List(Of ReorderSuggestion)) Implements IReorderService.GetPendingSuggestionsAsync
            Dim rawList = Await _dbContext.ReorderSuggestions.
                Where(Function(s) s.Status = "Pending").
                Select(Function(s) New With {
                    .Id = s.Id,
                    .ProductId = s.ProductId,
                    .SuggestedQuantity = s.SuggestedQuantity,
                    .Status = s.Status,
                    .ResultingPurchaseOrderId = s.ResultingPurchaseOrderId,
                    .Reason = s.Reason,
                    .RowVersion = s.RowVersion
                }).ToListAsync()
            Return rawList.Select(Function(s) New ReorderSuggestion With {
                    .Id = s.Id,
                    .ProductId = s.ProductId,
                    .SuggestedQuantity = s.SuggestedQuantity,
                    .Status = s.Status,
                    .ResultingPurchaseOrderId = s.ResultingPurchaseOrderId,
                    .Reason = s.Reason,
                    .RowVersion = s.RowVersion
                }).ToList()
        End Function

        Public Async Function AcceptSuggestionAsync(suggestionId As Integer, quantity As Integer) As Task(Of PurchaseOrder) Implements IReorderService.AcceptSuggestionAsync
            Dim suggestion = Await _dbContext.ReorderSuggestions.FirstOrDefaultAsync(Function(s) s.Id = suggestionId)
            If suggestion Is Nothing OrElse suggestion.Status <> "Pending" Then
                Throw New InvalidOperationException("Suggestion not found or not pending.")
            End If

            Dim config = Await _dbContext.ReorderConfigs.FirstOrDefaultAsync(Function(c) c.ProductId = suggestion.ProductId)

            ' Generate PO number
            Dim maxOrderStr = Await _dbContext.Set(Of PurchaseOrder)().
                IgnoreQueryFilters().
                OrderByDescending(Function(p) p.OrderNumber).
                Select(Function(p) p.OrderNumber).
                FirstOrDefaultAsync()

            Dim maxOrderNumber As Long = 0
            If Not String.IsNullOrEmpty(maxOrderStr) Then
                Long.TryParse(maxOrderStr, maxOrderNumber)
            End If
            Dim newOrderNumber As Long = maxOrderNumber + 1

            Dim newPo As New PurchaseOrder With {
                .OrderNumber = newOrderNumber.ToString(),
                .Status = SharedKernel.Enums.PurchaseOrderStatus.Draft,
                .VendorId = If(config?.PreferredVendorId, 0) ' Default to 0 or appropriate missing vendor logic
            }

            _dbContext.PurchaseOrders.Add(newPo)
            Await _dbContext.SaveChangesAsync() ' Save to get newPo.Id

            suggestion.Status = "Accepted"
            suggestion.ResultingPurchaseOrderId = newPo.Id

            ' Ideally we should add a PurchaseOrderLine here for the ProductId and quantity
            Dim poLine = New PurchaseOrderLine With {
                .PurchaseOrderId = newPo.Id,
                .ProductId = suggestion.ProductId,
                .Quantity = quantity,
                .ProductName = $"Product {suggestion.ProductId} (Auto-Reorder)",
                .UnitCost = 0, ' Needs to be fetched from somewhere, defaulting to 0
                .LineTotal = 0 ' UnitCost * quantity
            }
            _dbContext.PurchaseOrderLines.Add(poLine)

            Await _dbContext.SaveChangesAsync()

            Return newPo
        End Function

        Public Async Function DismissSuggestionAsync(suggestionId As Integer, reason As String) As Task Implements IReorderService.DismissSuggestionAsync
            Dim suggestion = Await _dbContext.ReorderSuggestions.FirstOrDefaultAsync(Function(s) s.Id = suggestionId)
            If suggestion Is Nothing OrElse suggestion.Status <> "Pending" Then
                Throw New InvalidOperationException("Suggestion not found or not pending.")
            End If

            suggestion.Status = "Dismissed"
            suggestion.Reason = reason
            Await _dbContext.SaveChangesAsync()
        End Function

        Public Async Function UpdateConfigAsync(config As ReorderConfig) As Task Implements IReorderService.UpdateConfigAsync
            Dim existing = Await _dbContext.ReorderConfigs.FirstOrDefaultAsync(Function(c) c.ProductId = config.ProductId)
            If existing Is Nothing Then
                _dbContext.ReorderConfigs.Add(config)
            Else
                existing.ReorderThreshold = config.ReorderThreshold
                existing.SafetyStock = config.SafetyStock
                existing.LeadTimeDays = config.LeadTimeDays
                existing.SeasonalMultiplier = config.SeasonalMultiplier
                existing.PreferredVendorId = config.PreferredVendorId
            End If
            Await _dbContext.SaveChangesAsync()
        End Function

        Public Async Function GetAllConfigsAsync() As Task(Of List(Of ReorderConfig)) Implements IReorderService.GetAllConfigsAsync
            Dim rawList = Await _dbContext.ReorderConfigs.
                Select(Function(c) New With {
                    .Id = c.Id,
                    .ProductId = c.ProductId,
                    .ReorderThreshold = c.ReorderThreshold,
                    .SafetyStock = c.SafetyStock,
                    .LeadTimeDays = c.LeadTimeDays,
                    .SeasonalMultiplier = c.SeasonalMultiplier,
                    .PreferredVendorId = c.PreferredVendorId,
                    .RowVersion = c.RowVersion
                }).ToListAsync()
            Return rawList.Select(Function(c) New ReorderConfig With {
                    .Id = c.Id,
                    .ProductId = c.ProductId,
                    .ReorderThreshold = c.ReorderThreshold,
                    .SafetyStock = c.SafetyStock,
                    .LeadTimeDays = c.LeadTimeDays,
                    .SeasonalMultiplier = c.SeasonalMultiplier,
                    .PreferredVendorId = c.PreferredVendorId,
                    .RowVersion = c.RowVersion
                }).ToList()
        End Function

        Public Async Function GetAllSuggestionsAsync() As Task(Of List(Of ReorderSuggestion)) Implements IReorderService.GetAllSuggestionsAsync
            Dim rawList = Await _dbContext.ReorderSuggestions.
                Select(Function(s) New With {
                    .Id = s.Id,
                    .ProductId = s.ProductId,
                    .SuggestedQuantity = s.SuggestedQuantity,
                    .Status = s.Status,
                    .ResultingPurchaseOrderId = s.ResultingPurchaseOrderId,
                    .Reason = s.Reason,
                    .RowVersion = s.RowVersion
                }).ToListAsync()
            Return rawList.Select(Function(s) New ReorderSuggestion With {
                    .Id = s.Id,
                    .ProductId = s.ProductId,
                    .SuggestedQuantity = s.SuggestedQuantity,
                    .Status = s.Status,
                    .ResultingPurchaseOrderId = s.ResultingPurchaseOrderId,
                    .Reason = s.Reason,
                    .RowVersion = s.RowVersion
                }).ToList()
        End Function

        ' To maintain compatibility with existing tests/code that might have depended on parameterless AcceptSuggestionAsync
        Public Async Function AcceptSuggestionAsync() As Task(Of PurchaseOrder)
            ' Find the highest order number including soft-deleted items so we don't duplicate sequence numbers.
            ' Policy A: Sequence skips deleted (OrderNumber is deterministic auto-generated).
            Dim maxOrderStr = Await _dbContext.Set(Of PurchaseOrder)().
                IgnoreQueryFilters().
                OrderByDescending(Function(p) p.OrderNumber).
                Select(Function(p) p.OrderNumber).
                FirstOrDefaultAsync()

            Dim maxOrderNumber As Long = 0
            If Not String.IsNullOrEmpty(maxOrderStr) Then
                Long.TryParse(maxOrderStr, maxOrderNumber)
            End If

            Dim newOrderNumber As Long = maxOrderNumber + 1

            Dim newPo As New PurchaseOrder With {
                .OrderNumber = newOrderNumber.ToString()
            }

            _dbContext.Set(Of PurchaseOrder)().Add(newPo)
            Await _dbContext.SaveChangesAsync()

            Return newPo
        End Function
    End Class
End Namespace
