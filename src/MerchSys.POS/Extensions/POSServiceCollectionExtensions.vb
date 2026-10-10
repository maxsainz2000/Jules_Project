Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.POS.Services
Imports MerchSys.POS.Presenters

Namespace Extensions
    Public Module POSServiceCollectionExtensions
        <System.Runtime.CompilerServices.Extension>
        Public Sub AddPOSServices(services As IServiceCollection)
            services.AddScoped(Of ISalesReturnService, SalesReturnService)()
            services.AddScoped(Of IDailySummaryService, DailySummaryService)()
            services.AddScoped(Of ICreditService, CreditService)()
            services.AddScoped(Of IReceiptIntegrityService, ReceiptIntegrityService)()
            services.AddTransient(Of CreditManagementPresenter)()
            services.AddTransient(Of TransactionHistoryPresenter)()
            services.AddTransient(Of DailySummaryPresenter)()
        End Sub
    End Module
End Namespace
