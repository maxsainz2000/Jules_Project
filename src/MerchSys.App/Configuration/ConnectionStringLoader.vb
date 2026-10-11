Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging

Namespace Configuration

    Public Module ConnectionStringLoader

        ''' <summary>
        ''' The canonical, operator-documented location for the production configuration file.
        ''' Matches the runbook (Plans/VISTA_Modules/Infrastructure/runbooks): operators place
        ''' the file under <c>%LOCALAPPDATA%\VISTA\</c>.
        ''' </summary>
        Public Function GetProductionConfigPath() As String
            Dim localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Return Path.Combine(localAppData, "VISTA", "appsettings.Production.json")
        End Function

        ''' <summary>
        ''' Developer fallback: a production file sitting next to the executable (gitignored).
        ''' Used only when the canonical LOCALAPPDATA file is absent, so local debugging keeps
        ''' working without weakening the operator contract.
        ''' </summary>
        Public Function GetDeveloperFallbackPath() As String
            Return Path.Combine(AppContext.BaseDirectory, "appsettings.Production.json")
        End Function

        ''' <summary>
        ''' Returns True when at least one production configuration file exists (canonical or fallback).
        ''' </summary>
        Public Function ProductionConfigExists() As Boolean
            Return File.Exists(GetProductionConfigPath()) OrElse File.Exists(GetDeveloperFallbackPath())
        End Function

        <Extension()>
        Public Function AddProductionOverlay(builder As IConfigurationBuilder) As IConfigurationBuilder
            ' Load the canonical operator file when present, otherwise fall back to the
            ' bin-adjacent developer file. Both are optional at the builder level; the caller
            ' (Program.vb) is responsible for failing loudly when neither exists.
            builder.AddJsonFile(GetProductionConfigPath(), optional:=True, reloadOnChange:=True)
            builder.AddJsonFile(GetDeveloperFallbackPath(), optional:=True, reloadOnChange:=True)
            Return builder
        End Function

        Public Function GetMariaDbConnectionString(cfg As IConfiguration, logger As ILogger) As String
            Dim connString = cfg.GetSection("Sync")("MariaDbConnection")

            If String.IsNullOrWhiteSpace(connString) Then
                If logger IsNot Nothing Then
                    logger.LogWarning("Sync:MariaDbConnection is missing or empty. Please configure appsettings.Production.json.")
                End If
                Return connString
            End If

            If Not connString.Contains("Pooling=true", StringComparison.OrdinalIgnoreCase) Then
                connString = connString.TrimEnd(";"c) & ";Pooling=true;Min Pool Size=1;Max Pool Size=100;"
            End If

            Return connString
        End Function

    End Module

End Namespace
