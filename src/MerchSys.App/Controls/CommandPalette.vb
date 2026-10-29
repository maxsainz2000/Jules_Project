Imports System.Windows.Forms
Imports System.Drawing
Imports MerchSys.App.Models
Imports MerchSys.App.Views
Imports MerchSys.App.Services.Theming

Namespace Controls
    Public Class CommandPalette
        Inherits UserControl
        Implements ICommandPaletteView

        Public Event SearchQueryChanged As EventHandler(Of String) Implements ICommandPaletteView.SearchQueryChanged
        Public Event ItemSelected As EventHandler(Of CommandPaletteItem) Implements ICommandPaletteView.ItemSelected
        Public Event DismissRequested As EventHandler Implements ICommandPaletteView.DismissRequested

        Public Sub New()
            InitializeComponent()

            AddHandler pnlBackdrop.Click, AddressOf pnlBackdrop_Click
            AddHandler txtSearch.TextChanged, AddressOf txtSearch_TextChanged
            AddHandler txtSearch.KeyDown, AddressOf txtSearch_KeyDown
            AddHandler txtSearch.GotFocus, AddressOf txtSearch_GotFocus
            AddHandler txtSearch.LostFocus, AddressOf txtSearch_LostFocus
            AddHandler lstResults.KeyDown, AddressOf lstResults_KeyDown
            AddHandler lstResults.DoubleClick, AddressOf lstResults_DoubleClick
        End Sub

        Protected Overrides Sub OnResize(e As EventArgs)
            MyBase.OnResize(e)
            If pnlCard IsNot Nothing Then
                pnlCard.Left = (Me.Width - pnlCard.Width) \ 2
                pnlCard.Top = Math.Max(20, (Me.Height - pnlCard.Height) \ 4)
            End If
        End Sub

        Protected Overrides Sub OnVisibleChanged(e As EventArgs)
            MyBase.OnVisibleChanged(e)
            If Me.Visible Then
                SetPlaceholderText()
                txtSearch.Focus()
            End If
        End Sub

        Private Sub SetPlaceholderText()
            If String.IsNullOrEmpty(txtSearch.Text) Then
                txtSearch.Text = "Type a command or search products..."
                txtSearch.ForeColor = SystemColors.GrayText
            End If
        End Sub

        Private Sub ClearPlaceholderText()
            If txtSearch.Text = "Type a command or search products..." Then
                txtSearch.Text = ""
                txtSearch.ForeColor = SystemColors.WindowText
            End If
        End Sub

        Private Sub txtSearch_GotFocus(sender As Object, e As EventArgs)
            ClearPlaceholderText()
        End Sub

        Private Sub txtSearch_LostFocus(sender As Object, e As EventArgs)
            SetPlaceholderText()
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsBusy As Boolean Implements ICommandPaletteView.IsBusy
            Get
                Return busyOverlay.IsBusy
            End Get
            Set(value As Boolean)
                If InvokeRequired Then
                    Invoke(Sub() IsBusy = value)
                    Return
                End If

                busyOverlay.IsBusy = value
                busyOverlay.Visible = value
                If value Then
                    busyOverlay.BringToFront()
                End If
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsEmpty As Boolean Implements ICommandPaletteView.IsEmpty
            Get
                Return emptyStatePanel.Visible
            End Get
            Set(value As Boolean)
                If InvokeRequired Then
                    Invoke(Sub() IsEmpty = value)
                    Return
                End If

                emptyStatePanel.Visible = value
                If value Then
                    emptyStatePanel.Title = "No results found"
                    emptyStatePanel.Description = "No matching screens or products found."
                    emptyStatePanel.BringToFront()
                End If
            End Set
        End Property

        Public Sub SetResults(results As IEnumerable(Of CommandPaletteItem)) Implements ICommandPaletteView.SetResults
            If InvokeRequired Then
                Invoke(Sub() SetResults(results))
                Return
            End If

            lstResults.Items.Clear()
            If results IsNot Nothing Then
                For Each item In results
                    lstResults.Items.Add(item)
                Next
            End If

            If lstResults.Items.Count > 0 Then
                lstResults.SelectedIndex = 0
            End If
        End Sub

        Public Sub ShowPalette() Implements ICommandPaletteView.ShowPalette
            Me.Visible = True
            Me.BringToFront()
            txtSearch.Text = ""
            txtSearch.Focus()
        End Sub

        Public Sub HidePalette() Implements ICommandPaletteView.HidePalette
            Me.Visible = False
            txtSearch.Text = ""
        End Sub

        Public Sub ApplyTheme(tokens As ThemeTokens) Implements ICommandPaletteView.ApplyTheme
            If tokens IsNot Nothing Then
                pnlBackdrop.BackColor = Color.FromArgb(128, tokens.SurfaceBackground.R, tokens.SurfaceBackground.G, tokens.SurfaceBackground.B)
                pnlCard.BackColor = tokens.CardBackground
                txtSearch.BackColor = tokens.SurfaceBackground
                txtSearch.ForeColor = tokens.PrimaryText
                lstResults.BackColor = tokens.CardBackground
                lstResults.ForeColor = tokens.PrimaryText

                busyOverlay.ApplyTheme(tokens)
                emptyStatePanel.ApplyTheme(tokens)
            End If
        End Sub

        Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs)
            If txtSearch.Text <> "Type a command or search products..." Then
                RaiseEvent SearchQueryChanged(Me, txtSearch.Text)
            End If
        End Sub

        Private Sub txtSearch_KeyDown(sender As Object, e As KeyEventArgs)
            If e.KeyCode = Keys.Escape Then
                RaiseEvent DismissRequested(Me, EventArgs.Empty)
                e.Handled = True
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.Down Then
                If lstResults.Items.Count > 0 Then
                    lstResults.Focus()
                    If lstResults.SelectedIndex < lstResults.Items.Count - 1 Then
                        lstResults.SelectedIndex += 1
                    End If
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.Up Then
                If lstResults.Items.Count > 0 Then
                    lstResults.Focus()
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.Enter Then
                If lstResults.SelectedItem IsNot Nothing Then
                    RaiseEvent ItemSelected(Me, DirectCast(lstResults.SelectedItem, CommandPaletteItem))
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End Sub

        Private Sub lstResults_KeyDown(sender As Object, e As KeyEventArgs)
            If e.KeyCode = Keys.Escape Then
                RaiseEvent DismissRequested(Me, EventArgs.Empty)
                e.Handled = True
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.Enter Then
                If lstResults.SelectedItem IsNot Nothing Then
                    RaiseEvent ItemSelected(Me, DirectCast(lstResults.SelectedItem, CommandPaletteItem))
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.Up AndAlso lstResults.SelectedIndex <= 0 Then
                txtSearch.Focus()
                e.Handled = True
            End If
        End Sub

        Private Sub lstResults_DoubleClick(sender As Object, e As EventArgs)
            If lstResults.SelectedItem IsNot Nothing Then
                RaiseEvent ItemSelected(Me, DirectCast(lstResults.SelectedItem, CommandPaletteItem))
            End If
        End Sub

        Private Sub pnlBackdrop_Click(sender As Object, e As EventArgs)
            RaiseEvent DismissRequested(Me, EventArgs.Empty)
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
