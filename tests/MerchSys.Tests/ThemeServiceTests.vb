Imports System.IO
Imports System.Text.Json
Imports System.Windows.Forms
Imports MerchSys.App.Services.Theming
Imports Xunit

Public Class ThemeServiceTests
    <Fact>
    Public Sub ThemeService_InitializesToLightByDefault()
        ' Arrange
        Dim tempPath = Path.GetTempFileName()
        File.Delete(tempPath) ' Ensure it doesn't exist

        ' Act
        Dim service = New ThemeService(tempPath)

        ' Assert
        Assert.Equal(AppTheme.Light, service.CurrentTheme)
        Assert.NotNull(service.Tokens)
    End Sub

    <Fact>
    Public Sub ThemeService_LoadsExistingThemeFromFile()
        ' Arrange
        Dim tempPath = Path.GetTempFileName()
        Dim json = "{ ""Theme"": 1 }" ' Dark
        File.WriteAllText(tempPath, json)

        ' Act
        Dim service = New ThemeService(tempPath)

        ' Assert
        Assert.Equal(AppTheme.Dark, service.CurrentTheme)

        ' Cleanup
        File.Delete(tempPath)
    End Sub

    <Fact>
    Public Sub ThemeService_ApplyTheme_UpdatesControlColors()
        ' Arrange
        Dim tempPath = Path.GetTempFileName()
        File.Delete(tempPath)
        Dim service = New ThemeService(tempPath)

        Dim panel = New Panel()
        Dim button = New Button()
        panel.Controls.Add(button)

        ' Act
        service.SetTheme(AppTheme.Dark)
        service.ApplyTheme(panel)

        ' Assert
        Assert.Equal(service.Tokens.SurfaceBackground, panel.BackColor)
        Assert.Equal(service.Tokens.Accent, button.BackColor)

        ' Act again
        service.SetTheme(AppTheme.Light)
        service.ApplyTheme(panel)

        ' Assert again
        Assert.Equal(service.Tokens.SurfaceBackground, panel.BackColor)
        Assert.Equal(service.Tokens.Accent, button.BackColor)

        ' Cleanup
        File.Delete(tempPath)
    End Sub

    <Fact>
    Public Sub ThemeService_ToggleTheme_ChangesThemeAndFiresEvent()
        ' Arrange
        Dim tempPath = Path.GetTempFileName()
        File.Delete(tempPath)
        Dim service = New ThemeService(tempPath)

        Dim eventFired = False
        Dim newTheme As AppTheme = AppTheme.Light
        AddHandler service.ThemeChanged, Sub(sender, e)
                                             eventFired = True
                                             newTheme = e
                                         End Sub

        ' Act
        service.ToggleTheme()

        ' Assert
        Assert.True(eventFired)
        Assert.Equal(AppTheme.Dark, newTheme)
        Assert.Equal(AppTheme.Dark, service.CurrentTheme)

        ' Act again
        eventFired = False
        service.ToggleTheme()

        ' Assert again
        Assert.True(eventFired)
        Assert.Equal(AppTheme.Light, newTheme)
        Assert.Equal(AppTheme.Light, service.CurrentTheme)

        ' Cleanup
        File.Delete(tempPath)
    End Sub
End Class
