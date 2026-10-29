Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports MerchSys.App.Models
Imports MerchSys.App.Services.Theming
Imports MerchSys.App.Views
Imports MerchSys.SharedKernel.Queries

Namespace Presenters
    Public Class CommandPalettePresenter
        Implements IDisposable

        Private ReadOnly _view As ICommandPaletteView
        Private ReadOnly _mainWindowPresenter As MainWindowPresenter
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _themeService As IThemeService

        Private _cts As CancellationTokenSource
        Private _disposed As Boolean

        Public Sub New(view As ICommandPaletteView,
                       mainWindowPresenter As MainWindowPresenter,
                       mediator As IMediator,
                       themeService As IThemeService)
            _view = view
            _mainWindowPresenter = mainWindowPresenter
            _mediator = mediator
            _themeService = themeService

            AddHandler _view.SearchQueryChanged, AddressOf OnSearchQueryChanged
            AddHandler _view.ItemSelected, AddressOf OnItemSelected
            AddHandler _view.DismissRequested, AddressOf OnDismissRequested

            If _themeService IsNot Nothing AndAlso TypeOf _view Is System.Windows.Forms.Control Then
                _themeService.ApplyTheme(DirectCast(_view, System.Windows.Forms.Control))
            End If
        End Sub

        Public Sub New(view As ICommandPaletteView,
                       mainWindowPresenter As MainWindowPresenter,
                       mediator As IMediator)
            Me.New(view, mainWindowPresenter, mediator, Nothing)
        End Sub

        Private Async Sub OnSearchQueryChanged(sender As Object, query As String)
            If _cts IsNot Nothing Then
                _cts.Cancel()
                _cts.Dispose()
            End If

            _cts = New CancellationTokenSource()
            Dim token = _cts.Token

            If String.IsNullOrWhiteSpace(query) Then
                ' Default state - Show all navigable items
                Dim allItems = _mainWindowPresenter.AllNavigableItems.Select(Function(n) New CommandPaletteItem With {
                    .Title = n.Name,
                    .Category = "Screens",
                    .Kind = CommandPaletteItemKind.Screen,
                    .NavigationItem = n,
                    .Subtitle = n.NavigationGroup
                }).ToList()
                _view.SetResults(allItems)
                _view.IsEmpty = False
                _view.IsBusy = False
                Return
            End If

            Try
                ' Debounce
                Await Task.Delay(250, token)

                _view.IsBusy = True

                ' Sync screens filtering
                Dim screens = _mainWindowPresenter.AllNavigableItems.
                    Where(Function(n) n.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).
                    Select(Function(n) New CommandPaletteItem With {
                        .Title = n.Name,
                        .Category = "Screens",
                        .Kind = CommandPaletteItemKind.Screen,
                        .NavigationItem = n,
                        .Subtitle = n.NavigationGroup
                    }).ToList()

                ' Async product filtering
                Dim productsQuery = New GetProductsForCatalogQuery()
                Dim allProducts = Await _mediator.Send(productsQuery, token)
                Dim products = allProducts.
                    Where(Function(p) p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).
                    Take(5).
                    Select(Function(p) New CommandPaletteItem With {
                        .Title = p.Name,
                        .Category = "Products",
                        .Kind = CommandPaletteItemKind.Product,
                        .ProductId = p.Id,
                        .Subtitle = $"${p.RetailPrice:N2}"
                    }).ToList()

                Dim results = screens.Concat(products).ToList()

                _view.SetResults(results)
                _view.IsEmpty = (results.Count = 0)
            Catch ex As OperationCanceledException
                ' Ignored
            Finally
                If Not token.IsCancellationRequested Then
                    _view.IsBusy = False
                End If
            End Try
        End Sub

        Private Sub OnItemSelected(sender As Object, item As CommandPaletteItem)
            If item.Kind = CommandPaletteItemKind.Screen AndAlso item.NavigationItem IsNot Nothing Then
                _mainWindowPresenter.NavigateTo(item.NavigationItem)
            ElseIf item.Kind = CommandPaletteItemKind.Product Then
                ' Not explicitly specified where to navigate for product, so let's try to navigate to Stock Dashboard or similar
                ' For now, just dismiss, assuming product selection navigation might be a future enhancement
            End If
            _view.HidePalette()
        End Sub

        Private Sub OnDismissRequested(sender As Object, e As EventArgs)
            _view.HidePalette()
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposed Then
                If disposing Then
                    RemoveHandler _view.SearchQueryChanged, AddressOf OnSearchQueryChanged
                    RemoveHandler _view.ItemSelected, AddressOf OnItemSelected
                    RemoveHandler _view.DismissRequested, AddressOf OnDismissRequested

                    If _cts IsNot Nothing Then
                        _cts.Cancel()
                        _cts.Dispose()
                        _cts = Nothing
                    End If
                End If
                _disposed = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
