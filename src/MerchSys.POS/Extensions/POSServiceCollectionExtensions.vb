Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.POS.Services
Imports MerchSys.POS.Presenters
Imports MerchSys.POS.Services.Archival
Imports MerchSys.POS.Services.ReceiptFormatting
Imports MerchSys.POS.Services.ReceiptRendering
Imports Microsoft.Extensions.Configuration
Imports QuestPDF.Infrastructure
Imports System

Namespace Extensions
    Public Module POSServiceCollectionExtensions
        <System.Runtime.CompilerServices.Extension>
        Public Sub AddPosModule(services As IServiceCollection)
            AddPOSServices(services)
        End Sub

        <System.Runtime.CompilerServices.Extension>
        Public Sub AddPOSServices(services As IServiceCollection)
            QuestPDF.Settings.License = LicenseType.Community

            services.AddOptions(Of ReceiptArchivalOptions)().BindConfiguration("Receipts:Archival")
            services.AddOptions(Of ReceiptPdfOptions)().BindConfiguration("POS:Receipt:Pdf")
            services.AddScoped(Of IReceiptArchivalService, ReceiptArchivalService)()
            services.AddHostedService(Of ReceiptArchivalService)()
            services.AddScoped(Of ISalesReturnService, SalesReturnService)()
            services.AddScoped(Of IDailySummaryService, DailySummaryService)()
            services.AddScoped(Of ICreditService, CreditService)()
            services.AddScoped(Of IReceiptIntegrityService, ReceiptIntegrityService)()
            services.AddScoped(Of IReceiptBodyComposer, BirCompliantReceiptBodyComposer)()

            services.AddScoped(Of ConsoleReceiptRenderer)()
            services.AddScoped(Of PdfReceiptRenderer)()

            services.AddScoped(Of IReceiptRenderer)(Function(sp)
                Dim config = sp.GetRequiredService(Of IConfiguration)()
                Dim targetStr = config("POS:Receipt:Target")

                Dim target As ReceiptRenderTarget
                If Not System.Enum.TryParse(targetStr, True, target) Then
                    target = ReceiptRenderTarget.Console
                End If

                If target = ReceiptRenderTarget.Pdf Then
                    Return sp.GetRequiredService(Of PdfReceiptRenderer)()
                End If

                Return sp.GetRequiredService(Of ConsoleReceiptRenderer)()
            End Function)

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
