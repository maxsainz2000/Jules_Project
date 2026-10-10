Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Accounting.Services
Imports System.Runtime.CompilerServices

Namespace Extensions

    Public Module AccountingServiceCollectionExtensions

        <Extension()>
        Public Sub AddAccountingServices(services As IServiceCollection)
            services.AddScoped(Of IFinancialOverviewService, FinancialOverviewService)()
        End Sub

    End Module

End Namespace