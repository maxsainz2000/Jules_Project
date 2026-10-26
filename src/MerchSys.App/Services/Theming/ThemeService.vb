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
                root.BackColor = _tokens.SurfaceBackground
                root.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf root Is GroupBox Then
                Dim gb = DirectCast(root, GroupBox)
                gb.BackColor = _tokens.SurfaceBackground
                gb.ForeColor = _tokens.PrimaryText
                gb.FlatStyle = FlatStyle.Flat
            ElseIf TypeOf root Is Label Then
                root.BackColor = System.Drawing.Color.Transparent
                root.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf root Is Button Then
                Dim btn = DirectCast(root, Button)
                btn.FlatStyle = FlatStyle.Flat
                Dim tagStr = Convert.ToString(btn.Tag)
                Dim btnName = If(btn.Name, "")
                If tagStr = "Accent" OrElse btnName.Contains("Primary") OrElse btnName.Contains("Save") OrElse btnName.Contains("Submit") OrElse btnName.Contains("Confirm") Then
                    btn.BackColor = _tokens.Accent
                    btn.ForeColor = System.Drawing.Color.White
                    btn.FlatAppearance.BorderColor = _tokens.Accent
                    btn.FlatAppearance.BorderSize = 1
                ElseIf tagStr = "Danger" OrElse btnName.Contains("Delete") OrElse btnName.Contains("Cancel") OrElse btnName.Contains("Logout") Then
                    btn.BackColor = _tokens.Danger
                    btn.ForeColor = System.Drawing.Color.White
                    btn.FlatAppearance.BorderColor = _tokens.Danger
                    btn.FlatAppearance.BorderSize = 1
                Else
                    btn.BackColor = _tokens.SurfaceBackground
                    btn.ForeColor = _tokens.PrimaryText
                    btn.FlatAppearance.BorderColor = _tokens.Border
                    btn.FlatAppearance.BorderSize = 1
                End If
            ElseIf TypeOf root Is TextBoxBase Then
                Dim txt = DirectCast(root, TextBoxBase)
                txt.BackColor = _tokens.CardBackground
                txt.ForeColor = _tokens.PrimaryText
                txt.BorderStyle = BorderStyle.FixedSingle
            ElseIf TypeOf root Is ComboBox Then
                Dim cbo = DirectCast(root, ComboBox)
                cbo.FlatStyle = FlatStyle.Flat
                cbo.BackColor = _tokens.CardBackground
                cbo.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf root Is CheckBox Then
                Dim chk = DirectCast(root, CheckBox)
                chk.FlatStyle = FlatStyle.Flat
                chk.BackColor = System.Drawing.Color.Transparent
                chk.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf root Is RadioButton Then
                Dim rdo = DirectCast(root, RadioButton)
                rdo.FlatStyle = FlatStyle.Flat
                rdo.BackColor = System.Drawing.Color.Transparent
                rdo.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf root Is ListControl Then
                Dim lst = DirectCast(root, ListControl)
                lst.BackColor = _tokens.CardBackground
                lst.ForeColor = _tokens.PrimaryText
                If TypeOf lst Is ListBox Then
                    DirectCast(lst, ListBox).BorderStyle = BorderStyle.FixedSingle
                End If
            ElseIf TypeOf root Is ListView Then
                Dim lvw = DirectCast(root, ListView)
                lvw.BackColor = _tokens.CardBackground
                lvw.ForeColor = _tokens.PrimaryText
                lvw.BorderStyle = BorderStyle.FixedSingle
            ElseIf TypeOf root Is TabControl Then
                Dim tab = DirectCast(root, TabControl)
                tab.Appearance = TabAppearance.Normal
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
