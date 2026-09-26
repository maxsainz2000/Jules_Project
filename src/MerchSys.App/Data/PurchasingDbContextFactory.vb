Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design
Imports Microsoft.Extensions.Configuration
Imports MerchSys.Purchasing.Data
Imports System.IO

Namespace Data
    Public Class PurchasingDbContextFactory
        Implements IDesignTimeDbContextFactory(Of PurchasingDbContext)

        Public Function CreateDbContext(args As String()) As PurchasingDbContext Implements IDesignTimeDbContextFactory(Of PurchasingDbContext).CreateDbContext
            Dim basePath = Directory.GetCurrentDirectory()

            Dim configuration = New ConfigurationBuilder().
                SetBasePath(basePath).
                AddJsonFile("appsettings.json", optional:=True, reloadOnChange:=True).
                AddJsonFile("appsettings.Production.json", optional:=True, reloadOnChange:=True).
                Build()

            Dim connectionString = configuration.GetSection("Sync")("MariaDbConnection")

            If String.IsNullOrWhiteSpace(connectionString) Then
                ' Default fallback for EF tool discovery
                connectionString = "Server=127.0.0.1;Port=3306;Database=villon_farm_supply;"
            End If

            Dim builder = New DbContextOptionsBuilder(Of PurchasingDbContext)()
            builder.UseMySQL(connectionString)

            Return New PurchasingDbContext(builder.Options)
        End Function
    End Class
End Namespace
