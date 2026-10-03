Imports System
Imports System.Linq
Imports System.Threading.Tasks
Imports MediatR
Imports MerchSys.POS.Views
Imports MerchSys.POS.Services
Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Queries

Namespace Presenters

    Public Class SalesCartPresenter
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _cartService As ICartService
        Private ReadOnly _paymentService As IPaymentService
        Private ReadOnly _creditService As ICreditService
        Private _view As ISalesCartView
        Private Const CartId As String = "MainCart"

        Public Sub New(mediator As IMediator, cartService As ICartService, paymentService As IPaymentService, creditService As ICreditService)
            _mediator = mediator
            _cartService = cartService
            _paymentService = paymentService
            _creditService = creditService
        End Sub

        Public Property View As ISalesCartView
            Get
                Return _view
            End Get
            Set(value As ISalesCartView)
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.SearchProduct, AddressOf OnSearchProductAsync
                    AddHandler _view.AddToCart, AddressOf OnAddToCart
                    AddHandler _view.UpdateQty, AddressOf OnUpdateQty
                    AddHandler _view.ProcessPayment, AddressOf OnProcessPaymentAsync
                    RefreshCartView()
                End If
            End Set
        End Property

        Private Async Sub OnSearchProductAsync(sender As Object, e As EventArgs)
            If _view Is Nothing Then Return

            Dim query As New GetProductCatalogQuery With {
                .SearchTerm = _view.SearchTerm
            }

            Dim result = Await _mediator.Send(query)
            _view.SearchResults = result.Items
        End Sub

        Private Sub OnAddToCart(sender As Object, e As EventArgs)
            If _view Is Nothing OrElse _view.SearchResults Is Nothing Then Return

            Dim productId = _view.SelectedProductId
            Dim product = _view.SearchResults.FirstOrDefault(Function(p) p.ProductId = productId)

            If product IsNot Nothing Then
                _cartService.AddToCart(CartId, product.ProductId, product.ProductName, product.UnitPrice, 1)
                RefreshCartView()
            End If
        End Sub

        Private Sub OnUpdateQty(sender As Object, e As EventArgs)
            If _view Is Nothing Then Return

            Dim productId = _view.SelectedProductId
            Dim newQty = _view.UpdateQuantity

            ' First check if it exists in the cart to get its details
            Dim cart = _cartService.GetCart(CartId)
            Dim lineItem = cart.Lines.FirstOrDefault(Function(l) l.ProductId = productId)

            If lineItem IsNot Nothing Then
                ' Removing it and re-adding it with new quantity
                _cartService.RemoveFromCart(CartId, productId)
                If newQty > 0 Then
                    _cartService.AddToCart(CartId, lineItem.ProductId, lineItem.ProductName, lineItem.UnitPrice, newQty)
                End If
                RefreshCartView()
            End If
        End Sub

        Private Async Sub OnProcessPaymentAsync(sender As Object, e As EventArgs)
            If _view Is Nothing Then Return

            Dim cart = _cartService.GetCart(CartId)

            ' CanPay Gate: empty cart
            If cart.Lines Is Nothing OrElse cart.Lines.Count = 0 Then
                Throw New InvalidOperationException("Cart is empty.")
            End If

            Dim paymentMethod = _view.PaymentMethod

            ' CanPay Gate: insufficient cash
            If String.Equals(paymentMethod, "Cash", StringComparison.OrdinalIgnoreCase) Then
                If _view.CashTendered < cart.TotalAmount Then
                    Throw New InvalidOperationException("Insufficient cash tendered.")
                End If
            End If

            Dim creditAccountId As Integer? = Nothing

            ' CanPay Gate: Credit validations
            If String.Equals(paymentMethod, "Credit", StringComparison.OrdinalIgnoreCase) Then
                Dim account = _view.SelectedCustomer
                If account Is Nothing Then
                    Throw New InvalidOperationException("Customer must be selected for credit payment.")
                End If

                ' Zero-tolerance hard blocking rule
                If account.IsBlocked OrElse account.CurrentBalance > 0 Then
                    Throw New CreditBlockedException("Account is blocked or has an outstanding balance.")
                End If

                creditAccountId = account.Id
            End If

            ' Process Transaction
            Dim transaction = Await _cartService.FinalizeTransactionAsync(CartId, paymentMethod, creditAccountId)

            If String.Equals(paymentMethod, "Cash", StringComparison.OrdinalIgnoreCase) Then
                Await _paymentService.ProcessPaymentAsync(transaction.Id, _view.CashTendered, "Cash", "")
            ElseIf String.Equals(paymentMethod, "Credit", StringComparison.OrdinalIgnoreCase) Then
                Await _creditService.ChargeAccountAsync(creditAccountId.Value, cart.TotalAmount)
            End If

            _cartService.ClearCart(CartId)
            RefreshCartView()
        End Sub

        Private Sub RefreshCartView()
            Dim cart = _cartService.GetCart(CartId)
            _view.CartLines = cart.Lines
            _view.Total = cart.TotalAmount
        End Sub

    End Class

End Namespace
