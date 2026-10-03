Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Data

Namespace Data

    Public Class POSDbContext
        Inherits BaseDbContext

        Public Property SalesTransactions As DbSet(Of Entities.SalesTransaction)
        Public Property SalesTransactionLines As DbSet(Of Entities.SalesTransactionLine)
        Public Property OfficialReceipts As DbSet(Of Entities.OfficialReceipt)
        Public Property CreditAccounts As DbSet(Of Entities.CreditAccount)
        Public Property CreditPayments As DbSet(Of Entities.CreditPayment)
        Public Property SalesReturns As DbSet(Of Entities.SalesReturn)

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
