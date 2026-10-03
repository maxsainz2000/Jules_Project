Imports System
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
Imports MerchSys.POS.Entities

Namespace Data.Configurations

    Public Class CreditAccountConfiguration
        Implements IEntityTypeConfiguration(Of CreditAccount)

        Public Sub Configure(builder As EntityTypeBuilder(Of CreditAccount)) Implements IEntityTypeConfiguration(Of CreditAccount).Configure
            builder.ToTable("Pos_CreditAccounts")

            For Each prop In builder.Metadata.GetProperties().Where(Function(p) p.ClrType = GetType(Decimal) OrElse p.ClrType = GetType(Decimal?))
                prop.SetPrecision(18)
                prop.SetScale(2)
            Next

            builder.Property(Function(e) e.CustomerName) _
                .IsRequired() _
                .HasMaxLength(200)

            builder.Property(Function(e) e.CurrentBalance) _
                .HasPrecision(18, 2)

            builder.HasIndex(Function(e) e.IsBlocked)

            ' Seed three sample accounts
            builder.HasData(
                New CreditAccount With {
                    .Id = 1,
                    .CustomerName = "John Doe",
                    .CurrentBalance = 150.50D,
                    .IsBlocked = True,
                    .CreatedAt = DateTime.UtcNow,
                    .CreatedBy = "System",
                    .IsDeleted = False
                },
                New CreditAccount With {
                    .Id = 2,
                    .CustomerName = "Jane Smith",
                    .CurrentBalance = 0D,
                    .IsBlocked = False,
                    .CreatedAt = DateTime.UtcNow,
                    .CreatedBy = "System",
                    .IsDeleted = False
                },
                New CreditAccount With {
                    .Id = 3,
                    .CustomerName = "Juan Dela Cruz",
                    .CurrentBalance = 5000.00D,
                    .IsBlocked = True,
                    .CreatedAt = DateTime.UtcNow,
                    .CreatedBy = "System",
                    .IsDeleted = False
                }
            )
        End Sub
    End Class

End Namespace
