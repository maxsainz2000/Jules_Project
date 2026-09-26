Imports Microsoft.Extensions.DependencyInjection
Imports MediatR
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.Purchasing.Data
Imports MerchSys.Inventory.Data
Imports MerchSys.POS.Data
Imports MerchSys.Accounting.Data

Namespace Startup

    Public Module MediatRConfig

        <System.Runtime.CompilerServices.Extension()>
        Public Sub AddMediatRServices(services As IServiceCollection)
            services.AddMediatR(Sub(cfg)
                                    cfg.RegisterServicesFromAssemblies(
                                        GetType(MerchSys.App.Views.MainWindow).Assembly,
                                        GetType(IEventBus).Assembly,
                                        GetType(PurchasingDbContext).Assembly,
                                        GetType(InventoryDbContext).Assembly,
                                        GetType(POSDbContext).Assembly,
                                        GetType(AccountingDbContext).Assembly
                                    )
                                End Sub)
        End Sub

    End Module

End Namespace
