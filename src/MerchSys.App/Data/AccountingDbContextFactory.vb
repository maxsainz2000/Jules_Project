Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design
Imports MerchSys.Accounting.Data

Namespace Data

    Public Class AccountingDbContextFactory
        Implements IDesignTimeDbContextFactory(Of AccountingDbContext)

        Public Function CreateDbContext(args As String()) As AccountingDbContext Implements IDesignTimeDbContextFactory(Of AccountingDbContext).CreateDbContext
            Dim optionsBuilder = New DbContextOptionsBuilder(Of AccountingDbContext)()
            Dim connectionString = "Server=127.0.0.1;Port=3306;Database=villon_farm_supply;User=root;"
            optionsBuilder.UseMySQL(connectionString)

            Return New AccountingDbContext(optionsBuilder.Options)
        End Function
    End Class

End Namespace
