Imports MerchSys.App.Controls
Imports MerchSys.App.Presenters
Imports MerchSys.App.Services.Theming
Imports MerchSys.App.Views.Dialogs
Imports Microsoft.Extensions.DependencyInjection
Imports Moq
Imports Xunit
Imports System.Threading.Tasks
Imports MerchSys.SharedKernel.Interfaces

Public Class StateAndFeedbackTests

    <Fact>
    Public Sub BusyOverlay_TogglingIsBusy_UpdatesVisibleAndMessage()
        Using overlay As New BusyOverlay()
            ' Initial state
            Assert.False(overlay.IsBusy)
            Assert.False(overlay.Visible)
            Assert.Equal("Loading...", overlay.Message)

            ' Set properties
            overlay.Message = "Please wait..."
            overlay.IsBusy = True

            ' Verify state
            Assert.True(overlay.IsBusy)
            Assert.True(overlay.Visible)
            Assert.Equal("Please wait...", overlay.Message)

            ' Turn off
            overlay.IsBusy = False
            Assert.False(overlay.Visible)
        End Using
    End Sub

    <Fact>
    Public Async Function EmptyStatePanel_SetProperties_FiresEvent() As Task
        Using panel As New EmptyStatePanel()
            ' Initial state
            Assert.Equal("No items found", panel.Title)

            ' Set properties
            panel.Title = "Custom Title"
            panel.Description = "Custom Description"
            panel.ActionText = "Retry"

            Assert.Equal("Custom Title", panel.Title)
            Assert.Equal("Custom Description", panel.Description)
            Assert.Equal("Retry", panel.ActionText)

            ' Verify event firing by simulating button click
            Dim eventFired As Boolean = False
            AddHandler panel.ActionClicked, Sub(sender, e) eventFired = True

            ' Reflection to get the button and invoke click
            Dim btnProp = panel.GetType().GetProperty("btnAction", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
            Dim btn = DirectCast(btnProp.GetValue(panel), System.Windows.Forms.Button)
            btn.PerformClick()

            Assert.True(eventFired)
            Await Task.Yield()
        End Using
    End Function

    <Fact>
    Public Sub DefaultConflictPresenter_CanInstantiate()
        Dim mockTheme = New Mock(Of IThemeService)()
        Dim presenter As New DefaultConflictPresenter(mockTheme.Object)
        Assert.NotNull(presenter)
    End Sub


    <Fact>
    Public Sub Program_Registers_IConflictPresenter()
        Dim services As New ServiceCollection()
        Dim mockTheme = New Mock(Of IThemeService)()
        services.AddSingleton(mockTheme.Object)
        services.AddTransient(Of IConflictPresenter, DefaultConflictPresenter)()

        Dim provider = services.BuildServiceProvider()
        Dim presenter = provider.GetService(Of IConflictPresenter)()

        Assert.NotNull(presenter)
        Assert.IsType(Of DefaultConflictPresenter)(presenter)
    End Sub


    <Fact>
    Public Sub ErrorStatePanel_SetProperties_FiresRetryEvent()
        Using panel As New ErrorStatePanel()
            panel.Title = "Error"
            panel.Message = "Failed"
            panel.RetryText = "Retry"

            Assert.Equal("Error", panel.Title)
            Assert.Equal("Failed", panel.Message)
            Assert.Equal("Retry", panel.RetryText)

            Dim fired = False
            AddHandler panel.RetryClicked, Sub(s, e) fired = True

            Dim btnProp = panel.GetType().GetProperty("btnRetry", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
            Dim btn = DirectCast(btnProp.GetValue(panel), System.Windows.Forms.Button)
            btn.PerformClick()

            Assert.True(fired)
        End Using
    End Sub

    <Fact>
    Public Sub ErrorStatePanel_ApplyTheme_UpdatesColors()
        Using panel As New ErrorStatePanel()
            Dim tokens As New ThemeTokens()
            tokens.Danger = System.Drawing.Color.Red
            tokens.SecondaryText = System.Drawing.Color.Gray
            panel.ApplyTheme(tokens)

            Dim lblTitleProp = panel.GetType().GetProperty("lblTitle", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
            Dim lblTitle = DirectCast(lblTitleProp.GetValue(panel), System.Windows.Forms.Label)

            Dim lblMsgProp = panel.GetType().GetProperty("lblMessage", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
            Dim lblMsg = DirectCast(lblMsgProp.GetValue(panel), System.Windows.Forms.Label)

            Assert.Equal(System.Drawing.Color.Red, lblTitle.ForeColor)
            Assert.Equal(System.Drawing.Color.Gray, lblMsg.ForeColor)
        End Using
    End Sub

End Class
