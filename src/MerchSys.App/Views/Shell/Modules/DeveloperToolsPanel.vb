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
        End Sub

#If DEBUG Then
        Private Async Sub btnRunVatSchemaHarness_Click(sender As Object, e As EventArgs) Handles btnRunVatSchemaHarness.Click
            Dim exMsg As String = Nothing
            Dim reportPath As String = Nothing
            Try
                btnRunVatSchemaHarness.Enabled = False
                reportPath = Await VatLedgerSchemaHarnessRunner.RunAndReportAsync(_dbContext)
            Catch ex As Exception
                exMsg = ex.Message
            Finally
                btnRunVatSchemaHarness.Enabled = True
            End Try

            If exMsg IsNot Nothing Then
                MessageBox.Show($"VAT Schema Harness failed: {exMsg}", "Schema Verification Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                MessageBox.Show($"VAT Schema Harness completed successfully! Report generated at:{Environment.NewLine}{reportPath}", "Schema Verification", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Sub
#End If

    End Class
End Namespace
