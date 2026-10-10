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
        Public Property ReceiptIntegrities As DbSet(Of Entities.ReceiptIntegrity)
        Public Property ReceiptSequences As DbSet(Of Entities.ReceiptSequence)
        Public Property OfficialReceiptArchives As DbSet(Of Entities.OfficialReceiptArchive)
        Public Property ReceiptIntegrityArchives As DbSet(Of Entities.ReceiptIntegrityArchive)
        Public Property VatConfigurations As DbSet(Of Entities.VatConfiguration)

        Public Sub New(options As DbContextOptions(Of POSDbContext))
            MyBase.New(options)
        End Sub

        Protected Overrides Sub OnConfiguring(optionsBuilder As DbContextOptionsBuilder)
            MyBase.OnConfiguring(optionsBuilder)
            If Not optionsBuilder.IsConfigured Then
                ' Used mostly for design time or fallback
            End If
            optionsBuilder.AddInterceptors(New Interceptors.ImmutableReceiptInterceptor())
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
