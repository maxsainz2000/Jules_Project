Imports System
Imports System.Runtime.CompilerServices
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Data
Imports MerchSys.Inventory.Data
Imports MerchSys.POS.Data
Imports MerchSys.Accounting.Data

Namespace Data

    Public Module DatabaseConfig

        ''' <summary>
        ''' Registers the four module DbContexts against a single, caller-supplied MariaDB
        ''' connection string. The connection string MUST be the already-resolved,
        ''' pooling-enabled value (see <see cref="Configuration.ConnectionStringLoader"/>);
        ''' this method never invents or hardcodes a server/database of its own, so every
        ''' EF context and every raw-ADO service target the same database.
        ''' </summary>
        <Extension()>
        Public Sub AddModuleDbContexts(services As IServiceCollection, connectionString As String)
            If String.IsNullOrWhiteSpace(connectionString) Then
                Throw New ArgumentException(
                    "A non-empty MariaDB connection string is required to register the module DbContexts.",
                    NameOf(connectionString))
            End If

            services.AddDbContext(Of PurchasingDbContext)(Sub(sp, opts)
                                                              Dim interceptor = sp.GetRequiredService(Of MerchSys.SharedKernel.Data.RoleGuardInterceptor)()
                                                              opts.UseMySQL(connectionString).AddInterceptors(interceptor)
                                                          End Sub)
            services.AddDbContext(Of InventoryDbContext)(Sub(sp, opts)
                                                             Dim interceptor = sp.GetRequiredService(Of MerchSys.SharedKernel.Data.RoleGuardInterceptor)()
                                                             opts.UseMySQL(connectionString).AddInterceptors(interceptor)
                                                         End Sub)
            services.AddDbContext(Of POSDbContext)(Sub(sp, opts)
                                                       Dim interceptor = sp.GetRequiredService(Of MerchSys.SharedKernel.Data.RoleGuardInterceptor)()
                                                       opts.UseMySQL(connectionString).AddInterceptors(interceptor)
                                                   End Sub)
            services.AddDbContext(Of AccountingDbContext)(Sub(sp, opts)
                                                              Dim interceptor = sp.GetRequiredService(Of MerchSys.SharedKernel.Data.RoleGuardInterceptor)()
                                                              opts.UseMySQL(connectionString).AddInterceptors(interceptor)
                                                          End Sub)
        End Sub

    End Module

End Namespace
