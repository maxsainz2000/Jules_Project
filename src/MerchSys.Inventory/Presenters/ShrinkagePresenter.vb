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
            Dim products = New List(Of ShrinkageProductItem)()
            Dim batches = New List(Of ShrinkageBatchItem)()
            Dim connection = CType(_db.Database.GetDbConnection(), MySqlConnection)
            Dim wasClosed = connection.State = System.Data.ConnectionState.Closed
            If wasClosed Then
                Await connection.OpenAsync()
            End If

            Dim savedEx As Exception = Nothing
            Try
                Using cmd = connection.CreateCommand()
                    cmd.CommandText = "SELECT Id, Name, SKU FROM Inv_Products WHERE IsDeleted = 0 ORDER BY Name"
                    Using reader = Await cmd.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            products.Add(New ShrinkageProductItem With {
                                .Id = reader.GetInt32(0),
                                .Name = reader.GetString(1),
                                .SKU = If(reader.IsDBNull(2), String.Empty, reader.GetString(2))
                            })
                        End While
                    End Using
                End Using

                Using cmd = connection.CreateCommand()
                    cmd.CommandText = "SELECT Id, ProductId, ReceiptDate, QuantityRemaining FROM Inv_StockBatches WHERE QuantityRemaining > 0 ORDER BY ReceiptDate"
                    Using reader = Await cmd.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            Dim batchId = reader.GetInt32(0)
                            Dim productId = reader.GetInt32(1)
                            Dim rDate = reader.GetDateTime(2)
                            Dim qRem = reader.GetInt32(3)
                            batches.Add(New ShrinkageBatchItem With {
                                .Id = batchId,
                                .ProductId = productId,
                                .ReceiptDate = rDate,
                                .QuantityRemaining = qRem,
                                .DisplayText = $"Batch {batchId} ({rDate:yyyy-MM-dd}) - Qty: {qRem}"
                            })
                        End While
                    End Using
                End Using

            Catch ex As Exception
                savedEx = ex
            End Try

            If wasClosed Then
                Await connection.CloseAsync()
            End If

            If savedEx IsNot Nothing Then
                Throw savedEx
            End If

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
