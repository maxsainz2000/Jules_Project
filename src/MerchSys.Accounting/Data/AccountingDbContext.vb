Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Data

Namespace Data

    Public Class AccountingDbContext
        Inherits BaseDbContext

        Public Sub New(options As DbContextOptions(Of AccountingDbContext))
            MyBase.New(options)
        End Sub

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)

            For Each entityType In modelBuilder.Model.GetEntityTypes()
                entityType.SetTableName("Acc_" & entityType.GetTableName())
            Next
        End Sub

    End Class

End Namespace
