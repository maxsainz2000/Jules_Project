Imports System.IO
Imports System.Text.Json
Imports System.Windows.Forms

Namespace Services.Theming
    Public Class ThemeService
        Implements IThemeService

        Private _currentTheme As AppTheme
        Private _tokens As ThemeTokens
        Private ReadOnly _themeFilePath As String

        Public Event ThemeChanged As EventHandler(Of AppTheme) Implements IThemeService.ThemeChanged

        Public Sub New(Optional themeFilePath As String = Nothing)
            If String.IsNullOrEmpty(themeFilePath) Then
                Dim appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VISTA")
                If Not Directory.Exists(appDataFolder) Then
                    Directory.CreateDirectory(appDataFolder)
                End If
                _themeFilePath = Path.Combine(appDataFolder, "theme.json")
            Else
                _themeFilePath = themeFilePath
            End If

            LoadTheme()
        End Sub

        Public ReadOnly Property CurrentTheme As AppTheme Implements IThemeService.CurrentTheme
            Get
                Return _currentTheme
            End Get
        End Property

        Public ReadOnly Property Tokens As ThemeTokens Implements IThemeService.Tokens
            Get
                Return _tokens
            End Get
        End Property

        Public Sub SetTheme(theme As AppTheme) Implements IThemeService.SetTheme
            If _currentTheme <> theme OrElse _tokens Is Nothing Then
                _currentTheme = theme
                _tokens = ThemeTokens.GetTokens(theme)
                SaveTheme()
                RaiseEvent ThemeChanged(Me, _currentTheme)
            End If
        End Sub

        Public Sub ToggleTheme() Implements IThemeService.ToggleTheme
            If _currentTheme = AppTheme.Light Then
                SetTheme(AppTheme.Dark)
            Else
                SetTheme(AppTheme.Light)
            End If
        End Sub

        Public Sub ApplyTheme(root As Control) Implements IThemeService.ApplyTheme
            If root Is Nothing Then Return

            ' Apply styles based on control type
            If TypeOf root Is Form Then
                root.BackColor = _tokens.WindowBackground
                root.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf root Is Panel OrElse TypeOf root Is UserControl OrElse TypeOf root Is SplitContainer OrElse TypeOf root Is TabPage Then
                If TypeOf root Is Panel AndAlso root.Tag IsNot Nothing AndAlso root.Tag.ToString() = "Card" Then
                    ComponentStyler.ApplyCard(DirectCast(root, Panel), _tokens)
                Else
                    root.BackColor = _tokens.SurfaceBackground
                    root.ForeColor = _tokens.PrimaryText
                End If
            ElseIf TypeOf root Is GroupBox Then
                Dim grp = DirectCast(root, GroupBox)
                grp.FlatStyle = FlatStyle.Flat
                grp.BackColor = _tokens.SurfaceBackground
                grp.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf root Is TabControl Then
                Dim tc = DirectCast(root, TabControl)
                tc.Appearance = TabAppearance.Normal
            ElseIf TypeOf root Is Button Then
                Dim btn = DirectCast(root, Button)
                Dim tagStr = If(btn.Tag IsNot Nothing, btn.Tag.ToString(), String.Empty)

                If tagStr = "Primary" OrElse tagStr = "Accent" OrElse
                   btn.Name.Contains("Primary") OrElse btn.Name.Contains("Save") OrElse
                   btn.Name.Contains("Submit") OrElse btn.Name.Contains("Confirm") Then
                    ComponentStyler.ApplyPrimaryButton(btn, _tokens)
                ElseIf tagStr = "Success" Then
                    ComponentStyler.ApplySuccessButton(btn, _tokens)
                ElseIf tagStr = "Danger" OrElse
                       btn.Name.Contains("Delete") OrElse btn.Name.Contains("Cancel") OrElse
                       btn.Name.Contains("Logout") Then
                    ComponentStyler.ApplyDangerButton(btn, _tokens)
                ElseIf tagStr = "Warning" Then
                    ComponentStyler.ApplyWarningButton(btn, _tokens)
                ElseIf tagStr = "Secondary" Then
                    ComponentStyler.ApplySecondaryButton(btn, _tokens)
                ElseIf tagStr = "Subtle" OrElse tagStr = "Link" Then
                    ComponentStyler.ApplySubtleButton(btn, _tokens)
                Else
                    ' Default button fallback is similar to secondary but follows old logic closely
                    ComponentStyler.ApplySecondaryButton(btn, _tokens)
                End If
            ElseIf TypeOf root Is Label Then
                Dim lbl = DirectCast(root, Label)
                Dim tagStr = If(lbl.Tag IsNot Nothing, lbl.Tag.ToString(), String.Empty)

                If tagStr = "PageTitle" Then
                    ComponentStyler.ApplyPageTitle(lbl, _tokens)
                    lbl.BackColor = System.Drawing.Color.Transparent
                ElseIf tagStr = "SectionHeader" Then
                    ComponentStyler.ApplySectionHeader(lbl, _tokens)
                    lbl.BackColor = System.Drawing.Color.Transparent
                ElseIf tagStr = "Subtitle" Then
                    ComponentStyler.ApplySubtitle(lbl, _tokens)
                    lbl.BackColor = System.Drawing.Color.Transparent
                ElseIf tagStr = "Caption" Then
                    ComponentStyler.ApplyCaption(lbl, _tokens)
                    lbl.BackColor = System.Drawing.Color.Transparent
                ElseIf tagStr = "MetricValue" Then
                    ComponentStyler.ApplyMetricValue(lbl, _tokens)
                    lbl.BackColor = System.Drawing.Color.Transparent
                Else
                    lbl.BackColor = System.Drawing.Color.Transparent
                    lbl.ForeColor = _tokens.PrimaryText
                End If
            ElseIf TypeOf root Is TextBoxBase Then
                Dim txt = DirectCast(root, TextBoxBase)
                txt.BackColor = _tokens.CardBackground
                txt.ForeColor = _tokens.PrimaryText
                txt.BorderStyle = BorderStyle.FixedSingle
            ElseIf TypeOf root Is ComboBox Then
                Dim cmb = DirectCast(root, ComboBox)
                cmb.FlatStyle = FlatStyle.Flat
                cmb.BackColor = _tokens.CardBackground
                cmb.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf root Is CheckBox OrElse TypeOf root Is RadioButton Then
                Dim btn = DirectCast(root, ButtonBase)
                btn.FlatStyle = FlatStyle.Flat
                btn.BackColor = System.Drawing.Color.Transparent
                btn.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf root Is ListBox OrElse TypeOf root Is ListView Then
                root.BackColor = _tokens.CardBackground
                root.ForeColor = _tokens.PrimaryText
                If TypeOf root Is ListBox Then
                    DirectCast(root, ListBox).BorderStyle = BorderStyle.FixedSingle
                Else
                    DirectCast(root, ListView).BorderStyle = BorderStyle.FixedSingle
                End If
            ElseIf TypeOf root Is DataGridView Then
                Dim dgv = DirectCast(root, DataGridView)
                dgv.BackgroundColor = _tokens.WindowBackground
                dgv.GridColor = _tokens.Border
                dgv.BorderStyle = BorderStyle.None
                dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
                dgv.EnableHeadersVisualStyles = False

                dgv.ColumnHeadersDefaultCellStyle.BackColor = _tokens.SidebarBackground
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = _tokens.PrimaryText
                dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = _tokens.SidebarBackground
                dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = _tokens.PrimaryText

                dgv.DefaultCellStyle.BackColor = _tokens.CardBackground
                dgv.DefaultCellStyle.ForeColor = _tokens.PrimaryText
                dgv.DefaultCellStyle.SelectionBackColor = _tokens.Accent
                dgv.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White

                dgv.AlternatingRowsDefaultCellStyle.BackColor = _tokens.SurfaceBackground
                dgv.AlternatingRowsDefaultCellStyle.ForeColor = _tokens.PrimaryText
            End If

            ' Recursively apply theme to child controls
            For Each child As Control In root.Controls
                ApplyTheme(child)
            Next
        End Sub

        Private Sub LoadTheme()
            Dim themeToLoad = AppTheme.Light

            If File.Exists(_themeFilePath) Then
                Try
                    Dim json = File.ReadAllText(_themeFilePath)
                    Dim options = New JsonSerializerOptions With { .PropertyNameCaseInsensitive = True }
                    Dim state = JsonSerializer.Deserialize(Of ThemeState)(json, options)
                    If state IsNot Nothing Then
                        themeToLoad = state.Theme
                    End If
                Catch ex As Exception
                    ' Ignore errors and fallback to Light theme
                End Try
            End If

            ' Initialize the tokens without firing event during construction
            _currentTheme = themeToLoad
            _tokens = ThemeTokens.GetTokens(_currentTheme)
        End Sub

        Private Sub SaveTheme()
            Try
                Dim state = New ThemeState With { .Theme = _currentTheme }
                Dim json = JsonSerializer.Serialize(state)
                File.WriteAllText(_themeFilePath, json)
            Catch ex As Exception
                ' Ignore save errors (e.g. permission issues)
            End Try
        End Sub

        Private Class ThemeState
            Public Property Theme As AppTheme
        End Class

    End Class
End Namespace
