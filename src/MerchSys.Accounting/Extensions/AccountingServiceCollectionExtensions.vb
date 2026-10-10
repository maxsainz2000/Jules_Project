Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Accounting.Services
Imports MerchSys.Accounting.Presenters
Imports System.Runtime.CompilerServices

Namespace Extensions

    Public Module AccountingServiceCollectionExtensions

        <Extension()>
        Public Sub AddAccountingServices(services As IServiceCollection)
            services.AddScoped(Of IFinancialOverviewService, FinancialOverviewService)()
            services.AddScoped(Of IIncomeStatementService, IncomeStatementService)()
            services.AddScoped(Of ISalesSummaryService, SalesSummaryService)()
            services.AddScoped(Of IWhatThisMeansService, WhatThisMeansService)()
            services.AddTransient(Of FinancialOverviewPresenter)()
            services.AddTransient(Of IncomeStatementPresenter)()
            services.AddTransient(Of SalesSummaryPresenter)()
        End Sub

    End Module

End Namespace