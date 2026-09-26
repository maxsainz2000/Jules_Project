Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design
Imports MerchSys.POS.Data

Namespace Data

    Public Class POSDbContextFactory
        Implements IDesignTimeDbContextFactory(Of POSDbContext)

        Public Function CreateDbContext(args As String()) As POSDbContext Implements IDesignTimeDbContextFactory(Of POSDbContext).CreateDbContext
            Dim optionsBuilder = New DbContextOptionsBuilder(Of POSDbContext)()
            Dim connectionString = "Server=127.0.0.1;Port=3306;Database=villon_farm_supply;User=root;"
            optionsBuilder.UseMySQL(connectionString)

            Return New POSDbContext(optionsBuilder.Options)
        End Function
    End Class

End Namespace
