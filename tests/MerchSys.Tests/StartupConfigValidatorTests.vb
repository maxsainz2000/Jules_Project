Imports Xunit
Imports MerchSys.App.Configuration

''' <summary>
''' Covers the pre-build configuration guard (C4/M15). The check has to run before the host is
''' built, because a missing connection string otherwise throws from inside ConfigureServices
''' while Build() is running and the operator dialog never appears.
''' </summary>
Public Class StartupConfigValidatorTests

    <Fact>
    Public Sub RejectsMissingConnectionString()
        Assert.NotNull(StartupConfigValidator.ValidateConnectionString(Nothing))
        Assert.NotNull(StartupConfigValidator.ValidateConnectionString(String.Empty))
        Assert.NotNull(StartupConfigValidator.ValidateConnectionString("   "))
    End Sub

    <Fact>
    Public Sub AcceptsAUsableConnectionString()
        Assert.Null(StartupConfigValidator.ValidateConnectionString(
            "Server=host;Port=3306;Database=merchsys_central;Uid=u;Pwd=p;"))
    End Sub

    <Fact>
    Public Sub MessagePointsTheOperatorAtTheConfigPath()
        Dim message = StartupConfigValidator.ValidateConnectionString(Nothing)

        Assert.Contains(ConnectionStringLoader.GetProductionConfigPath(), message)
    End Sub

End Class
