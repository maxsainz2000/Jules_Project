Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.POS.Services
Imports MerchSys.POS.Presenters
Imports MerchSys.POS.Services.Archival
Imports MerchSys.POS.Services.ReceiptFormatting
Imports Microsoft.Extensions.Configuration

Namespace Extensions
    Public Module POSServiceCollectionExtensions
        <System.Runtime.CompilerServices.Extension>
        Public Sub AddPosModule(services As IServiceCollection)
            AddPOSServices(services)
        End Sub

        <System.Runtime.CompilerServices.Extension>
        Public Sub AddPOSServices(services As IServiceCollection)
            services.AddOptions(Of ReceiptArchivalOptions)().BindConfiguration("Receipts:Archival")
            services.AddScoped(Of IReceiptArchivalService, ReceiptArchivalService)()
            services.AddHostedService(Of ReceiptArchivalService)()
            services.AddScoped(Of ISalesReturnService, SalesReturnService)()
            services.AddScoped(Of IDailySummaryService, DailySummaryService)()
            services.AddScoped(Of ICreditService, CreditService)()
            services.AddScoped(Of IReceiptIntegrityService, ReceiptIntegrityService)()
            services.AddScoped(Of IReceiptBodyComposer, BirCompliantReceiptBodyComposer)()

            services.AddScoped(Of ReceiptService)()
            services.AddScoped(Of IReceiptService, VatAwareReceiptService)()
            services.AddScoped(Of IVatCalculator, VatCalculator)()
            services.AddSingleton(Of VatConfigurationLoader)()
            services.AddScoped(Of IVatConfigurationWriter, VatConfigurationWriter)()

            services.AddTransient(Of CreditManagementPresenter)()
            services.AddTransient(Of TransactionHistoryPresenter)()
            services.AddTransient(Of DailySummaryPresenter)()
            services.AddTransient(Of VatSettingsPresenter)()
        End Sub
    End Module
End Namespace
