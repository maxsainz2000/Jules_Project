Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Inventory.Services

Namespace Startup

    Public Module InventoryConfig

        <System.Runtime.CompilerServices.Extension>
        Public Sub AddInventoryServices(services As IServiceCollection)
            services.AddScoped(Of IStockService, StockService)()
        End Sub

    End Module

End Namespace
