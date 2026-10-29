Imports System.Windows.Forms
Imports MerchSys.App.Models
Imports MerchSys.App.Views

Namespace Controls
    Public Class CommandPalette
        Implements ICommandPaletteView

        Public Event SearchQueryChanged As EventHandler(Of String) Implements ICommandPaletteView.SearchQueryChanged
        Public Event ItemSelected As EventHandler(Of CommandPaletteItem) Implements ICommandPaletteView.ItemSelected
        Public Event DismissRequested As EventHandler Implements ICommandPaletteView.DismissRequested

        Public Sub New()
            InitializeComponent()

            txtSearch.PlaceholderText = "Type a command or search products..."
            lstResults.DisplayMember = "ToString"

            AddHandler txtSearch.TextChanged, AddressOf OnSearchTextChanged
            AddHandler txtSearch.KeyDown, AddressOf OnSearchKeyDown
            AddHandler lstResults.KeyDown, AddressOf OnListKeyDown
            AddHandler lstResults.MouseDoubleClick, AddressOf OnListDoubleClick
            AddHandler pnlBackdrop.Click, AddressOf OnBackdropClick
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsBusy As Boolean Implements ICommandPaletteView.IsBusy
            Get
                Return pnlBusy.Visible
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub()
                               pnlBusy.Visible = value
                               If value Then pnlBusy.BringToFront()
                           End Sub)
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsEmpty As Boolean Implements ICommandPaletteView.IsEmpty
            Get
                Return pnlEmpty.Visible
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub()
                               pnlEmpty.Visible = value
                               If value Then pnlEmpty.BringToFront()
                           End Sub)
            End Set
        End Property

        Public Sub SetResults(results As IEnumerable(Of CommandPaletteItem)) Implements ICommandPaletteView.SetResults
            SafeInvoke(Sub()
                           lstResults.DataSource = results.ToList()
                           If Not IsBusy AndAlso Not IsEmpty Then
                               lstResults.BringToFront()
                           End If
                       End Sub)
        End Sub

        Public Sub ShowPalette() Implements ICommandPaletteView.ShowPalette
            Me.Visible = True
            Me.BringToFront()
            txtSearch.Text = String.Empty
            txtSearch.Focus()
        End Sub

        Public Sub HidePalette() Implements ICommandPaletteView.HidePalette
            Me.Visible = False
        End Sub

        Private Sub OnSearchTextChanged(sender As Object, e As EventArgs)
            RaiseEvent SearchQueryChanged(Me, txtSearch.Text)
        End Sub

        Private Sub OnSearchKeyDown(sender As Object, e As KeyEventArgs)
            If e.KeyCode = Keys.Escape Then
                RaiseEvent DismissRequested(Me, EventArgs.Empty)
                e.Handled = True
            ElseIf e.KeyCode = Keys.Down Then
                If lstResults.Items.Count > 0 Then
                    lstResults.Focus()
                    lstResults.SelectedIndex = 0
                End If
                e.Handled = True
            ElseIf e.KeyCode = Keys.Enter Then
                e.Handled = True
                e.SuppressKeyPress = True
                If lstResults.Items.Count > 0 AndAlso lstResults.SelectedIndex >= 0 Then
                    RaiseEvent ItemSelected(Me, DirectCast(lstResults.SelectedItem, CommandPaletteItem))
                End If
            End If
        End Sub

        Private Sub OnListKeyDown(sender As Object, e As KeyEventArgs)
            If e.KeyCode = Keys.Escape Then
                RaiseEvent DismissRequested(Me, EventArgs.Empty)
                e.Handled = True
            ElseIf e.KeyCode = Keys.Enter Then
                If lstResults.SelectedIndex >= 0 Then
                    RaiseEvent ItemSelected(Me, DirectCast(lstResults.SelectedItem, CommandPaletteItem))
                End If
                e.Handled = True
            ElseIf e.KeyCode = Keys.Up AndAlso lstResults.SelectedIndex <= 0 Then
                txtSearch.Focus()
                e.Handled = True
            End If
        End Sub

        Private Sub OnListDoubleClick(sender As Object, e As MouseEventArgs)
            If lstResults.SelectedIndex >= 0 Then
                RaiseEvent ItemSelected(Me, DirectCast(lstResults.SelectedItem, CommandPaletteItem))
            End If
        End Sub

        Private Sub OnBackdropClick(sender As Object, e As EventArgs)
            RaiseEvent DismissRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub SafeInvoke(action As Action)
            If Me.InvokeRequired Then
                Me.Invoke(action)
            Else
                action()
            End If
        End Sub

        Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
            If keyData = Keys.Escape Then
                RaiseEvent DismissRequested(Me, EventArgs.Empty)
                Return True
            End If
            Return MyBase.ProcessCmdKey(msg, keyData)
        End Function
    End Class
End Namespace
