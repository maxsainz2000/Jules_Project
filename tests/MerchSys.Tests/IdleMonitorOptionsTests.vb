Imports Xunit
Imports MerchSys.App.Services

''' <summary>
''' Guards the idle-timeout configuration.
'''
''' A zero timeout is not a harmless default: <c>WinFormsIdleMonitor</c> compares elapsed
''' seconds against it, so zero makes every session expire on the first timer tick. That is
''' what happened whenever <c>appsettings.json</c> was missing from the deployed output, since
''' the options class previously had no defaults of its own.
''' </summary>
Public Class IdleMonitorOptionsTests

    <Fact>
    Public Sub UnsetSectionLeavesTheDocumentedDefaults()
        ' Configuration binding leaves CLR defaults in place when the section is absent, so
        ' constructing the type is equivalent to binding an empty section.
        Dim options As New IdleMonitorOptions()

        Assert.Equal(IdleMonitorOptions.DefaultIdleTimeoutMinutes, options.IdleTimeoutMinutes)
        Assert.Equal(IdleMonitorOptions.DefaultWarningLeadSeconds, options.WarningLeadSeconds)
    End Sub

    <Fact>
    Public Sub DefaultsAreUsable()
        Dim options As New IdleMonitorOptions()

        Assert.True(options.IdleTimeoutMinutes > 0, "a non-positive timeout expires every session immediately")
        Assert.True(options.WarningLeadSeconds >= 0)
        Assert.True(options.WarningLeadSeconds < options.IdleTimeoutMinutes * 60)
    End Sub

    <Fact>
    Public Sub ZeroTimeoutFallsBackInsteadOfExpiringImmediately()
        Dim options As New IdleMonitorOptions() With {.IdleTimeoutMinutes = 0}

        Assert.Equal(IdleMonitorOptions.DefaultIdleTimeoutMinutes, options.IdleTimeoutMinutes)
    End Sub

    <Fact>
    Public Sub NegativeTimeoutFallsBack()
        Dim options As New IdleMonitorOptions() With {.IdleTimeoutMinutes = -5}

        Assert.Equal(IdleMonitorOptions.DefaultIdleTimeoutMinutes, options.IdleTimeoutMinutes)
    End Sub

    <Fact>
    Public Sub ExplicitValuesAreHonoured()
        Dim options As New IdleMonitorOptions() With {
            .IdleTimeoutMinutes = 30,
            .WarningLeadSeconds = 90
        }

        Assert.Equal(30, options.IdleTimeoutMinutes)
        Assert.Equal(90, options.WarningLeadSeconds)
    End Sub

End Class
