Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Data

Namespace Data

    Public Class POSDbContext
        Inherits BaseDbContext

        Public Sub New(options As DbContextOptions(Of POSDbContext))
            MyBase.New(options)
        End Sub

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.ApplyConfigurationsFromAssembly(GetType(POSDbContext).Assembly)

            For Each entityType In modelBuilder.Model.GetEntityTypes()
                entityType.SetTableName("Pos_" & entityType.GetTableName())
            Next
        End Sub

    End Class

End Namespace
