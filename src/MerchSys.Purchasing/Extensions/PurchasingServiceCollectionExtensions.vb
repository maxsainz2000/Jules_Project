Imports Microsoft.Extensions.DependencyInjection
Imports System.Runtime.CompilerServices
Imports MerchSys.Purchasing.Services

Namespace Extensions
    Public Module PurchasingServiceCollectionExtensions
        <Extension()>
        Public Sub AddPurchasingServices(services As IServiceCollection)
            services.AddScoped(Of IPurchaseOrderService, PurchaseOrderService)()
        End Sub
    End Module
End Namespace
