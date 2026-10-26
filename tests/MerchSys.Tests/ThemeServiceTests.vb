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
    Public Sub ThemeService_ApplyTheme_StylesStandardControls()
        ' Arrange
        Dim tempPath = Path.GetTempFileName()
        File.Delete(tempPath)
        Dim service = New ThemeService(tempPath)

        Dim pnl = New Panel()
        Dim cbo = New ComboBox()
        Dim chk = New CheckBox()
        Dim rdo = New RadioButton()
        Dim lst = New ListBox()
        Dim tab = New TabControl()
        Dim txt = New TextBox()

        pnl.Controls.Add(cbo)
        pnl.Controls.Add(chk)
        pnl.Controls.Add(rdo)
        pnl.Controls.Add(lst)
        pnl.Controls.Add(tab)
        pnl.Controls.Add(txt)

        ' Act
        service.SetTheme(AppTheme.Dark)
        service.ApplyTheme(pnl)

        ' Assert
        Assert.Equal(service.Tokens.CardBackground, cbo.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, cbo.ForeColor)
        Assert.Equal(FlatStyle.Flat, cbo.FlatStyle)

        Assert.Equal(System.Drawing.Color.Transparent, chk.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, chk.ForeColor)
        Assert.Equal(FlatStyle.Flat, chk.FlatStyle)

        Assert.Equal(System.Drawing.Color.Transparent, rdo.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, rdo.ForeColor)
        Assert.Equal(FlatStyle.Flat, rdo.FlatStyle)

        Assert.Equal(service.Tokens.CardBackground, lst.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, lst.ForeColor)
        Assert.Equal(BorderStyle.FixedSingle, lst.BorderStyle)

        Assert.Equal(TabAppearance.Normal, tab.Appearance)

        Assert.Equal(service.Tokens.CardBackground, txt.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, txt.ForeColor)
        Assert.Equal(BorderStyle.FixedSingle, txt.BorderStyle)

        ' Cleanup
        File.Delete(tempPath)
    End Sub

    <Fact>
    Public Sub ThemeService_ApplyTheme_StylesButtonVariants()
        ' Arrange
        Dim tempPath = Path.GetTempFileName()
        File.Delete(tempPath)
        Dim service = New ThemeService(tempPath)

        Dim pnl = New Panel()

        Dim btnStandard = New Button()

        Dim btnAccentTag = New Button() With { .Tag = "Accent" }
        Dim btnAccentName = New Button() With { .Name = "btnSave" }

        Dim btnDangerTag = New Button() With { .Tag = "Danger" }
        Dim btnDangerName = New Button() With { .Name = "btnDelete" }

        pnl.Controls.Add(btnStandard)
        pnl.Controls.Add(btnAccentTag)
        pnl.Controls.Add(btnAccentName)
        pnl.Controls.Add(btnDangerTag)
        pnl.Controls.Add(btnDangerName)

        ' Act
        service.SetTheme(AppTheme.Dark)
        service.ApplyTheme(pnl)

        ' Assert
        Assert.Equal(service.Tokens.SurfaceBackground, btnStandard.BackColor)
        Assert.Equal(service.Tokens.PrimaryText, btnStandard.ForeColor)
        Assert.Equal(service.Tokens.Border, btnStandard.FlatAppearance.BorderColor)

        Assert.Equal(service.Tokens.Accent, btnAccentTag.BackColor)
        Assert.Equal(System.Drawing.Color.White, btnAccentTag.ForeColor)
        Assert.Equal(service.Tokens.Accent, btnAccentTag.FlatAppearance.BorderColor)

        Assert.Equal(service.Tokens.Accent, btnAccentName.BackColor)
        Assert.Equal(System.Drawing.Color.White, btnAccentName.ForeColor)
        Assert.Equal(service.Tokens.Accent, btnAccentName.FlatAppearance.BorderColor)

        Assert.Equal(service.Tokens.Danger, btnDangerTag.BackColor)
        Assert.Equal(System.Drawing.Color.White, btnDangerTag.ForeColor)
        Assert.Equal(service.Tokens.Danger, btnDangerTag.FlatAppearance.BorderColor)

        Assert.Equal(service.Tokens.Danger, btnDangerName.BackColor)
        Assert.Equal(System.Drawing.Color.White, btnDangerName.ForeColor)
        Assert.Equal(service.Tokens.Danger, btnDangerName.FlatAppearance.BorderColor)

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

    <Fact>
    Public Sub ThemeService_ApplyTheme_StylesDataGridView()
        ' Arrange
        Dim tempPath = Path.GetTempFileName()
        File.Delete(tempPath)
        Dim service = New ThemeService(tempPath)

        Dim dgv = New DataGridView()
        Dim form = New Form()
        form.Controls.Add(dgv)

        ' Act
        service.SetTheme(AppTheme.Dark)
        service.ApplyTheme(form)

        ' Assert
        Assert.False(dgv.EnableHeadersVisualStyles)
        Assert.Equal(BorderStyle.None, dgv.BorderStyle)
        Assert.Equal(DataGridViewCellBorderStyle.SingleHorizontal, dgv.CellBorderStyle)
        Assert.Equal(service.Tokens.WindowBackground, dgv.BackgroundColor)
        Assert.Equal(service.Tokens.Border, dgv.GridColor)

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

        ' Cleanup
        File.Delete(tempPath)
    End Sub
End Class
