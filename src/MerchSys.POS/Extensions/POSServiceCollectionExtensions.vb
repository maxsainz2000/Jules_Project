Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.POS.Services

Namespace Extensions
    Public Module POSServiceCollectionExtensions
        <System.Runtime.CompilerServices.Extension>
        Public Sub AddPOSServices(services As IServiceCollection)
            services.AddScoped(Of ISalesReturnService, SalesReturnService)()
            services.AddScoped(Of IDailySummaryService, DailySummaryService)()
        End Sub
    End Module
End Namespace
