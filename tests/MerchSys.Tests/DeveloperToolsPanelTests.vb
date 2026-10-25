#If DEBUG Then
Imports System.IO
Imports System.Windows.Forms
Imports Microsoft.EntityFrameworkCore
Imports Xunit
Imports MerchSys.App.Views.Shell.Modules
Imports MerchSys.Accounting.Data

Namespace Tests
    Public Class DeveloperToolsPanelTests

        <Fact>
        Public Sub DeveloperToolsPanel_Instantiates_AndButtonExists()
            ' Arrange & Act
            Dim panel As New DeveloperToolsPanel()

            ' Assert
            Assert.NotNull(panel)
            Dim btn = panel.Controls.Find("btnRunVatSchemaHarness", True)
            Assert.Single(btn)
            Assert.Equal("Run VAT Schema Harness", btn(0).Text)
        End Sub

        <Fact>
        Public Sub btnRunVatSchemaHarness_Click_ExecutesHarness_AndGeneratesReport()
            ' Arrange
            Dim optionsBuilder As New DbContextOptionsBuilder(Of AccountingDbContext)()
            optionsBuilder.UseMySQL("Server=dummy;Database=dummy;Uid=dummy;Pwd=dummy;")

            Dim tcs As New System.Threading.Tasks.TaskCompletionSource(Of Boolean)()
            Dim reportPath As String = Nothing

            Using context As New AccountingDbContext(optionsBuilder.Options)
                Dim panel As New DeveloperToolsPanel(context)
                Dim btn As Button = DirectCast(panel.Controls.Find("btnRunVatSchemaHarness", True)(0), Button)

                ' Act
                ' Using reflection to invoke the private event handler directly as we can't easily wait for a void async method without side effects or UI thread synchcontext
                Dim method = GetType(DeveloperToolsPanel).GetMethod("btnRunVatSchemaHarness_Click", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)

                If method IsNot Nothing Then
                   method.Invoke(panel, New Object() { btn, EventArgs.Empty })
                Else
                   btn.PerformClick()
                End If

                ' Wait a short bit for the async void to run and generate the file in temp
                System.Threading.Thread.Sleep(2000)

                ' We will find the latest file in temp starting with vat-ledger-schema-report-
                Dim tempDir = Path.GetTempPath()
                Dim files = Directory.GetFiles(tempDir, "vat-ledger-schema-report-*.md")

                ' Assert
                Assert.True(files.Length > 0, "No report file generated.")

                Dim latestFile = files.OrderByDescending(Function(f) File.GetCreationTimeUtc(f)).First()
                Dim content = File.ReadAllText(latestFile)

                Assert.Contains("PASS", content)
                Assert.Contains("✅ Pass", content)

                ' Cleanup
                File.Delete(latestFile)
            End Using
        End Sub
    End Class
End Namespace
#End If
