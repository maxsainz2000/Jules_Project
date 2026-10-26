Imports System.Windows.Forms
Imports MerchSys.Accounting.Data
#If DEBUG Then
Imports MerchSys.Accounting.Debug
#End If

Namespace Views.Shell.Modules
    Public Class DeveloperToolsPanel
        Inherits UserControl

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(Optional dbContext As AccountingDbContext = Nothing)
            InitializeComponent()
            Dock = DockStyle.Fill
            BackColor = System.Drawing.Color.Transparent
            _dbContext = dbContext

#If Not DEBUG Then
            btnRunVatSchemaHarness.Visible = False
#End If
        End Sub

#If DEBUG Then
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Friend Property ShowNotificationAction As Action(Of String, String, MessageBoxIcon) =
            Sub(msg, caption, icon) MessageBox.Show(msg, caption, MessageBoxButtons.OK, icon)

        Friend Async Function RunVatSchemaHarnessInternalAsync() As Task
            Dim reportPath As String = Nothing
            Dim errorMessage As String = Nothing

            Try
                reportPath = Await VatLedgerSchemaHarnessRunner.RunAndReportAsync(_dbContext)
            Catch ex As Exception
                errorMessage = ex.Message
            End Try

            If errorMessage IsNot Nothing Then
                ShowNotificationAction.Invoke($"Harness execution failed: {errorMessage}", "Error", MessageBoxIcon.Error)
            ElseIf reportPath IsNot Nothing Then
                ShowNotificationAction.Invoke($"Harness report generated successfully at:{Environment.NewLine}{reportPath}", "Success", MessageBoxIcon.Information)
            End If
        End Function

        Private Async Sub btnRunVatSchemaHarness_Click(sender As Object, e As EventArgs) Handles btnRunVatSchemaHarness.Click
            Await RunVatSchemaHarnessInternalAsync()
        End Sub
#End If
    End Class
End Namespace
