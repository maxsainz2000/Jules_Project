Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Data

Namespace Data

    Public Class AccountingDbContext
        Inherits BaseDbContext

        Public Sub New(options As DbContextOptions(Of AccountingDbContext))
            MyBase.New(options)
        End Sub

        Public Property FinancialPeriods As DbSet(Of Entities.FinancialPeriod)
        Public Property RevenueRecords As DbSet(Of Entities.RevenueRecord)
        Public Property ExpenseRecords As DbSet(Of Entities.ExpenseRecord)
        Public Property FinancialSnapshots As DbSet(Of Entities.FinancialSnapshot)
        Public Property VatReturns As DbSet(Of Entities.VatReturn)
        Public Property VatReturnLines As DbSet(Of Entities.VatReturnLine)

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.ApplyConfigurationsFromAssembly(GetType(AccountingDbContext).Assembly)

            For Each entityType In modelBuilder.Model.GetEntityTypes()
                Dim currentName = entityType.GetTableName()
                If Not currentName.StartsWith("Acc_") Then
                    entityType.SetTableName("Acc_" & currentName)
                End If
            Next
        End Sub

    End Class

End Namespace
