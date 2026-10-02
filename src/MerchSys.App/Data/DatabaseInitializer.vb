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

                        cmd.CommandText = "
                            CREATE TABLE IF NOT EXISTS Pur_VendorProducts (
                                Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                                VendorId INT NOT NULL,
                                ProductId INT NOT NULL,
                                UnitCost DECIMAL(18,2) NOT NULL,
                                Notes LONGTEXT,
                                CreatedAt DATETIME(6) NOT NULL,
                                CreatedBy LONGTEXT,
                                ModifiedAt DATETIME(6),
                                ModifiedBy LONGTEXT,
                                IsDeleted TINYINT(1) NOT NULL
                            );
                        "
                        cmd.ExecuteNonQuery()

                        ' Create composite unique index manually if not exists
                        cmd.CommandText = "
                            SELECT COUNT(1) INTO @IndexExists
                            FROM INFORMATION_SCHEMA.STATISTICS
                            WHERE table_schema = DATABASE()
                              AND table_name = 'Pur_VendorProducts'
                              AND index_name = 'IX_Pur_VendorProducts_VendorId_ProductId';

                            SET @query = IF(@IndexExists = 0, 'CREATE UNIQUE INDEX IX_Pur_VendorProducts_VendorId_ProductId ON Pur_VendorProducts (VendorId, ProductId)', 'SELECT 1');
                            PREPARE stmt FROM @query;
                            EXECUTE stmt;
                            DEALLOCATE PREPARE stmt;
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
