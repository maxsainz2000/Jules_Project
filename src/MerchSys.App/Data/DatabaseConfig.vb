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
            services.AddDbContext(Of PurchasingDbContext)(Sub(opts) opts.UseMySQL(connectionString))
            services.AddDbContext(Of InventoryDbContext)(Sub(opts) opts.UseMySQL(connectionString))
            services.AddDbContext(Of POSDbContext)(Sub(opts) opts.UseMySQL(connectionString))
            services.AddDbContext(Of AccountingDbContext)(Sub(opts) opts.UseMySQL(connectionString))
        End Sub

    End Module

End Namespace
