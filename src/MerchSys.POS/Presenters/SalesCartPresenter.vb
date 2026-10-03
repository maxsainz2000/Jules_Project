Imports System
Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports System.Linq
Imports MediatR
Imports MerchSys.POS.Services
Imports MerchSys.POS.Views
Imports MerchSys.SharedKernel.Queries

Namespace Presenters

    Public Class SalesCartPresenter
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _cartService As ICartService
        Private ReadOnly _paymentService As IPaymentService
        Private ReadOnly _creditService As ICreditService
        Private ReadOnly _receiptService As IReceiptService

        Private _view As ISalesCartView
        Private _searchResults As List(Of ProductCatalogItem) = New List(Of ProductCatalogItem)()
        Private ReadOnly _cartId As String = Guid.NewGuid().ToString()
        Private ReadOnly _syncContext As TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext()

        Public Sub New(mediator As IMediator, cartService As ICartService, paymentService As IPaymentService, creditService As ICreditService, receiptService As IReceiptService)
            _mediator = mediator
            _cartService = cartService
            _paymentService = paymentService
            _creditService = creditService
            _receiptService = receiptService
        End Sub

        Public Property View As ISalesCartView
            Get
                Return _view
            End Get
            Set(value As ISalesCartView)
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.SearchProduct, AddressOf OnSearchProduct
                    AddHandler _view.AddToCart, AddressOf OnAddToCart
                    AddHandler _view.UpdateQty, AddressOf OnUpdateQty
                    AddHandler _view.ProcessPayment, AddressOf OnProcessPayment
                    AddHandler _view.PreviewReceipt, AddressOf OnPreviewReceipt
                End If
            End Set
        End Property

        Private Sub OnSearchProduct(sender As Object, e As String)
            Dim query = New GetProductCatalogQuery With { .SearchTerm = e }
            _mediator.Send(query).ContinueWith(
                Sub(t)
                    If Not t.IsFaulted AndAlso t.Result IsNot Nothing Then
                        _searchResults = t.Result.Items
                    End If
                End Sub, _syncContext)
        End Sub

        Private Sub OnAddToCart(sender As Object, e As Integer)
            Dim product = _searchResults.FirstOrDefault(Function(p) p.ProductId = e)
            If product IsNot Nothing Then
                _cartService.AddToCart(_cartId, product.ProductId, product.ProductName, product.UnitPrice, 1)
                RefreshCartView()
            End If
        End Sub

        Private Sub OnUpdateQty(sender As Object, e As UpdateQtyEventArgs)
            Dim productId = e.ProductId
            Dim newQuantity = e.NewQuantity

            Dim cartDto = _cartService.GetCart(_cartId)
            Dim lineItem = cartDto.Lines.FirstOrDefault(Function(l) l.ProductId = productId)

            If lineItem IsNot Nothing Then
                _cartService.RemoveFromCart(_cartId, productId)
                _cartService.AddToCart(_cartId, lineItem.ProductId, lineItem.ProductName, lineItem.UnitPrice, newQuantity)
                RefreshCartView()
            Else
                Dim product = _searchResults.FirstOrDefault(Function(p) p.ProductId = productId)
                If product IsNot Nothing Then
                    _cartService.AddToCart(_cartId, product.ProductId, product.ProductName, product.UnitPrice, newQuantity)
                    RefreshCartView()
                End If
            End If
        End Sub

        Private Sub OnProcessPayment(sender As Object, e As EventArgs)
            If _view.CartLines Is Nothing OrElse Not _view.CartLines.Any() Then
                Throw New Exception("Cart is empty")
            End If

            If _view.PaymentMethod = "Cash" AndAlso _view.CashTendered < _view.Total Then
                Throw New Exception("Insufficient cash")
            End If

            If _view.PaymentMethod = "Credit" Then
                If Not _view.SelectedCustomer.HasValue Then
                    Throw New Exception("Customer must be selected for credit payment")
                End If

                _creditService.GetAccountAsync(_view.SelectedCustomer.Value).ContinueWith(
                    Sub(t)
                        If t.IsFaulted Then
                            Throw t.Exception
                        End If
                        Dim account = t.Result
                        If account.IsBlocked OrElse account.CurrentBalance > 0 Then
                            Throw New CreditBlockedException("Credit account blocked")
                        End If

                        ProceedWithFinalizeTransaction()
                    End Sub, _syncContext)
            Else
                ProceedWithFinalizeTransaction()
            End If
        End Sub

        Private Sub ProceedWithFinalizeTransaction()
            _cartService.FinalizeTransactionAsync(_cartId, _view.PaymentMethod, _view.SelectedCustomer).ContinueWith(
                Sub(t)
                    If t.IsFaulted Then
                        Throw t.Exception
                    End If
                    Dim transaction = t.Result
                    _paymentService.ProcessPaymentAsync(transaction.Id, _view.CashTendered, _view.PaymentMethod, "").ContinueWith(
                        Sub(pt)
                            If pt.IsFaulted Then
                                Throw pt.Exception
                            End If
                            _cartService.ClearCart(_cartId)
                            RefreshCartView()
                        End Sub, _syncContext)
                End Sub, _syncContext)
        End Sub

        Private Sub OnPreviewReceipt(sender As Object, e As Integer)
            _receiptService.GetReceiptByTransactionAsync(e).ContinueWith(
                Sub(t)
                    If Not t.IsFaulted AndAlso t.Result IsNot Nothing Then
                        _receiptService.PrintReceiptAsync(t.Result.Id).ContinueWith(
                            Sub(pt)
                                If Not pt.IsFaulted AndAlso pt.Result IsNot Nothing Then
                                    _view.ReceiptPreview = pt.Result
                                End If
                            End Sub, _syncContext)
                    End If
                End Sub, _syncContext)
        End Sub

        Private Sub RefreshCartView()
            Dim cartDto = _cartService.GetCart(_cartId)
            _view.CartLines = cartDto.Lines
            _view.Total = cartDto.TotalAmount
        End Sub

    End Class

End Namespace
