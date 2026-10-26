#If DEBUG Then
Imports System.IO
Imports System.Windows.Forms
Imports MerchSys.App.Views.Shell.Modules
Imports Xunit
Imports MerchSys.Accounting.Data
Imports Microsoft.EntityFrameworkCore

Public Class DeveloperToolsPanelTests

    <Fact>
    Public Sub DeveloperToolsPanel_Instantiates_AndButtonExists()
        ' Arrange & Act
        Dim panel As New DeveloperToolsPanel()

        ' Assert
        Assert.NotNull(panel.btnRunVatSchemaHarness)
        Assert.Equal("Run VAT Schema Harness", panel.btnRunVatSchemaHarness.Text)
    End Sub

    <Fact>
    Public Async Function btnRunVatSchemaHarness_Click_ExecutesHarness_AndGeneratesReport() As Task
        ' Arrange
        Dim options = New DbContextOptionsBuilder(Of AccountingDbContext)() _
            .UseMySQL("Server=dummy;Database=dummy;Uid=dummy;Pwd=dummy;") _
            .Options

        Dim dbContext = New AccountingDbContext(options)
        Dim panel As New DeveloperToolsPanel(dbContext)

        Dim capturedMsg As String = Nothing
        panel.ShowNotificationAction = Sub(msg, caption, icon) capturedMsg = msg

        ' Act
        Await panel.RunVatSchemaHarnessInternalAsync()

        ' Assert
        Assert.NotNull(capturedMsg)
        Assert.Contains("Harness report generated successfully at", capturedMsg)

        Dim lines = capturedMsg.Split(New String() {Environment.NewLine}, StringSplitOptions.None)
        Dim filePath = lines(1)

        Assert.True(File.Exists(filePath), $"File {filePath} should exist")

        Dim content = File.ReadAllText(filePath)
        Assert.Contains("PASS", content)

        ' Clean up
        File.Delete(filePath)
    End Function

End Class
#End If