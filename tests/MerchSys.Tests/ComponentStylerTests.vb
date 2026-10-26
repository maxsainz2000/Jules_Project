Imports System.Drawing
Imports System.Windows.Forms
Imports MerchSys.App.Services.Theming
Imports Xunit

Public Class ComponentStylerTests

    Private ReadOnly _tokens As ThemeTokens

    Public Sub New()
        _tokens = ThemeTokens.GetTokens(AppTheme.Light)
    End Sub

    <Fact>
    Public Sub ApplyPrimaryButton_SetsCorrectStyles()
        ' Arrange
        Dim btn = New Button()

        ' Act
        ComponentStyler.ApplyPrimaryButton(btn, _tokens)

        ' Assert
        Assert.Equal(_tokens.Accent, btn.BackColor)
        Assert.Equal(Color.White, btn.ForeColor)
        Assert.Equal(FlatStyle.Flat, btn.FlatStyle)
    End Sub

    <Fact>
    Public Sub ApplySuccessButton_SetsCorrectStyles()
        ' Arrange
        Dim btn = New Button()

        ' Act
        ComponentStyler.ApplySuccessButton(btn, _tokens)

        ' Assert
        Assert.Equal(_tokens.Success, btn.BackColor)
        Assert.Equal(Color.White, btn.ForeColor)
        Assert.Equal(FlatStyle.Flat, btn.FlatStyle)
    End Sub

    <Fact>
    Public Sub ApplyDangerButton_SetsCorrectStyles()
        ' Arrange
        Dim btn = New Button()

        ' Act
        ComponentStyler.ApplyDangerButton(btn, _tokens)

        ' Assert
        Assert.Equal(_tokens.Danger, btn.BackColor)
        Assert.Equal(Color.White, btn.ForeColor)
        Assert.Equal(FlatStyle.Flat, btn.FlatStyle)
    End Sub

    <Fact>
    Public Sub ApplyWarningButton_SetsCorrectStyles()
        ' Arrange
        Dim btn = New Button()

        ' Act
        ComponentStyler.ApplyWarningButton(btn, _tokens)

        ' Assert
        Assert.Equal(_tokens.Warning, btn.BackColor)
        Assert.Equal(Color.White, btn.ForeColor)
        Assert.Equal(FlatStyle.Flat, btn.FlatStyle)
    End Sub

    <Fact>
    Public Sub ApplySecondaryButton_SetsCorrectStyles()
        ' Arrange
        Dim btn = New Button()

        ' Act
        ComponentStyler.ApplySecondaryButton(btn, _tokens)

        ' Assert
        Assert.Equal(_tokens.SurfaceBackground, btn.BackColor)
        Assert.Equal(_tokens.PrimaryText, btn.ForeColor)
        Assert.Equal(_tokens.Border, btn.FlatAppearance.BorderColor)
        Assert.Equal(1, btn.FlatAppearance.BorderSize)
        Assert.Equal(FlatStyle.Flat, btn.FlatStyle)
    End Sub

    <Fact>
    Public Sub ApplySubtleButton_SetsCorrectStyles()
        ' Arrange
        Dim btn = New Button()

        ' Act
        ComponentStyler.ApplySubtleButton(btn, _tokens)

        ' Assert
        Assert.Equal(Color.Transparent, btn.BackColor)
        Assert.Equal(_tokens.PrimaryText, btn.ForeColor)
        Assert.Equal(0, btn.FlatAppearance.BorderSize)
        Assert.Equal(FlatStyle.Flat, btn.FlatStyle)
    End Sub

    <Fact>
    Public Sub ApplyCard_SetsCorrectStyles()
        ' Arrange
        Dim pnl = New Panel()

        ' Act
        ComponentStyler.ApplyCard(pnl, _tokens)

        ' Assert
        Assert.Equal(_tokens.CardBackground, pnl.BackColor)
        Assert.Equal(New Padding(12), pnl.Padding)
    End Sub

    <Fact>
    Public Sub ApplyAlertCard_SetsCorrectStyles()
        ' Arrange
        Dim pnlDanger = New Panel()
        Dim pnlWarning = New Panel()
        Dim pnlSuccess = New Panel()

        ' Act
        ComponentStyler.ApplyAlertCard(pnlDanger, _tokens, "Danger")
        ComponentStyler.ApplyAlertCard(pnlWarning, _tokens, "Warning")
        ComponentStyler.ApplyAlertCard(pnlSuccess, _tokens, "Success")

        ' Assert
        Assert.Equal(Color.FromArgb(40, _tokens.Danger.R, _tokens.Danger.G, _tokens.Danger.B), pnlDanger.BackColor)
        Assert.Equal(New Padding(12), pnlDanger.Padding)

        Assert.Equal(Color.FromArgb(40, _tokens.Warning.R, _tokens.Warning.G, _tokens.Warning.B), pnlWarning.BackColor)
        Assert.Equal(New Padding(12), pnlWarning.Padding)

        Assert.Equal(Color.FromArgb(40, _tokens.Success.R, _tokens.Success.G, _tokens.Success.B), pnlSuccess.BackColor)
        Assert.Equal(New Padding(12), pnlSuccess.Padding)
    End Sub

    <Fact>
    Public Sub ApplyTypography_SetsCorrectFontsAndColors()
        ' Arrange
        Dim lblTitle = New Label()
        Dim lblSection = New Label()
        Dim lblSubtitle = New Label()
        Dim lblCaption = New Label()
        Dim lblMetric = New Label()
        Dim lblMetricLarge = New Label()

        ' Act
        ComponentStyler.ApplyPageTitle(lblTitle, _tokens)
        ComponentStyler.ApplySectionHeader(lblSection, _tokens)
        ComponentStyler.ApplySubtitle(lblSubtitle, _tokens)
        ComponentStyler.ApplyCaption(lblCaption, _tokens)
        ComponentStyler.ApplyMetricValue(lblMetric, _tokens)
        ComponentStyler.ApplyMetricValue(lblMetricLarge, _tokens, True)

        ' Assert
        ' Check colors
        Assert.Equal(_tokens.PrimaryText, lblTitle.ForeColor)
        Assert.Equal(_tokens.PrimaryText, lblSection.ForeColor)
        Assert.Equal(_tokens.SecondaryText, lblSubtitle.ForeColor)
        Assert.Equal(_tokens.MutedText, lblCaption.ForeColor)
        Assert.Equal(_tokens.PrimaryText, lblMetric.ForeColor)
        Assert.Equal(_tokens.PrimaryText, lblMetricLarge.ForeColor)

        ' Check sizes
        Assert.Equal(16.0F, lblTitle.Font.Size)
        Assert.Equal(12.0F, lblSection.Font.Size)
        Assert.Equal(10.0F, lblSubtitle.Font.Size)
        Assert.Equal(8.0F, lblCaption.Font.Size)
        Assert.Equal(18.0F, lblMetric.Font.Size)
        Assert.Equal(24.0F, lblMetricLarge.Font.Size)

        ' Check styles
        Assert.Equal(FontStyle.Bold, lblTitle.Font.Style And FontStyle.Bold)
        Assert.Equal(FontStyle.Bold, lblSection.Font.Style And FontStyle.Bold)
        Assert.Equal(FontStyle.Regular, lblSubtitle.Font.Style And FontStyle.Regular)
        Assert.Equal(FontStyle.Regular, lblCaption.Font.Style And FontStyle.Regular)
        Assert.Equal(FontStyle.Bold, lblMetric.Font.Style And FontStyle.Bold)
        Assert.Equal(FontStyle.Bold, lblMetricLarge.Font.Style And FontStyle.Bold)
    End Sub

    <Fact>
    Public Sub ApplyNumericCellFormatting_SetsCorrectStyle()
        ' Arrange
        Dim col = New DataGridViewTextBoxColumn()

        ' Act
        ComponentStyler.ApplyNumericCellFormatting(col)

        ' Assert
        Assert.Equal(DataGridViewContentAlignment.MiddleRight, col.DefaultCellStyle.Alignment)
        Assert.Equal("N2", col.DefaultCellStyle.Format)
    End Sub

    <Fact>
    Public Sub ApplySemanticStatusRow_SetsCorrectColors()
        ' Arrange
        Dim dgv = New DataGridView()
        Dim col = New DataGridViewTextBoxColumn()
        dgv.Columns.Add(col)

        Dim indexSuccess = dgv.Rows.Add()
        Dim rowSuccess = dgv.Rows(indexSuccess)

        Dim indexWarning = dgv.Rows.Add()
        Dim rowWarning = dgv.Rows(indexWarning)

        Dim indexDanger = dgv.Rows.Add()
        Dim rowDanger = dgv.Rows(indexDanger)

        ' Act
        ComponentStyler.ApplySemanticStatusRow(rowSuccess, "Completed", _tokens)
        ComponentStyler.ApplySemanticStatusRow(rowWarning, "Pending", _tokens)
        ComponentStyler.ApplySemanticStatusRow(rowDanger, "Overdue", _tokens)

        ' Assert
        Assert.Equal(Color.FromArgb(40, _tokens.Success.R, _tokens.Success.G, _tokens.Success.B), rowSuccess.DefaultCellStyle.BackColor)
        Assert.Equal(_tokens.Success, rowSuccess.DefaultCellStyle.ForeColor)

        Assert.Equal(Color.FromArgb(40, _tokens.Warning.R, _tokens.Warning.G, _tokens.Warning.B), rowWarning.DefaultCellStyle.BackColor)
        Assert.Equal(_tokens.Warning, rowWarning.DefaultCellStyle.ForeColor)

        Assert.Equal(Color.FromArgb(40, _tokens.Danger.R, _tokens.Danger.G, _tokens.Danger.B), rowDanger.DefaultCellStyle.BackColor)
        Assert.Equal(_tokens.Danger, rowDanger.DefaultCellStyle.ForeColor)
    End Sub

    <Fact>
    Public Sub ThemeService_ApplyTheme_RoutesToComponentStyler_BasedOnTags()
        ' Arrange
        Dim tempPath = System.IO.Path.GetTempFileName()
        System.IO.File.Delete(tempPath)
        Dim service = New ThemeService(tempPath)

        Dim root = New Panel()

        Dim btnSuccess = New Button() With { .Tag = "Success" }
        Dim btnDanger = New Button() With { .Tag = "Danger" }
        Dim pnlCard = New Panel() With { .Tag = "Card" }
        Dim lblTitle = New Label() With { .Tag = "PageTitle" }

        root.Controls.Add(btnSuccess)
        root.Controls.Add(btnDanger)
        root.Controls.Add(pnlCard)
        root.Controls.Add(lblTitle)

        ' Act
        service.ApplyTheme(root)

        ' Assert
        ' Button
        Assert.Equal(service.Tokens.Success, btnSuccess.BackColor)
        Assert.Equal(service.Tokens.Danger, btnDanger.BackColor)

        ' Card
        Assert.Equal(service.Tokens.CardBackground, pnlCard.BackColor)
        Assert.Equal(New Padding(12), pnlCard.Padding)

        ' Typography
        Assert.Equal(service.Tokens.PrimaryText, lblTitle.ForeColor)
        Assert.Equal(16.0F, lblTitle.Font.Size)

        ' Cleanup
        System.IO.File.Delete(tempPath)
    End Sub

End Class