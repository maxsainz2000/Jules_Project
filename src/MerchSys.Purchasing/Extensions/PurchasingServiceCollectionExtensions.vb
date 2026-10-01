Imports Microsoft.Extensions.DependencyInjection
Imports System.Runtime.CompilerServices
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.Purchasing.Views.Dialogs
Imports MerchSys.Purchasing.Presenters

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
            
            ' Views
            services.AddTransient(Of IPurchaseOrderListView, PurchaseOrderListView)()
            services.AddTransient(Of IPurchaseOrderEditorView, PurchaseOrderEditorDialog)()
            
            ' Presenters
            services.AddTransient(Of PurchaseOrderListPresenter)()
            services.AddTransient(Of PurchaseOrderEditorPresenter)()
        End Sub
    End Module
End Namespace