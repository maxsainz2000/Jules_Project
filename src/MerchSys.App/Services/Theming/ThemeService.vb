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
            ElseIf TypeOf root Is Panel Then
                root.BackColor = _tokens.SurfaceBackground
                root.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf root Is Button Then
                Dim btn = DirectCast(root, Button)
                btn.BackColor = _tokens.Accent
                btn.ForeColor = _tokens.PrimaryText
                btn.FlatStyle = FlatStyle.Flat
                btn.FlatAppearance.BorderColor = _tokens.Border
            ElseIf TypeOf root Is Label Then
                root.BackColor = System.Drawing.Color.Transparent
                root.ForeColor = _tokens.PrimaryText
            ElseIf TypeOf root Is TextBox Then
                root.BackColor = _tokens.CardBackground
                root.ForeColor = _tokens.PrimaryText
                ' Optional: Set border if using specialized text box
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
