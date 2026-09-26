Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design
Imports MerchSys.Inventory.Data

Namespace Data

    Public Class InventoryDbContextFactory
        Implements IDesignTimeDbContextFactory(Of InventoryDbContext)

        Public Function CreateDbContext(args As String()) As InventoryDbContext Implements IDesignTimeDbContextFactory(Of InventoryDbContext).CreateDbContext
            Dim optionsBuilder = New DbContextOptionsBuilder(Of InventoryDbContext)()
            Dim connectionString = "Server=127.0.0.1;Port=3306;Database=villon_farm_supply;User=root;"
            optionsBuilder.UseMySQL(connectionString)

            Return New InventoryDbContext(optionsBuilder.Options)
        End Function
    End Class

End Namespace
