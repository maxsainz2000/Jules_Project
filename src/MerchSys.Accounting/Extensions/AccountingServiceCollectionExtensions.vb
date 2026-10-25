Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Presenters
Imports System.Runtime.CompilerServices

Namespace Extensions

    Public Module AccountingServiceCollectionExtensions

        <Extension()>
        Public Sub AddAccountingServices(services As IServiceCollection)
            services.AddScoped(Of ITamperAuditQueryService, TamperAuditQueryService)()
            services.AddScoped(Of FinancialOverviewService)()
            services.AddScoped(Of IFinancialOverviewService)(Function(sp)
                                                                 Dim inner = sp.GetRequiredService(Of FinancialOverviewService)()
                                                                 Dim kpis = sp.GetServices(Of IKpiProvider)()
                                                                 Return New VatEnrichedFinancialOverviewService(inner, kpis)
                                                             End Function)
            services.AddScoped(Of IKpiProvider, VatPayableKpiProvider)()
            services.AddScoped(Of Insights.IFinancialInsightProvider, Insights.VatPayableInsightProvider)()
            services.AddScoped(Of IIncomeStatementService, IncomeStatementService)()
            services.AddScoped(Of ISalesSummaryService, SalesSummaryService)()
            services.AddScoped(Of IWhatThisMeansService, WhatThisMeansService)()
            services.AddScoped(Of IVatReportingService, VatReportingService)()
            services.AddScoped(Of IVatReturnExporter, VatReturnExporter)()
            services.AddTransient(Of FinancialOverviewPresenter)()
            services.AddTransient(Of IncomeStatementPresenter)()
            services.AddTransient(Of SalesSummaryPresenter)()
            services.AddTransient(Of VatReturnPresenter)()
        End Sub

    End Module

End Namespace