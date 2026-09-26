Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Inventory.Services
Imports MerchSys.Inventory.Views
Imports MerchSys.Inventory.Presenters

Namespace Startup

    Public Module InventoryConfig

        <System.Runtime.CompilerServices.Extension>
        Public Sub AddInventoryServices(services As IServiceCollection)
            services.AddScoped(Of IStockService, StockService)()
            services.AddScoped(Of IExpiryTrackingService, ExpiryTrackingService)()
            services.AddScoped(Of IStockDashboardService, StockDashboardService)()
            services.AddScoped(Of ILowStockAlertService, LowStockAlertService)()
            services.AddScoped(Of IShrinkageService, ShrinkageService)()
            services.AddScoped(Of IVelocityService, VelocityService)()
            services.AddScoped(Of IStockoutEstimationService, StockoutEstimationService)()

            ' MVP Registration for Stock Dashboard
            services.AddTransient(Of IStockDashboardView, StockDashboardView)()
            services.AddTransient(Of StockDashboardPresenter)()

            ' MVP Registration for Product Management
            services.AddTransient(Of IProductManagementView, ProductManagementView)()
            services.AddTransient(Of ProductManagementPresenter)()
        End Sub

    End Module

End Namespace
