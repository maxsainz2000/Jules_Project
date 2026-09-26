Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design
Imports Microsoft.Extensions.Configuration
Imports MerchSys.POS.Data
Imports System.IO

Namespace Data
    Public Class POSDbContextFactory
        Implements IDesignTimeDbContextFactory(Of POSDbContext)

        Public Function CreateDbContext(args As String()) As POSDbContext Implements IDesignTimeDbContextFactory(Of POSDbContext).CreateDbContext
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

            Dim builder = New DbContextOptionsBuilder(Of POSDbContext)()
            builder.UseMySQL(connectionString)

            Return New POSDbContext(builder.Options)
        End Function
    End Class
End Namespace
