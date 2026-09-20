Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging
Imports MerchSys.App.Configuration

Namespace Startup

    Public Module SyncConfig
        Public Sub AddSyncServices(services As IServiceCollection, cfg As IConfiguration, logger As ILogger)
            Dim connString = ConnectionStringLoader.GetMariaDbConnectionString(cfg, logger)

            ' Comment out SyncWorker and SyncJournalDbContext
            ' services.AddHostedService(Of SyncWorker)()
            ' services.AddDbContext(Of SyncJournalDbContext)(Sub(opts) opts.UseMySql(connString, ServerVersion.AutoDetect(connString)))
        End Sub
    End Module

End Namespace
