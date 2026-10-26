Imports System.Windows.Forms
Imports MerchSys.Accounting.Data
#If DEBUG Then
Imports MerchSys.Accounting.Debug
#End If

Namespace Views.Shell.Modules
    Public Class DeveloperToolsPanel
        Inherits UserControl

        Private ReadOnly _dbContext As AccountingDbContext
        Private ReadOnly _themeService As MerchSys.App.Services.Theming.IThemeService

#If DEBUG Then
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property ShowNotificationAction As Action(Of String, String, MessageBoxIcon) = Sub(msg, title, icon) MessageBox.Show(msg, title, MessageBoxButtons.OK, icon)
#End If

        Public Sub New(Optional dbContext As AccountingDbContext = Nothing, Optional themeService As MerchSys.App.Services.Theming.IThemeService = Nothing)
            InitializeComponent()
            Dock = DockStyle.Fill
            BackColor = System.Drawing.Color.Transparent
            _dbContext = dbContext
            _themeService = themeService

#If Not DEBUG Then
            btnRunVatSchemaHarness.Visible = False
#End If
        End Sub

#If DEBUG Then
        Private Async Sub btnRunVatSchemaHarness_Click(sender As Object, e As EventArgs) Handles btnRunVatSchemaHarness.Click
            If _dbContext Is Nothing Then
                ShowNotificationAction?.Invoke("Database context is not available.", "Harness Error", MessageBoxIcon.Error)
                Return
            End If

            Try
                Dim reportPath = Await VatLedgerSchemaHarnessRunner.RunAndReportAsync(_dbContext)
                ShowNotificationAction?.Invoke($"Harness executed successfully." & Environment.NewLine & $"Report saved to: {reportPath}", "Harness Success", MessageBoxIcon.Information)
            Catch ex As Exception
                ShowNotificationAction?.Invoke($"Error running harness: {ex.Message}", "Harness Error", MessageBoxIcon.Error)
            End Try
        End Sub
#End If

        Private Sub btnToggleTheme_Click(sender As Object, e As EventArgs) Handles btnToggleTheme.Click
            If _themeService IsNot Nothing Then
                _themeService.ToggleTheme()
            End If
        End Sub

    End Class
End Namespace
