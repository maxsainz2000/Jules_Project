Imports System

Namespace Configuration

    ''' <summary>
    ''' Validates the configuration the application needs before it can start.
    '''
    ''' Deliberately pure, so it can be unit-tested without a host or a UI. <c>Program.Main</c>
    ''' must be able to decide whether to show the operator dialog <b>before</b> it builds the
    ''' host: a missing connection string otherwise throws from inside <c>ConfigureServices</c>
    ''' while <c>Build()</c> is running, and the dialog never gets a chance to appear (C4/M15).
    ''' </summary>
    Public Module StartupConfigValidator

        ''' <summary>
        ''' Returns the message to show the operator when the resolved MariaDB connection string
        ''' is missing or blank, or <c>Nothing</c> when it is usable.
        ''' </summary>
        Public Function ValidateConnectionString(connectionString As String) As String
            If Not String.IsNullOrWhiteSpace(connectionString) Then Return Nothing

            Return "The production database configuration could not be found." & Environment.NewLine &
                   Environment.NewLine &
                   "Expected a configuration file at:" & Environment.NewLine &
                   ConnectionStringLoader.GetProductionConfigPath() & Environment.NewLine &
                   Environment.NewLine &
                   "Copy appsettings.Production.template.json to that location, fill in the real " &
                   "server host and credentials, then start the application again."
        End Function

    End Module

End Namespace
