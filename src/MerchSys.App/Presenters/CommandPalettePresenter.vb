Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports MerchSys.App.Models
Imports MerchSys.App.Services.Theming
Imports MerchSys.App.Views
Imports MerchSys.SharedKernel.Queries
Imports MerchSys.App.Controls

Namespace Presenters
    Public Class CommandPalettePresenter
        Implements IDisposable

        Private ReadOnly _mainWindowPresenter As MainWindowPresenter
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _themeService As IThemeService

        Private _cts As CancellationTokenSource
        Private _disposed As Boolean

        Public Property View As ICommandPaletteView

        Public Sub New(paletteView As ICommandPaletteView,
                       mainWindowPresenter As MainWindowPresenter,
                       mediator As IMediator,
                       themeService As IThemeService)
            Me.View = paletteView
            _mainWindowPresenter = mainWindowPresenter
            _mediator = mediator
            _themeService = themeService

            AddHandler Me.View.SearchQueryChanged, AddressOf OnSearchQueryChanged
            AddHandler Me.View.ItemSelected, AddressOf OnItemSelected
            AddHandler Me.View.DismissRequested, AddressOf OnDismissRequested

            If _themeService IsNot Nothing AndAlso TypeOf Me.View Is CommandPalette Then
                Dim cmdPalette = DirectCast(Me.View, CommandPalette)
                cmdPalette.ApplyTheme(_themeService.Tokens)
                AddHandler _themeService.ThemeChanged, AddressOf OnThemeChanged
            End If
        End Sub

        Public Sub New(paletteView As ICommandPaletteView,
                       mainWindowPresenter As MainWindowPresenter,
                       mediator As IMediator)
            Me.New(paletteView, mainWindowPresenter, mediator, Nothing)
        End Sub

        Private Sub OnThemeChanged(sender As Object, theme As AppTheme)
            If _themeService IsNot Nothing AndAlso TypeOf Me.View Is CommandPalette Then
                Dim cmdPalette = DirectCast(Me.View, CommandPalette)
                cmdPalette.ApplyTheme(_themeService.Tokens)
            End If
        End Sub

        Private Async Sub OnSearchQueryChanged(sender As Object, query As String)
            If _cts IsNot Nothing Then
                _cts.Cancel()
                _cts.Dispose()
            End If

            _cts = New CancellationTokenSource()
            Dim token = _cts.Token

            If String.IsNullOrWhiteSpace(query) Then
                Me.View.SetResults(Enumerable.Empty(Of CommandPaletteItem)())
                Me.View.IsEmpty = False
                Me.View.IsBusy = False
                Return
            End If

            ' Sync screens filtering
            Dim screenResults = _mainWindowPresenter.AllNavigableItems.
                Where(Function(n) n.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 OrElse n.NavigationGroup.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).
                Select(Function(n) New CommandPaletteItem With {
                    .Title = n.Name,
                    .Category = "Screens",
                    .Kind = CommandPaletteItemKind.Screen,
                    .NavigationItem = n,
                    .Subtitle = n.NavigationGroup
                }).ToList()

            Me.View.SetResults(screenResults)

            Me.View.IsBusy = True
            Try
                Await Task.Delay(250, token)
                Dim productList = Await _mediator.Send(New GetProductsForCatalogQuery(), token)
                Dim matchingProducts = productList.
                    Where(Function(p) p.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).
                    Take(5).
                    Select(Function(p) New CommandPaletteItem With {
                        .Title = p.Name,
                        .Subtitle = $"₱{p.RetailPrice:N2}",
                        .Category = "Products",
                        .Kind = CommandPaletteItemKind.Product,
                        .ProductId = p.Id
                    }).ToList()

                ' Combine screens and products
                Dim allResults = screenResults.Concat(matchingProducts).ToList()
                Me.View.SetResults(allResults)
                Me.View.IsEmpty = (allResults.Count = 0)
            Catch ex As OperationCanceledException
                ' Search superseded by newer keystroke
            Finally
                Me.View.IsBusy = False
            End Try
        End Sub

        Private Sub OnItemSelected(sender As Object, item As CommandPaletteItem)
            If item.Kind = CommandPaletteItemKind.Screen AndAlso item.NavigationItem IsNot Nothing Then
                _mainWindowPresenter.NavigateTo(item.NavigationItem)
            ElseIf item.Kind = CommandPaletteItemKind.Product Then
                ' Not explicitly specified where to navigate for product, so let's try to navigate to Stock Dashboard or similar
                ' For now, just dismiss, assuming product selection navigation might be a future enhancement
            End If
            Me.View.HidePalette()
        End Sub

        Private Sub OnDismissRequested(sender As Object, e As EventArgs)
            Me.View.HidePalette()
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposed Then
                If disposing Then
                    RemoveHandler Me.View.SearchQueryChanged, AddressOf OnSearchQueryChanged
                    RemoveHandler Me.View.ItemSelected, AddressOf OnItemSelected
                    RemoveHandler Me.View.DismissRequested, AddressOf OnDismissRequested

                    If _themeService IsNot Nothing Then
                        RemoveHandler _themeService.ThemeChanged, AddressOf OnThemeChanged
                    End If

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
