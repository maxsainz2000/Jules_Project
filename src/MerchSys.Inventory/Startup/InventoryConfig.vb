Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Inventory.Services

Namespace Startup

    Public Module InventoryConfig

        <System.Runtime.CompilerServices.Extension>
        Public Sub AddInventoryServices(services As IServiceCollection)
            services.AddScoped(Of IStockService, StockService)()
            services.AddScoped(Of IExpiryTrackingService, ExpiryTrackingService)()
            services.AddScoped(Of IStockDashboardService, StockDashboardService)()
            services.AddScoped(Of ILowStockAlertService, LowStockAlertService)()
            services.AddScoped(Of IShrinkageService, ShrinkageService)()
        End Sub

    End Module

End Namespace
