#If DEBUG Then
Imports System.Windows.Forms
Imports MerchSys.App.Views.Shell.Modules
Imports MerchSys.Accounting.Data
Imports Microsoft.EntityFrameworkCore
Imports Xunit
Imports System.IO

Public Class DeveloperToolsPanelTests

    <Fact>
    Public Sub DeveloperToolsPanel_Instantiates_AndButtonExists()
        Dim panel As New DeveloperToolsPanel()

        Assert.NotNull(panel)
        Dim btn = TryCast(panel.GetType().GetProperty("btnRunVatSchemaHarness", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(panel), Button)

        Assert.NotNull(btn)
        Assert.Equal("Run VAT Schema Harness", btn.Text)
    End Sub

    <Fact>
    Public Async Function btnRunVatSchemaHarness_Click_ExecutesHarness_AndSilencesDialog() As Task
        ' Implement trailing dot rule for VB.NET
        Dim options = New DbContextOptionsBuilder(Of AccountingDbContext)().
            UseMySQL("Server=dummy;Database=dummy;Uid=dummy;Pwd=dummy;").
            Options

        Dim dbContext As New AccountingDbContext(options)
        Dim panel As New DeveloperToolsPanel(dbContext)
        Dim notificationMsg As String = ""
        Dim notificationTitle As String = ""

        panel.ShowNotificationAction = Sub(msg, title, icon)
                                           notificationMsg = msg
                                           notificationTitle = title
                                       End Sub

        Dim btn = TryCast(panel.GetType().GetProperty("btnRunVatSchemaHarness", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(panel), Button)
        Assert.NotNull(btn)

        ' We can trigger the click by invoking the private handler or by calling PerformClick() which runs synchronously
        ' BUT btnRunVatSchemaHarness_Click is Async Sub which might return to caller before finishing Await.
        ' However, ExecuteAsync uses Task.FromResult(report), so it runs synchronously!
        ' Wait, VatLedgerSchemaHarnessRunner.RunAndReportAsync has an Await, but inside ExecuteAsync it returns Await Task.FromResult... which means it actually runs inline synchronously.
        ' File.WriteAllText is synchronous. So PerformClick() will complete fully!
        btn.PerformClick()

        ' Just yield to let any synchronous tasks finish up (silences BC42356)
        Await Task.Yield()

        ' Assert notification was shown
        Assert.Equal("Harness Success", notificationTitle)
        Assert.Contains("Harness executed successfully", notificationMsg)

        ' Extract report path from message
        Dim lines = notificationMsg.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        Dim reportPath = ""
        For Each line In lines
            If line.StartsWith("Report saved to: ") Then
                reportPath = line.Substring("Report saved to: ".Length).Trim()
                Exit For
            End If
        Next

        Assert.NotEmpty(reportPath)
        Assert.True(File.Exists(reportPath))

        ' Assert report contains PASS
        Dim content = File.ReadAllText(reportPath)
        Assert.Contains("PASS", content)

        ' Clean up
        File.Delete(reportPath)
    End Function
End Class
#End If
