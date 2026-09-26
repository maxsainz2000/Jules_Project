Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design
Imports Microsoft.Extensions.Configuration
Imports MerchSys.Accounting.Data
Imports System.IO

Namespace Data
    Public Class AccountingDbContextFactory
        Implements IDesignTimeDbContextFactory(Of AccountingDbContext)

        Public Function CreateDbContext(args As String()) As AccountingDbContext Implements IDesignTimeDbContextFactory(Of AccountingDbContext).CreateDbContext
            Dim basePath = Directory.GetCurrentDirectory()

            Dim configuration = New ConfigurationBuilder().
                SetBasePath(basePath).
                AddJsonFile("appsettings.json", optional:=True, reloadOnChange:=True).
                AddJsonFile("appsettings.Production.json", optional:=True, reloadOnChange:=True).
                Build()

            Dim connectionString = configuration.GetSection("Sync")("MariaDbConnection")

            If String.IsNullOrWhiteSpace(connectionString) Then
                connectionString = "Server=127.0.0.1;Port=3306;Database=villon_farm_supply;"
            End If

            Dim builder = New DbContextOptionsBuilder(Of AccountingDbContext)()
            builder.UseMySQL(connectionString)

            Return New AccountingDbContext(builder.Options)
        End Function
    End Class
End Namespace
