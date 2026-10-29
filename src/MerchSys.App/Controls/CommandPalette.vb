Imports System.Drawing
Imports System.Windows.Forms
Imports MerchSys.App.Models
Imports MerchSys.App.Views
Imports MerchSys.App.Services.Theming

Namespace Controls
    Public Class CommandPalette
        Implements ICommandPaletteView

        Public Event SearchQueryChanged As EventHandler(Of String) Implements ICommandPaletteView.SearchQueryChanged
        Public Event ItemSelected As EventHandler(Of CommandPaletteItem) Implements ICommandPaletteView.ItemSelected
        Public Event DismissRequested As EventHandler Implements ICommandPaletteView.DismissRequested

        Private _themeTokens As ThemeTokens
        Private _isBusy As Boolean
        Private _isEmpty As Boolean

        Public Sub New()
            InitializeComponent()

            ' Placeholder setup
            txtSearch.Text = "Type a command or search products..."
            txtSearch.ForeColor = Color.Gray

            AddHandler txtSearch.GotFocus, AddressOf txtSearch_GotFocus
            AddHandler txtSearch.LostFocus, AddressOf txtSearch_LostFocus
            AddHandler txtSearch.TextChanged, AddressOf txtSearch_TextChanged
            AddHandler txtSearch.KeyDown, AddressOf txtSearch_KeyDown
            AddHandler lstResults.KeyDown, AddressOf lstResults_KeyDown
            AddHandler lstResults.DoubleClick, AddressOf lstResults_DoubleClick
            AddHandler Me.Click, AddressOf Backdrop_Click

            lstResults.DrawMode = DrawMode.OwnerDrawFixed
            AddHandler lstResults.DrawItem, AddressOf lstResults_DrawItem
        End Sub

        Public Sub SetResults(results As IEnumerable(Of CommandPaletteItem)) Implements ICommandPaletteView.SetResults
            SafeInvoke(Sub()
                lstResults.Items.Clear()
                If results IsNot Nothing Then
                    For Each item In results
                        lstResults.Items.Add(item)
                    Next
                End If
                If lstResults.Items.Count > 0 Then
                    lstResults.SelectedIndex = 0
                End If
            End Sub)
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsBusy As Boolean Implements ICommandPaletteView.IsBusy
            Get
                Return _isBusy
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub()
                    _isBusy = value
                    busyOverlay.Visible = value
                    If value Then
                        busyOverlay.BringToFront()
                    End If
                End Sub)
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsEmpty As Boolean Implements ICommandPaletteView.IsEmpty
            Get
                Return _isEmpty
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub()
                    _isEmpty = value
                    emptyStatePanel.Visible = value
                    If value Then
                        emptyStatePanel.BringToFront()
                        ' Directly access the underlying label to set custom text since EmptyStatePanel might not expose it
                        ' Or if it exposes properties like Title/Description we'd use them,
                        ' but modifying child controls via properties is better if available,
                        ' here we assume Title/Description properties exist or just use default text.
                        emptyStatePanel.lblTitle.Text = "No results found"
                        emptyStatePanel.lblDescription.Text = "No matching screens or products found."
                    End If
                End Sub)
            End Set
        End Property

        Public Sub ShowPalette() Implements ICommandPaletteView.ShowPalette
            SafeInvoke(Sub()
                Me.Visible = True
                Me.BringToFront()
                txtSearch.Focus()
                txtSearch.SelectAll()
            End Sub)
        End Sub

        Public Sub HidePalette() Implements ICommandPaletteView.HidePalette
            SafeInvoke(Sub()
                Me.Visible = False
            End Sub)
        End Sub

        Public Sub ApplyTheme(tokens As ThemeTokens) Implements ICommandPaletteView.ApplyTheme
            _themeTokens = tokens
            SafeInvoke(Sub()
                pnlCard.BackColor = tokens.CardBackground
                txtSearch.BackColor = tokens.SurfaceBackground
                txtSearch.ForeColor = tokens.PrimaryText
                lstResults.BackColor = tokens.SurfaceBackground
                lstResults.ForeColor = tokens.PrimaryText

                ' Apply theme to embedded controls via reflection or directly if methods exist
                ' If ApplyTheme method exists on EmptyStatePanel / BusyOverlay
                ' TryCast isn't working if methods don't exist on the control, we can set properties directly
                emptyStatePanel.BackColor = tokens.SurfaceBackground
                emptyStatePanel.lblTitle.ForeColor = tokens.PrimaryText
                emptyStatePanel.lblDescription.ForeColor = tokens.SecondaryText

                busyOverlay.BackColor = tokens.SurfaceBackground
                busyOverlay.lblMessage.ForeColor = tokens.PrimaryText
            End Sub)
        End Sub

        Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
            If keyData = Keys.Escape Then
                RaiseEvent DismissRequested(Me, EventArgs.Empty)
                Return True
            End If
            Return MyBase.ProcessCmdKey(msg, keyData)
        End Function

        Private Sub txtSearch_GotFocus(sender As Object, e As EventArgs)
            If txtSearch.Text = "Type a command or search products..." Then
                txtSearch.Text = ""
                txtSearch.ForeColor = If(_themeTokens IsNot Nothing, _themeTokens.PrimaryText, Color.Black)
            End If
        End Sub

        Private Sub txtSearch_LostFocus(sender As Object, e As EventArgs)
            If String.IsNullOrWhiteSpace(txtSearch.Text) Then
                txtSearch.Text = "Type a command or search products..."
                txtSearch.ForeColor = Color.Gray
            End If
        End Sub

        Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs)
            If txtSearch.Text <> "Type a command or search products..." AndAlso txtSearch.Focused Then
                RaiseEvent SearchQueryChanged(Me, txtSearch.Text)
            End If
        End Sub

        Private Sub txtSearch_KeyDown(sender As Object, e As KeyEventArgs)
            If e.KeyCode = Keys.Down Then
                If lstResults.Items.Count > 0 Then
                    lstResults.Focus()
                    If lstResults.SelectedIndex < 0 Then
                        lstResults.SelectedIndex = 0
                    End If
                End If
                e.Handled = True
            ElseIf e.KeyCode = Keys.Enter Then
                e.Handled = True
                e.SuppressKeyPress = True
                If lstResults.SelectedItem IsNot Nothing Then
                    RaiseEvent ItemSelected(Me, DirectCast(lstResults.SelectedItem, CommandPaletteItem))
                End If
            End If
        End Sub

        Private Sub lstResults_KeyDown(sender As Object, e As KeyEventArgs)
            If e.KeyCode = Keys.Up AndAlso lstResults.SelectedIndex = 0 Then
                txtSearch.Focus()
                e.Handled = True
            ElseIf e.KeyCode = Keys.Enter Then
                e.Handled = True
                e.SuppressKeyPress = True
                If lstResults.SelectedItem IsNot Nothing Then
                    RaiseEvent ItemSelected(Me, DirectCast(lstResults.SelectedItem, CommandPaletteItem))
                End If
            End If
        End Sub

        Private Sub lstResults_DoubleClick(sender As Object, e As EventArgs)
            If lstResults.SelectedItem IsNot Nothing Then
                RaiseEvent ItemSelected(Me, DirectCast(lstResults.SelectedItem, CommandPaletteItem))
            End If
        End Sub

        Private Sub Backdrop_Click(sender As Object, e As EventArgs)
            RaiseEvent DismissRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub lstResults_DrawItem(sender As Object, e As DrawItemEventArgs)
            If e.Index < 0 Then Return

            e.DrawBackground()

            Dim item As CommandPaletteItem = DirectCast(lstResults.Items(e.Index), CommandPaletteItem)

            Dim isSelected = (e.State And DrawItemState.Selected) = DrawItemState.Selected
            Dim textColor = If(isSelected, Color.White, If(_themeTokens IsNot Nothing, _themeTokens.PrimaryText, Color.Black))

            If isSelected AndAlso _themeTokens IsNot Nothing Then
                e.Graphics.FillRectangle(New SolidBrush(_themeTokens.Accent), e.Bounds)
            End If

            Dim titleFont As New Font(e.Font, FontStyle.Bold)
            Dim subFont As New Font(e.Font.FontFamily, e.Font.Size - 1, FontStyle.Regular)

            e.Graphics.DrawString(item.Title, titleFont, New SolidBrush(textColor), e.Bounds.Left + 5, e.Bounds.Top + 2)
            e.Graphics.DrawString($"{item.Category} • {item.Subtitle}", subFont, New SolidBrush(textColor), e.Bounds.Left + 5, e.Bounds.Top + 20)

            e.DrawFocusRectangle()
        End Sub

        Private Sub SafeInvoke(action As Action)
            If Me.InvokeRequired Then
                Me.Invoke(action)
            Else
                action()
            End If
        End Sub

    End Class
End Namespace
