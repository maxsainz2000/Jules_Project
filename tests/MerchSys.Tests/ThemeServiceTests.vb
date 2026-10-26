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
        Dim button = New Button() With { .Tag = "Accent" }
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

    <Fact>
    Public Sub ThemeService_ApplyTheme_DataGridView()
        ' Arrange
        Dim tempPath = Path.GetTempFileName()
        File.Delete(tempPath)
        Dim service = New ThemeService(tempPath)
        Dim dgv = New DataGridView()

        ' Act
        service.ApplyTheme(dgv)

        ' Assert
        Assert.Equal(service.Tokens.WindowBackground, dgv.BackgroundColor)
        Assert.Equal(service.Tokens.Border, dgv.GridColor)
        Assert.Equal(BorderStyle.None, dgv.BorderStyle)
        Assert.Equal(DataGridViewCellBorderStyle.SingleHorizontal, dgv.CellBorderStyle)
        Assert.False(dgv.EnableHeadersVisualStyles)

        Assert.Equal(service.Tokens.SidebarBackground, dgv.ColumnHeadersDefaultCellStyle.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, dgv.ColumnHeadersDefaultCellStyle.ForeColor)
        Assert.Equal(service.Tokens.SidebarBackground, dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor)
        Assert.Equal(service.Tokens.PrimaryText, dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor)

        Assert.Equal(service.Tokens.CardBackground, dgv.DefaultCellStyle.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, dgv.DefaultCellStyle.ForeColor)
        Assert.Equal(service.Tokens.Accent, dgv.DefaultCellStyle.SelectionBackColor)
        Assert.Equal(System.Drawing.Color.White, dgv.DefaultCellStyle.SelectionForeColor)

        Assert.Equal(service.Tokens.SurfaceBackground, dgv.AlternatingRowsDefaultCellStyle.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, dgv.AlternatingRowsDefaultCellStyle.ForeColor)
    End Sub

    <Fact>
    Public Sub ThemeService_ApplyTheme_ButtonVariants()
        ' Arrange
        Dim tempPath = Path.GetTempFileName()
        File.Delete(tempPath)
        Dim service = New ThemeService(tempPath)

        Dim btnAccent = New Button() With { .Tag = "Accent" }
        Dim btnDanger = New Button() With { .Tag = "Danger" }
        Dim btnStandard = New Button()

        ' Act
        service.ApplyTheme(btnAccent)
        service.ApplyTheme(btnDanger)
        service.ApplyTheme(btnStandard)

        ' Assert
        Assert.Equal(service.Tokens.Accent, btnAccent.BackColor)
        Assert.Equal(System.Drawing.Color.White, btnAccent.ForeColor)

        Assert.Equal(service.Tokens.Danger, btnDanger.BackColor)
        Assert.Equal(System.Drawing.Color.White, btnDanger.ForeColor)

        Assert.Equal(service.Tokens.SurfaceBackground, btnStandard.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, btnStandard.ForeColor)
        Assert.Equal(service.Tokens.Border, btnStandard.FlatAppearance.BorderColor)
        Assert.Equal(1, btnStandard.FlatAppearance.BorderSize)
    End Sub

    <Fact>
    Public Sub ThemeService_ApplyTheme_CommonControls()
        ' Arrange
        Dim tempPath = Path.GetTempFileName()
        File.Delete(tempPath)
        Dim service = New ThemeService(tempPath)

        Dim cmb = New ComboBox()
        Dim chk = New CheckBox()
        Dim rdo = New RadioButton()
        Dim lst = New ListBox()
        Dim tab = New TabControl()

        ' Act
        service.ApplyTheme(cmb)
        service.ApplyTheme(chk)
        service.ApplyTheme(rdo)
        service.ApplyTheme(lst)
        service.ApplyTheme(tab)

        ' Assert
        ' ComboBox
        Assert.Equal(FlatStyle.Flat, cmb.FlatStyle)
        Assert.Equal(service.Tokens.CardBackground, cmb.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, cmb.ForeColor)

        ' CheckBox
        Assert.Equal(FlatStyle.Flat, chk.FlatStyle)
        Assert.Equal(System.Drawing.Color.Transparent, chk.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, chk.ForeColor)

        ' RadioButton
        Assert.Equal(FlatStyle.Flat, rdo.FlatStyle)
        Assert.Equal(System.Drawing.Color.Transparent, rdo.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, rdo.ForeColor)

        ' ListBox
        Assert.Equal(BorderStyle.FixedSingle, lst.BorderStyle)
        Assert.Equal(service.Tokens.CardBackground, lst.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, lst.ForeColor)

        ' TabControl
        Assert.Equal(TabAppearance.Normal, tab.Appearance)
    End Sub
End Class
