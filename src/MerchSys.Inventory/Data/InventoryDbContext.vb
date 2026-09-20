Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Data

Namespace Data

    Public Class InventoryDbContext
        Inherits BaseDbContext

        Public Sub New(options As DbContextOptions(Of InventoryDbContext))
            MyBase.New(options)
        End Sub

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)

            For Each entityType In modelBuilder.Model.GetEntityTypes()
                entityType.SetTableName("Inv_" & entityType.GetTableName())
            Next
        End Sub

    End Class

End Namespace
