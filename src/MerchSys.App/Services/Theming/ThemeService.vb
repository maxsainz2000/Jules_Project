Imports System.IO
Imports System.Text.Json
Imports System.Windows.Forms
Imports System.Drawing

Namespace Services.Theming
    Public Class ThemeService
        Implements IThemeService

        Private ReadOnly _themeFilePath As String
        Private _currentTheme As AppTheme
        Private _tokens As ThemeTokens

        Public Event ThemeChanged As EventHandler(Of AppTheme) Implements IThemeService.ThemeChanged

        Public Sub New()
            Dim appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            Dim vistaFolder = Path.Combine(appDataFolder, "VISTA")
            If Not Directory.Exists(vistaFolder) Then
                Directory.CreateDirectory(vistaFolder)
            End If

            _themeFilePath = Path.Combine(vistaFolder, "theme.json")
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
            _currentTheme = theme
            UpdateTokens()
            SaveTheme()
            RaiseEvent ThemeChanged(Me, _currentTheme)
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

            ' Apply to root
            StyleControl(root)

            ' Apply recursively
            For Each child As Control In root.Controls
                ApplyTheme(child)
            Next
        End Sub

        Private Sub StyleControl(ctrl As Control)
            ' Basic styling based on type
            If TypeOf ctrl Is Form Then
                ctrl.BackColor = _tokens.WindowBackground
                ctrl.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf ctrl Is Panel Then
                ctrl.BackColor = _tokens.SurfaceBackground
                ctrl.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf ctrl Is Button Then
                Dim btn = DirectCast(ctrl, Button)
                btn.BackColor = _tokens.Accent
                btn.ForeColor = _tokens.SurfaceBackground
                btn.FlatStyle = FlatStyle.Flat
                btn.FlatAppearance.BorderColor = _tokens.BorderDefault
                btn.Font = _tokens.DefaultFont
            ElseIf TypeOf ctrl Is Label Then
                ctrl.BackColor = Color.Transparent
                ctrl.ForeColor = _tokens.PrimaryText
                ctrl.Font = _tokens.DefaultFont
            ElseIf TypeOf ctrl Is TextBoxBase Then
                ctrl.BackColor = _tokens.SurfaceBackground
                ctrl.ForeColor = _tokens.PrimaryText
                ctrl.Font = _tokens.DefaultFont
            ElseIf TypeOf ctrl Is ComboBox Then
                ctrl.BackColor = _tokens.SurfaceBackground
                ctrl.ForeColor = _tokens.PrimaryText
                ctrl.Font = _tokens.DefaultFont
            ElseIf TypeOf ctrl Is DataGridView Then
                Dim dgv = DirectCast(ctrl, DataGridView)
                dgv.BackgroundColor = _tokens.SurfaceBackground
                dgv.GridColor = _tokens.BorderDefault
                dgv.DefaultCellStyle.BackColor = _tokens.SurfaceBackground
                dgv.DefaultCellStyle.ForeColor = _tokens.PrimaryText
                dgv.DefaultCellStyle.SelectionBackColor = _tokens.Accent
                dgv.DefaultCellStyle.SelectionForeColor = _tokens.SurfaceBackground
                dgv.ColumnHeadersDefaultCellStyle.BackColor = _tokens.SidebarBackground
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = _tokens.PrimaryText
            Else
                ' Fallback
                ctrl.BackColor = _tokens.SurfaceBackground
                ctrl.ForeColor = _tokens.PrimaryText
                ctrl.Font = _tokens.DefaultFont
            End If
        End Sub

        Private Sub LoadTheme()
            If File.Exists(_themeFilePath) Then
                Try
                    Dim json = File.ReadAllText(_themeFilePath)
                    Dim savedTheme = JsonSerializer.Deserialize(Of ThemeSettings)(json)
                    If savedTheme IsNot Nothing Then
                        _currentTheme = savedTheme.Theme
                    Else
                        _currentTheme = AppTheme.Light
                    End If
                Catch
                    _currentTheme = AppTheme.Light
                End Try
            Else
                _currentTheme = AppTheme.Light
            End If
            UpdateTokens()
        End Sub

        Private Sub SaveTheme()
            Try
                Dim settings As New ThemeSettings With {.Theme = _currentTheme}
                Dim json = JsonSerializer.Serialize(settings)
                File.WriteAllText(_themeFilePath, json)
            Catch
                ' Ignore save errors silently
            End Try
        End Sub

        Private Sub UpdateTokens()
            If _currentTheme = AppTheme.Light Then
                _tokens = ThemeTokens.CreateLightTokens()
            Else
                _tokens = ThemeTokens.CreateDarkTokens()
            End If
        End Sub

        Public Class ThemeSettings
            Public Property Theme As AppTheme
        End Class
    End Class
End Namespace
