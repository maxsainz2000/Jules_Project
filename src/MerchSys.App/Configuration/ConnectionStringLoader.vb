Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging

Namespace Configuration

    Public Module ConnectionStringLoader

        Public Function GetProductionConfigPath() As String
            Dim exePath = AppContext.BaseDirectory
            Return Path.Combine(exePath, "appsettings.Production.json")
        End Function

        <Extension()>
        Public Function AddProductionOverlay(builder As IConfigurationBuilder) As IConfigurationBuilder
            Dim prodPath = GetProductionConfigPath()
            builder.AddJsonFile(prodPath, optional:=True, reloadOnChange:=True)
            Return builder
        End Function

        Public Function GetMariaDbConnectionString(cfg As IConfiguration, logger As ILogger) As String
            Dim connString = cfg.GetSection("Sync")("MariaDbConnection")

            If String.IsNullOrWhiteSpace(connString) Then
                If logger IsNot Nothing Then
                    logger.LogWarning("Sync:MariaDbConnection is missing or empty. Please configure appsettings.Production.json.")
                End If
            End If

            Return connString
        End Function

    End Module

End Namespace
