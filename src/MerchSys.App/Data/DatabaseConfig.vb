Imports System.Runtime.CompilerServices
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Inventory.Data
Imports MerchSys.POS.Data
Imports MerchSys.Accounting.Data
Imports MerchSys.SharedKernel.Data

Namespace Data

    Public Module DatabaseConfig

        <Extension()>
        Public Sub AddModuleDbContexts(services As IServiceCollection, connectionString As String)
            services.AddDbContext(Of PurchasingDbContext)(Sub(sp, opts)
                                                              Dim interceptor = sp.GetRequiredService(Of RoleGuardInterceptor)()
                                                              opts.UseMySQL(connectionString).AddInterceptors(interceptor)
                                                          End Sub)
            services.AddDbContext(Of InventoryDbContext)(Sub(sp, opts)
                                                             Dim interceptor = sp.GetRequiredService(Of RoleGuardInterceptor)()
                                                             opts.UseMySQL(connectionString).AddInterceptors(interceptor)
                                                         End Sub)
            services.AddDbContext(Of POSDbContext)(Sub(sp, opts)
                                                       Dim interceptor = sp.GetRequiredService(Of RoleGuardInterceptor)()
                                                       opts.UseMySQL(connectionString).AddInterceptors(interceptor)
                                                   End Sub)
            services.AddDbContext(Of AccountingDbContext)(Sub(sp, opts)
                                                              Dim interceptor = sp.GetRequiredService(Of RoleGuardInterceptor)()
                                                              opts.UseMySQL(connectionString).AddInterceptors(interceptor)
                                                          End Sub)
        End Sub

    End Module

End Namespace
