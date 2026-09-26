Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Purchasing.Services

Namespace Startup
    Public Module ServiceCollectionExtensions
        <System.Runtime.CompilerServices.Extension()>
        Public Sub AddPurchasingServices(services As IServiceCollection)
            services.AddScoped(Of PurchaseOrderService)()
            services.AddScoped(Of ReorderService)()
            services.AddScoped(Of VendorProductService)()
            services.AddScoped(Of VendorService)()
        End Sub
    End Module
End Namespace
