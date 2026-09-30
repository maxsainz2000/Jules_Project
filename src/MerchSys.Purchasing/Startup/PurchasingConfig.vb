Imports Microsoft.Extensions.DependencyInjection
Imports System.Runtime.CompilerServices
Imports MerchSys.Purchasing.Services

Namespace Startup
    Public Module PurchasingConfig
        <Extension()>
        Public Sub AddPurchasingServices(services As IServiceCollection)
            services.AddScoped(Of IPurchaseOrderService, PurchaseOrderService)()
            services.AddScoped(Of IVendorService, VendorService)()
            services.AddScoped(Of PurchaseOrderService)()
            services.AddScoped(Of VendorProductService)()
            services.AddScoped(Of ReorderService)()
            services.AddScoped(Of VendorService)()

            services.AddTransient(Of Views.PurchaseOrders.IPurchaseOrderListView, Views.PurchaseOrders.PurchaseOrderListView)()
            services.AddTransient(Of Presenters.PurchaseOrders.PurchaseOrderListPresenter)()
            services.AddTransient(Of Views.Dialogs.IPurchaseOrderEditorView, Views.Dialogs.PurchaseOrderEditorView)()
            services.AddTransient(Of Presenters.PurchaseOrders.PurchaseOrderEditorPresenter)()
        End Sub
    End Module
End Namespace
