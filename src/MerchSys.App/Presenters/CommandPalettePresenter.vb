Imports MediatR
Imports MerchSys.App.Models
Imports MerchSys.App.Services.Theming
Imports MerchSys.App.Views
Imports MerchSys.SharedKernel.Queries
Imports System.Threading

Namespace Presenters
    Public Class CommandPalettePresenter
        Implements IDisposable

        Private _view As ICommandPaletteView
        Private ReadOnly _mainWindowPresenter As MainWindowPresenter
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _themeService As IThemeService

        Private _cts As CancellationTokenSource

        Public Sub New(view As ICommandPaletteView,
                       mainWindowPresenter As MainWindowPresenter,
                       mediator As IMediator,
                       Optional themeService As IThemeService = Nothing)
            _mainWindowPresenter = mainWindowPresenter
            _mediator = mediator
            _themeService = themeService

            If _themeService IsNot Nothing Then
                AddHandler _themeService.ThemeChanged, AddressOf OnThemeChanged
            End If

            View = view
        End Sub

        Public Property View As ICommandPaletteView
            Get
                Return _view
            End Get
            Set(value As ICommandPaletteView)
                If _view IsNot Nothing Then
                    RemoveHandler _view.SearchQueryChanged, AddressOf OnSearchQueryChanged
                    RemoveHandler _view.ItemSelected, AddressOf OnItemSelected
                    RemoveHandler _view.DismissRequested, AddressOf OnDismissRequested
                End If
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.SearchQueryChanged, AddressOf OnSearchQueryChanged
                    AddHandler _view.ItemSelected, AddressOf OnItemSelected
                    AddHandler _view.DismissRequested, AddressOf OnDismissRequested

                    If _themeService IsNot Nothing Then
                        _view.ApplyTheme(_themeService.Tokens)
                    End If
                    LoadInitialData()
                End If
            End Set
        End Property

        Private Sub LoadInitialData()
            If _view Is Nothing Then Return

            Dim results = _mainWindowPresenter.AllNavigableItems.
                Select(Function(i) New CommandPaletteItem With {
                    .Title = i.Name,
                    .Category = "Screens",
                    .Kind = CommandPaletteItemKind.Screen,
                    .NavigationItem = i
                }).ToList()

            _view.SetResults(results)
            _view.IsEmpty = Not results.Any()
        End Sub

        Private Async Sub OnSearchQueryChanged(sender As Object, query As String)
            If _cts IsNot Nothing Then
                _cts.Cancel()
                _cts.Dispose()
            End If

            _cts = New CancellationTokenSource()
            Dim token = _cts.Token

            If String.IsNullOrWhiteSpace(query) Then
                LoadInitialData()
                Return
            End If

            Try
                _view.IsBusy = True

                Await Task.Delay(250, token)

                If token.IsCancellationRequested Then Return

                Dim results = New List(Of CommandPaletteItem)()

                ' Sync screen search
                Dim screens = _mainWindowPresenter.AllNavigableItems.
                    Where(Function(i) i.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).
                    Select(Function(i) New CommandPaletteItem With {
                        .Title = i.Name,
                        .Category = "Screens",
                        .Kind = CommandPaletteItemKind.Screen,
                        .NavigationItem = i
                    })
                results.AddRange(screens)

                ' Async product search
                Dim productsQuery = New GetProductsForCatalogQuery()
                Dim allProducts = Await _mediator.Send(productsQuery, token)

                Dim products = allProducts.
                    Where(Function(p) p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).
                    Take(5).
                    Select(Function(p) New CommandPaletteItem With {
                        .Title = p.Name,
                        .Subtitle = $"Price: {p.RetailPrice:C2}",
                        .Category = "Products",
                        .Kind = CommandPaletteItemKind.Product,
                        .ProductId = p.Id
                    })

                results.AddRange(products)

                If Not token.IsCancellationRequested Then
                    _view.SetResults(results)
                    _view.IsBusy = False
                    _view.IsEmpty = Not results.Any()
                End If
            Catch ex As OperationCanceledException
                ' Ignore
            Finally
                If Not token.IsCancellationRequested Then
                    _view.IsBusy = False
                End If
            End Try
        End Sub

        Private Sub OnItemSelected(sender As Object, item As CommandPaletteItem)
            If item.Kind = CommandPaletteItemKind.Screen Then
                _mainWindowPresenter.NavigateTo(item.NavigationItem)
            End If
            ' TODO: Handle product selection if needed

            _view.HidePalette()
        End Sub

        Private Sub OnDismissRequested(sender As Object, e As EventArgs)
            _view.HidePalette()
        End Sub

        Private Sub OnThemeChanged(sender As Object, appTheme As AppTheme)
            If _view IsNot Nothing AndAlso _themeService IsNot Nothing Then
                _view.ApplyTheme(_themeService.Tokens)
            End If
        End Sub

        Private _disposed As Boolean
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposed Then
                If disposing Then
                    If _themeService IsNot Nothing Then
                        RemoveHandler _themeService.ThemeChanged, AddressOf OnThemeChanged
                    End If
                    View = Nothing
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
