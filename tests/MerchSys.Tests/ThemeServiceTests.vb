Imports System.IO
Imports System.Windows.Forms
Imports MerchSys.App.Services.Theming
Imports Xunit

Public Class ThemeServiceTests
    Private ReadOnly _themeFilePath As String

    Public Sub New()
        Dim appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        Dim vistaFolder = Path.Combine(appDataFolder, "VISTA")
        _themeFilePath = Path.Combine(vistaFolder, "theme.json")
    End Sub

    <Fact>
    Public Sub ThemeService_InitializesToLightOrPersisted()
        ' Ensure the file doesn't exist for the first part of the test
        If File.Exists(_themeFilePath) Then
            File.Delete(_themeFilePath)
        End If

        Dim service As New ThemeService()

        Assert.Equal(AppTheme.Light, service.CurrentTheme)
        Assert.NotNull(service.Tokens)
        Assert.Equal(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"), service.Tokens.SurfaceBackground)

        ' Now test persistence
        service.SetTheme(AppTheme.Dark)

        ' Re-instantiate to test loading
        Dim service2 As New ThemeService()
        Assert.Equal(AppTheme.Dark, service2.CurrentTheme)
        Assert.Equal(System.Drawing.ColorTranslator.FromHtml("#2D2D30"), service2.Tokens.SurfaceBackground)

        ' Cleanup
        If File.Exists(_themeFilePath) Then
            File.Delete(_themeFilePath)
        End If
    End Sub

    <Fact>
    Public Sub ToggleTheme_ChangesThemeAndRaisesEvent()
        ' Setup clean state
        If File.Exists(_themeFilePath) Then
            File.Delete(_themeFilePath)
        End If

        Dim service As New ThemeService()
        Assert.Equal(AppTheme.Light, service.CurrentTheme)

        Dim eventRaised As Boolean = False
        Dim newTheme As AppTheme
        AddHandler service.ThemeChanged, Sub(sender, theme)
                                             eventRaised = True
                                             newTheme = theme
                                         End Sub

        service.ToggleTheme()

        Assert.True(eventRaised)
        Assert.Equal(AppTheme.Dark, newTheme)
        Assert.Equal(AppTheme.Dark, service.CurrentTheme)

        eventRaised = False
        service.ToggleTheme()

        Assert.True(eventRaised)
        Assert.Equal(AppTheme.Light, newTheme)
        Assert.Equal(AppTheme.Light, service.CurrentTheme)

        ' Cleanup
        If File.Exists(_themeFilePath) Then
            File.Delete(_themeFilePath)
        End If
    End Sub

    <Fact>
    Public Sub ApplyTheme_UpdatesControlColors()
        ' Setup clean state
        If File.Exists(_themeFilePath) Then
            File.Delete(_themeFilePath)
        End If

        Dim service As New ThemeService()
        service.SetTheme(AppTheme.Dark)

        Dim form As New Form()
        Dim panel As New Panel()
        Dim btn As New Button()

        form.Controls.Add(panel)
        panel.Controls.Add(btn)

        service.ApplyTheme(form)

        Assert.Equal(service.Tokens.WindowBackground, form.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, form.ForeColor)

        Assert.Equal(service.Tokens.SurfaceBackground, panel.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, panel.ForeColor)

        Assert.Equal(service.Tokens.Accent, btn.BackColor)
        Assert.Equal(service.Tokens.SurfaceBackground, btn.ForeColor)

        ' Cleanup
        If File.Exists(_themeFilePath) Then
            File.Delete(_themeFilePath)
        End If
    End Sub
End Class
