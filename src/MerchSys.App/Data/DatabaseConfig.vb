Imports System.Runtime.CompilerServices
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Inventory.Data
Imports MerchSys.POS.Data
Imports MerchSys.Accounting.Data

Namespace Data

    Public Module DatabaseConfig

        <Extension()>
        Public Sub AddModuleDbContexts(services As IServiceCollection, connectionString As String)
            Dim hardcodedConnStr = "Server=127.0.0.1;Port=3306;Database=villon_farm_supply;"
            services.AddDbContext(Of PurchasingDbContext)(Sub(sp, opts)
                                                              Dim interceptor = sp.GetRequiredService(Of MerchSys.SharedKernel.Data.RoleGuardInterceptor)()
                                                              opts.UseMySQL(hardcodedConnStr).AddInterceptors(interceptor)
                                                          End Sub)
            services.AddDbContext(Of InventoryDbContext)(Sub(sp, opts)
                                                             Dim interceptor = sp.GetRequiredService(Of MerchSys.SharedKernel.Data.RoleGuardInterceptor)()
                                                             opts.UseMySQL(hardcodedConnStr).AddInterceptors(interceptor)
                                                         End Sub)
            services.AddDbContext(Of POSDbContext)(Sub(sp, opts)
                                                       Dim interceptor = sp.GetRequiredService(Of MerchSys.SharedKernel.Data.RoleGuardInterceptor)()
                                                       opts.UseMySQL(hardcodedConnStr).AddInterceptors(interceptor)
                                                   End Sub)
            services.AddDbContext(Of AccountingDbContext)(Sub(sp, opts)
                                                              Dim interceptor = sp.GetRequiredService(Of MerchSys.SharedKernel.Data.RoleGuardInterceptor)()
                                                              opts.UseMySQL(hardcodedConnStr).AddInterceptors(interceptor)
                                                          End Sub)
        End Sub

    End Module

End Namespace
