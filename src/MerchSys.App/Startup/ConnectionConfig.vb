Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.App.Services
Imports MerchSys.App.Presenters
Imports MerchSys.App.Views

Namespace Startup
    Public Module ConnectionConfig
        <System.Runtime.CompilerServices.Extension>
        Public Function AddConnectionHealthMonitor(services As IServiceCollection) As IServiceCollection
            services.AddSingleton(Of IConnectionHealthMonitor, ConnectionHealthMonitor)()
            services.AddTransient(Of ConnectionStatusIndicator)()
            services.AddTransient(Of ConnectionStatusPresenter)()
            Return services
        End Function
    End Module
End Namespace