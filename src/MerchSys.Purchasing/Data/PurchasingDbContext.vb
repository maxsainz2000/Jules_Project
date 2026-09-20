Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Data

Namespace Data

    Public Class PurchasingDbContext
        Inherits BaseDbContext

        Public Sub New(options As DbContextOptions(Of PurchasingDbContext))
            MyBase.New(options)
        End Sub

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)

            For Each entityType In modelBuilder.Model.GetEntityTypes()
                entityType.SetTableName("Pur_" & entityType.GetTableName())
            Next
        End Sub

    End Class

End Namespace
