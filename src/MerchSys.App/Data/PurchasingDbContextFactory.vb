Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design
Imports MerchSys.Purchasing.Data

Namespace Data

    Public Class PurchasingDbContextFactory
        Implements IDesignTimeDbContextFactory(Of PurchasingDbContext)

        Public Function CreateDbContext(args As String()) As PurchasingDbContext Implements IDesignTimeDbContextFactory(Of PurchasingDbContext).CreateDbContext
            Dim optionsBuilder = New DbContextOptionsBuilder(Of PurchasingDbContext)()
            Dim connectionString = "Server=127.0.0.1;Port=3306;Database=vista_central;User=root;"
            optionsBuilder.UseMySQL(connectionString)

            Return New PurchasingDbContext(optionsBuilder.Options)
        End Function
    End Class

End Namespace
