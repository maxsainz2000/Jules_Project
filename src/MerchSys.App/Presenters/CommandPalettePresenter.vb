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

        Private _debounceTimer As Timer
        Private _currentCts As CancellationTokenSource

        Public Sub New(paletteView As ICommandPaletteView,
                       mainWindowPresenter As MainWindowPresenter,
                       mediator As IMediator,
                       Optional themeService As IThemeService = Nothing)
            _mainWindowPresenter = mainWindowPresenter
            _mediator = mediator
            _themeService = themeService

            If _themeService IsNot Nothing Then
                AddHandler _themeService.ThemeChanged, AddressOf OnThemeChanged
            End If

            Me.View = paletteView
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
                End If
            End Set
        End Property

        Private Sub OnThemeChanged(sender As Object, theme As AppTheme)
            If _view IsNot Nothing AndAlso _themeService IsNot Nothing Then
                _view.ApplyTheme(_themeService.Tokens)
            End If
        End Sub

        Private Sub OnDismissRequested(sender As Object, e As EventArgs)
            If _view IsNot Nothing Then
                _view.HidePalette()
            End If
        End Sub

        Private Sub OnItemSelected(sender As Object, item As CommandPaletteItem)
            If _view IsNot Nothing Then
                _view.HidePalette()
            End If

            If item.Kind = CommandPaletteItemKind.Screen AndAlso item.NavigationItem IsNot Nothing Then
                _mainWindowPresenter.NavigateTo(item.NavigationItem)
            End If
            ' Product navigation is left out of this UX-16 scope for now but placeholder is here
        End Sub

        Private Sub OnSearchQueryChanged(sender As Object, query As String)
            If _currentCts IsNot Nothing Then
                _currentCts.Cancel()
                _currentCts.Dispose()
                _currentCts = Nothing
            End If

            If _debounceTimer IsNot Nothing Then
                _debounceTimer.Dispose()
            End If

            If String.IsNullOrWhiteSpace(query) Then
                If _view IsNot Nothing Then
                    _view.IsBusy = False
                    _view.IsEmpty = False
                    _view.SetResults(New List(Of CommandPaletteItem)())
                End If
                Return
            End If

            If _view IsNot Nothing Then
                _view.IsBusy = True
            End If

            _currentCts = New CancellationTokenSource()
            Dim token = _currentCts.Token

            _debounceTimer = New Timer(Sub(state)
                                           If Not token.IsCancellationRequested Then
                                               Dim t = PerformSearchAsync(query, token)
                                           End If
                                       End Sub, Nothing, 250, Timeout.Infinite)
        End Sub

        Private Async Function PerformSearchAsync(query As String, token As CancellationToken) As Task
            Dim results As New List(Of CommandPaletteItem)()
            Dim lowerQuery = query.ToLowerInvariant()

            ' 1. Synchronous Screen Search
            Dim screens = _mainWindowPresenter.AllNavigableItems.
                Where(Function(i) i.Name.ToLowerInvariant().Contains(lowerQuery)).
                Select(Function(i) New CommandPaletteItem With {
                    .Title = i.Name,
                    .Subtitle = "Screen",
                    .Category = "Screens",
                    .Kind = CommandPaletteItemKind.Screen,
                    .NavigationItem = i
                })
            results.AddRange(screens)

            ' 2. Asynchronous Product Search
            Try
                Dim products = Await _mediator.Send(New GetProductsForCatalogQuery(), token)
                Dim filteredProducts = products.
                    Where(Function(p) p.Name.ToLowerInvariant().Contains(lowerQuery)).
                    Take(5).
                    Select(Function(p) New CommandPaletteItem With {
                        .Title = p.Name,
                        .Subtitle = $"Price: {p.RetailPrice:C}",
                        .Category = "Products",
                        .Kind = CommandPaletteItemKind.Product,
                        .ProductId = p.Id
                    })
                results.AddRange(filteredProducts)
            Catch ex As OperationCanceledException
                Return
            Catch ex As Exception
                ' Silently ignore errors during search UX
            End Try

            If token.IsCancellationRequested Then Return

            If _view IsNot Nothing Then
                Dim viewResults = results.ToList()
                _view.SetResults(viewResults)
                _view.IsBusy = False
                _view.IsEmpty = viewResults.Count = 0
            End If
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Me.View = Nothing

            If _themeService IsNot Nothing Then
                RemoveHandler _themeService.ThemeChanged, AddressOf OnThemeChanged
            End If

            If _debounceTimer IsNot Nothing Then
                _debounceTimer.Dispose()
                _debounceTimer = Nothing
            End If
            If _currentCts IsNot Nothing Then
                _currentCts.Cancel()
                _currentCts.Dispose()
                _currentCts = Nothing
            End If
        End Sub
    End Class
End Namespace
