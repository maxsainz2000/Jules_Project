Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Services
Imports MerchSys.Inventory.Views
Imports MySqlConnector

Namespace Presenters

    Public Class ShrinkageRowItem
        Public Property Id As Integer
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property Reason As String
        Public Property QuantityLost As Integer
        Public Property UnitCost As Decimal
        Public Property TotalValue As Decimal
        Public Property CreatedAt As DateTime
    End Class

    Public Class ShrinkageProductItem
        Public Property Id As Integer
        Public Property Name As String
        Public Property SKU As String
    End Class

    Public Class ShrinkageBatchItem
        Public Property Id As Integer
        Public Property ProductId As Integer
        Public Property ReceiptDate As DateTime
        Public Property QuantityRemaining As Integer
        Public Property DisplayText As String
    End Class

    Public Class ShrinkageRecordRequest
        Inherits EventArgs
        Public Property ProductId As Integer
        Public Property Quantity As Integer
        Public Property Reason As String
        Public Property BatchId As Integer?
    End Class

    Public Class ShrinkagePresenter
        Private ReadOnly _view As IShrinkageView
        Private ReadOnly _shrinkageService As IShrinkageService
        Private ReadOnly _db As InventoryDbContext

        Private _fullHistory As List(Of ShrinkageHistoryDto)

        Public Sub New(view As IShrinkageView, shrinkageService As IShrinkageService, db As InventoryDbContext)
            _view = view
            _shrinkageService = shrinkageService
            _db = db

            _view.Presenter = Me

            AddHandler _view.LoadView, Async Sub(sender, e) Await HandleLoadViewAsync()
            AddHandler _view.FilterChanged, Sub(sender, e) ApplyFilters()
            AddHandler _view.RecordShrinkageRequested, Async Sub(sender, e) Await RecordShrinkageAsync(e)
        End Sub

        Private Async Function HandleLoadViewAsync() As Task
            Await LoadLookupsAsync()
            Await LoadHistoryAsync()
        End Function

        Private Async Function LoadLookupsAsync() As Task
            ' Plain EF queries. The raw readers this replaced existed only for the
            ' "EF Core ToListAsync returns an empty list" workaround, which was retested
            ' against MariaDB on 2026-10-12 and did not reproduce (see AGENTS.md).
            ' Soft-deleted rows are excluded by BaseDbContext's global query filter.
            Dim products = Await _db.Products.
                OrderBy(Function(p) p.Name).
                Select(Function(p) New ShrinkageProductItem With {
                    .Id = p.Id,
                    .Name = p.Name,
                    .SKU = If(p.SKU, String.Empty)
                }).
                ToListAsync()

            ' DisplayText is composed after materialising: a formatted-string projection would
            ' not translate to SQL.
            Dim batchRows = Await _db.StockBatches.
                Where(Function(b) b.QuantityRemaining > 0).
                OrderBy(Function(b) b.ReceiptDate).
                Select(Function(b) New With {
                    .Id = b.Id,
                    .ProductId = b.ProductId,
                    .ReceiptDate = b.ReceiptDate,
                    .QuantityRemaining = b.QuantityRemaining
                }).
                ToListAsync()

            Dim batches = batchRows.Select(Function(b) New ShrinkageBatchItem With {
                    .Id = b.Id,
                    .ProductId = b.ProductId,
                    .ReceiptDate = b.ReceiptDate,
                    .QuantityRemaining = b.QuantityRemaining,
                    .DisplayText = $"Batch {b.Id} ({b.ReceiptDate:yyyy-MM-dd}) - Qty: {b.QuantityRemaining}"
                }).ToList()

            _view.AvailableProducts = products
            _view.AvailableBatches = batches
        End Function

        Private Async Function LoadHistoryAsync() As Task
            _fullHistory = Await _shrinkageService.GetShrinkageHistoryAsync()
            ApplyFilters()
        End Function

        Private Sub ApplyFilters()
            If _fullHistory Is Nothing Then Return

            Dim query = _fullHistory.AsEnumerable()

            If _view.FilterStartDate.HasValue Then
                Dim sd = _view.FilterStartDate.Value.Date
                query = query.Where(Function(x) x.CreatedAt.Date >= sd)
            End If

            If _view.FilterEndDate.HasValue Then
                Dim ed = _view.FilterEndDate.Value.Date
                query = query.Where(Function(x) x.CreatedAt.Date <= ed)
            End If

            If Not String.IsNullOrWhiteSpace(_view.FilterReason) AndAlso _view.FilterReason <> "All" Then
                Dim reason = _view.FilterReason
                query = query.Where(Function(x) String.Equals(x.Reason, reason, StringComparison.OrdinalIgnoreCase))
            End If

            Dim filtered = query.ToList()

            _view.ShrinkageHistory = filtered.Select(Function(x) New ShrinkageRowItem With {
                .Id = x.Id,
                .ProductId = x.ProductId,
                .ProductName = x.ProductName,
                .Reason = x.Reason,
                .QuantityLost = x.QuantityLost,
                .UnitCost = x.UnitCost,
                .TotalValue = x.TotalValue,
                .CreatedAt = x.CreatedAt
            }).ToList()

            _view.SummaryTotalItemsLost = filtered.Sum(Function(x) x.QuantityLost)
            _view.SummaryTotalValueLost = filtered.Sum(Function(x) x.TotalValue)
        End Sub

        Private Async Function RecordShrinkageAsync(e As ShrinkageRecordRequest) As Task
            Await _shrinkageService.RecordShrinkageAsync(e.ProductId, e.Quantity, e.Reason, e.BatchId)
            Await LoadLookupsAsync()
            Await LoadHistoryAsync()
        End Function

    End Class

End Namespace
