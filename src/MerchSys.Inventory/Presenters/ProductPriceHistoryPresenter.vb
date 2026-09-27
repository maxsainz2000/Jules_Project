Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data
Imports MerchSys.Inventory.Views

Namespace Presenters

    Public Class PriceHistoryRowItem
        Public Property ChangedAt As DateTime
        Public Property OldPrice As Decimal
        Public Property NewPrice As Decimal
        Public Property ChangedBy As String
        Public Property Reason As String
    End Class

    Public Class ProductPriceHistoryPresenter

        Private ReadOnly _db As InventoryDbContext
        Private ReadOnly _view As IProductPriceHistoryView

        Public Sub New(db As InventoryDbContext, view As IProductPriceHistoryView)
            _db = db
            _view = view
        End Sub

        Public ReadOnly Property View As IProductPriceHistoryView
            Get
                Return _view
            End Get
        End Property

        Public Async Function LoadHistoryAsync(productId As Integer) As Task
            Dim histories = Await _db.ProductPriceHistories.
                Where(Function(h) h.ProductId = productId).
                OrderByDescending(Function(h) h.ChangedAt).
                Select(Function(h) New PriceHistoryRowItem With {
                    .ChangedAt = h.ChangedAt,
                    .OldPrice = h.OldPrice,
                    .NewPrice = h.NewPrice,
                    .ChangedBy = h.ChangedBy,
                    .Reason = h.Reason
                }).
                ToListAsync()

            _view.PriceHistories = histories
        End Function

    End Class

End Namespace
