Imports Microsoft.Extensions.DependencyInjection
Imports System.Runtime.CompilerServices
Imports MerchSys.Purchasing.Services

Namespace Startup
    Public Module PurchasingConfig
        <Extension()>
        Public Sub AddPurchasingServices(services As IServiceCollection)
            services.AddScoped(Of PurchaseOrderService)()
            services.AddScoped(Of VendorProductService)()
            services.AddScoped(Of ReorderService)()
            services.AddScoped(Of VendorService)()
        End Sub
    End Module
End Namespace
