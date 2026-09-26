Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Entities
Imports MerchSys.SharedKernel.Data

Namespace Data.SeedData
    Public NotInheritable Class InventorySeedData
        Private Sub New()
        End Sub

        Public Shared Sub Seed(modelBuilder As ModelBuilder)
            Dim fertilizersId As Integer = 1
            Dim pesticidesId As Integer = 2
            Dim seedsId As Integer = 3
            Dim animalFeedsId As Integer = 4

            modelBuilder.Entity(Of ProductCategory)().HasData(
                New ProductCategory With {.Id = fertilizersId, .Name = "Fertilizers"},
                New ProductCategory With {.Id = pesticidesId, .Name = "Pesticides/Chemicals"},
                New ProductCategory With {.Id = seedsId, .Name = "Seeds"},
                New ProductCategory With {.Id = animalFeedsId, .Name = "Animal Feeds"}
            )

            Dim products = New List(Of Product)()

            Dim rand = New Random(12345)
            Dim nextProductId = 1

            For i = 1 To 5
                products.Add(New Product With {
                    .Id = nextProductId,
                    .ProductCategoryId = fertilizersId,
                    .Name = "Fertilizer Product " & i,
                    .SKU = "FERT-" & nextProductId.ToString("D4"),
                    .RetailPrice = CDec(rand.Next(10, 100)),
                    .Unit = "kg",
                    .HasExpiry = True,
                    .MinimumThreshold = 10
                })
                nextProductId += 1
            Next

            For i = 1 To 5
                products.Add(New Product With {
                    .Id = nextProductId,
                    .ProductCategoryId = pesticidesId,
                    .Name = "Pesticide Product " & i,
                    .SKU = "PEST-" & nextProductId.ToString("D4"),
                    .RetailPrice = CDec(rand.Next(15, 150)),
                    .Unit = "L",
                    .HasExpiry = True,
                    .MinimumThreshold = 5
                })
                nextProductId += 1
            Next

            For i = 1 To 5
                products.Add(New Product With {
                    .Id = nextProductId,
                    .ProductCategoryId = seedsId,
                    .Name = "Seed Variety " & i,
                    .SKU = "SEED-" & nextProductId.ToString("D4"),
                    .RetailPrice = CDec(rand.Next(5, 50)),
                    .Unit = "pack",
                    .HasExpiry = False,
                    .MinimumThreshold = 20
                })
                nextProductId += 1
            Next

            For i = 1 To 5
                products.Add(New Product With {
                    .Id = nextProductId,
                    .ProductCategoryId = animalFeedsId,
                    .Name = "Animal Feed " & i,
                    .SKU = "FEED-" & nextProductId.ToString("D4"),
                    .RetailPrice = CDec(rand.Next(20, 200)),
                    .Unit = "bag",
                    .HasExpiry = True,
                    .MinimumThreshold = 15
                })
                nextProductId += 1
            Next

            modelBuilder.Entity(Of Product)().HasData(products)
        End Sub
    End Class
End Namespace
