Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Purchasing.Entities

Namespace Data.SeedData
    Public NotInheritable Class PurchasingSeedData

        Private Sub New()
        End Sub

        Public Shared Sub Seed(modelBuilder As ModelBuilder)
            modelBuilder.Entity(Of Vendor)().HasData(
                New Vendor With {
                    .Id = 1,
                    .Name = "AgriChem Supplies",
                    .LeadTimeDays = 5
                },
                New Vendor With {
                    .Id = 2,
                    .Name = "FarmFresh Seeds Corp.",
                    .LeadTimeDays = 7
                },
                New Vendor With {
                    .Id = 3,
                    .Name = "Golden Feeds Trading",
                    .LeadTimeDays = 3
                }
            )
        End Sub

    End Class
End Namespace
