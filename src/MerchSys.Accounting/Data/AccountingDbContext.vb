Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Data

Namespace Data

    Public Class AccountingDbContext
        Inherits BaseDbContext

        Public Sub New(options As DbContextOptions(Of AccountingDbContext))
            Me.New(options, Nothing, Nothing)
        End Sub

        Public Sub New(options As DbContextOptions(Of AccountingDbContext), Optional currentUserProvider As MerchSys.SharedKernel.Interfaces.ICurrentUserProvider = Nothing, Optional writeContextScope As MerchSys.SharedKernel.Interfaces.IWriteContextScope = Nothing)
            MyBase.New(options, currentUserProvider, writeContextScope)
        End Sub

        ''' <summary>Tag written into the audit log for records this context writes.</summary>
        Protected Overrides ReadOnly Property AuditModuleTag As String
            Get
                Return "Acc"
            End Get
        End Property

        Public Property FinancialPeriods As DbSet(Of Entities.FinancialPeriod)
        Public Property RevenueRecords As DbSet(Of Entities.RevenueRecord)
        Public Property ExpenseRecords As DbSet(Of Entities.ExpenseRecord)
        Public Property FinancialSnapshots As DbSet(Of Entities.FinancialSnapshot)
        Public Property VatReturns As DbSet(Of Entities.VatReturn)
        Public Property VatReturnLines As DbSet(Of Entities.VatReturnLine)
        Public Overridable Property TamperAuditEntries As DbSet(Of Entities.TamperAuditEntry)

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.ApplyConfigurationsFromAssembly(GetType(AccountingDbContext).Assembly)

            ApplyTablePrefix(modelBuilder, "Acc_")
        End Sub

    End Class

End Namespace
