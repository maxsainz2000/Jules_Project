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
        Public Property PriceDelta As Decimal
        Public Property ChangedBy As String
        Public Property Reason As String
    End Class

    Public Class ProductPriceHistoryPresenter
        Private ReadOnly _db As InventoryDbContext
        Public ReadOnly Property View As IProductPriceHistoryView

        Public Sub New(view As IProductPriceHistoryView, db As InventoryDbContext)
            View = view
            _db = db

            View.Presenter = Me

            AddHandler View.LoadView, Async Sub(sender, e) Await LoadHistoryAsync()
        End Sub

        Private Async Function LoadHistoryAsync() As Task
            Dim history = Await _db.ProductPriceHistories.
                Where(Function(h) h.ProductId = View.ProductId).
                OrderByDescending(Function(h) h.ChangedAt).
                Select(Function(h) New PriceHistoryRowItem With {
                    .ChangedAt = h.ChangedAt,
                    .OldPrice = h.OldPrice,
                    .NewPrice = h.NewPrice,
                    .PriceDelta = h.NewPrice - h.OldPrice,
                    .ChangedBy = h.ChangedBy,
                    .Reason = h.Reason
                }).
                ToListAsync()

            View.History = history
        End Function
    End Class

End Namespace
