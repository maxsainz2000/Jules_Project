Imports Microsoft.Extensions.DependencyInjection
Imports System.Runtime.CompilerServices
Imports MerchSys.Purchasing.Services

Namespace Extensions
    Public Module PurchasingServiceCollectionExtensions
        <Extension()>
        Public Sub AddPurchasingServices(services As IServiceCollection)
            services.AddScoped(Of IPurchaseOrderService, PurchaseOrderService)()
            services.AddScoped(Of IPriceChangeService, PriceChangeService)()
            services.AddScoped(Of IGoodsReceivingService, GoodsReceivingService)()
            services.AddScoped(Of IVendorService, VendorService)()
            services.AddScoped(Of IAccountsPayableService, AccountsPayableService)()
            services.AddScoped(Of IReorderService, ReorderService)()
        End Sub
    End Module
End Namespace