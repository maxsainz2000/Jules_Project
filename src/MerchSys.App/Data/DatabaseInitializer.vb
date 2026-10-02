Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Purchasing.Data
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Inventory.Data
Imports MerchSys.POS.Data
Imports MerchSys.Accounting.Data

Namespace Data

    Public Class DatabaseInitializer

        Public Shared Sub Initialize(serviceProvider As IServiceProvider)
            Using scope = serviceProvider.CreateScope()
                Dim services = scope.ServiceProvider

                ' Resolve each DbContext and ensure the database is created
                Dim purchasingDb = services.GetRequiredService(Of PurchasingDbContext)()
                purchasingDb.Database.EnsureCreated()

                ' Apply manual migrations for Purchasing
                Dim conn = purchasingDb.Database.GetDbConnection()
                Dim wasClosed = (conn.State = System.Data.ConnectionState.Closed)
                If wasClosed Then conn.Open()
                Try
                    Using cmd = conn.CreateCommand()
                        ' 20260516140000_AddGoodsReceiptLineVatColumns
                        cmd.CommandText = "
                            ALTER TABLE Pur_GoodsReceiptLines
                            ADD COLUMN IF NOT EXISTS VatClassification INT NOT NULL DEFAULT 0,
                            ADD COLUMN IF NOT EXISTS VatAmount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
                            ADD COLUMN IF NOT EXISTS VatableSales DECIMAL(18,2) NOT NULL DEFAULT 0.00;
                        "
                        cmd.ExecuteNonQuery()

                        ' 20260527110000_AddVendorProductCatalog
                        cmd.CommandText = "
                            CREATE TABLE IF NOT EXISTS Pur_VendorProducts (
                                Id INT AUTO_INCREMENT PRIMARY KEY,
                                VendorId INT NOT NULL,
                                ProductId INT NOT NULL,
                                UnitCost DECIMAL(18,2) NOT NULL,
                                Notes LONGTEXT,
                                CreatedAt DATETIME(6) NOT NULL,
                                CreatedBy LONGTEXT,
                                ModifiedAt DATETIME(6),
                                ModifiedBy LONGTEXT,
                                IsDeleted TINYINT(1) NOT NULL DEFAULT 0
                            );

                            -- Use CREATE INDEX IF NOT EXISTS which MariaDB supports for checking existing indexes
                            CREATE UNIQUE INDEX IF NOT EXISTS IX_Pur_VendorProducts_VendorId_ProductId
                            ON Pur_VendorProducts (VendorId, ProductId);
                        "
                        cmd.ExecuteNonQuery()
                    End Using
                Finally
                    If wasClosed Then conn.Close()
                End Try

                Dim inventoryDb = services.GetRequiredService(Of InventoryDbContext)()
                inventoryDb.Database.EnsureCreated()

                Dim posDb = services.GetRequiredService(Of POSDbContext)()
                posDb.Database.EnsureCreated()

                Dim accountingDb = services.GetRequiredService(Of AccountingDbContext)()
                accountingDb.Database.EnsureCreated()
            End Using
        End Sub

    End Class

End Namespace
