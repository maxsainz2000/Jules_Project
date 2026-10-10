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

                        cmd.CommandText = "
                            ALTER TABLE Pos_CreditAccounts
                            ADD COLUMN IF NOT EXISTS TotalCreditExtended DECIMAL(18,2) NOT NULL DEFAULT 0.00,
                            ADD COLUMN IF NOT EXISTS LastTransactionDate DATETIME(6) NULL;
                            ALTER TABLE Pos_SalesTransactionLines
                            ADD COLUMN IF NOT EXISTS ProductId INT NOT NULL DEFAULT 0;
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

                Dim accConn = accountingDb.Database.GetDbConnection()
                Dim accWasClosed = (accConn.State = System.Data.ConnectionState.Closed)
                If accWasClosed Then accConn.Open()
                Try
                    Using cmd = accConn.CreateCommand()
                        cmd.CommandText = "
                            ALTER TABLE Acc_RevenueRecords
                            ADD COLUMN IF NOT EXISTS VatableAmount DECIMAL(18,2) DEFAULT 0,
                            ADD COLUMN IF NOT EXISTS VatExemptAmount DECIMAL(18,2) DEFAULT 0,
                            ADD COLUMN IF NOT EXISTS ZeroRatedAmount DECIMAL(18,2) DEFAULT 0,
                            ADD COLUMN IF NOT EXISTS OutputVat DECIMAL(18,2) DEFAULT 0,
                            ADD COLUMN IF NOT EXISTS InputVat DECIMAL(18,2) DEFAULT 0,
                            ADD COLUMN IF NOT EXISTS VatTreatment VARCHAR(50) NULL;

                            ALTER TABLE Acc_ExpenseRecords
                            ADD COLUMN IF NOT EXISTS VatableAmount DECIMAL(18,2) DEFAULT 0,
                            ADD COLUMN IF NOT EXISTS VatExemptAmount DECIMAL(18,2) DEFAULT 0,
                            ADD COLUMN IF NOT EXISTS ZeroRatedAmount DECIMAL(18,2) DEFAULT 0,
                            ADD COLUMN IF NOT EXISTS OutputVat DECIMAL(18,2) DEFAULT 0,
                            ADD COLUMN IF NOT EXISTS InputVat DECIMAL(18,2) DEFAULT 0,
                            ADD COLUMN IF NOT EXISTS VatTreatment VARCHAR(50) NULL;

                            CREATE TABLE IF NOT EXISTS Acc_VatReturns (
                                Id INT AUTO_INCREMENT PRIMARY KEY,
                                Year INT NOT NULL,
                                Period INT NOT NULL,
                                PeriodType INT NOT NULL,
                                FormType INT NOT NULL,
                                FilingStatus INT NOT NULL,
                                PeriodStart DATETIME(6) NOT NULL,
                                PeriodEnd DATETIME(6) NOT NULL,
                                TotalSales DECIMAL(18,2) NOT NULL DEFAULT 0,
                                TotalPurchases DECIMAL(18,2) NOT NULL DEFAULT 0,
                                OutputVatTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
                                InputVatTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
                                NetVatPayable DECIMAL(18,2) NOT NULL DEFAULT 0,
                                FiledAt DATETIME(6) NULL,
                                FiledBy LONGTEXT NULL,
                                CreatedAt DATETIME(6) NOT NULL,
                                CreatedBy LONGTEXT NULL,
                                ModifiedAt DATETIME(6) NULL,
                                ModifiedBy LONGTEXT NULL,
                                IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
                                RowVersion TIMESTAMP(6) DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)
                            );

                            CREATE UNIQUE INDEX IF NOT EXISTS IX_Acc_VatReturns_Year_Period_PeriodType_FormType
                            ON Acc_VatReturns(Year, Period, PeriodType, FormType);

                            CREATE TABLE IF NOT EXISTS Acc_VatReturnLines (
                                Id INT AUTO_INCREMENT PRIMARY KEY,
                                VatReturnId INT NOT NULL,
                                LineNumber INT NOT NULL,
                                LineCode LONGTEXT NULL,
                                Description LONGTEXT NULL,
                                Amount DECIMAL(18,2) NOT NULL DEFAULT 0,
                                TaxRate DECIMAL(6,4) NOT NULL DEFAULT 0,
                                TaxAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
                                CreatedAt DATETIME(6) NOT NULL,
                                CreatedBy LONGTEXT NULL,
                                ModifiedAt DATETIME(6) NULL,
                                ModifiedBy LONGTEXT NULL,
                                IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
                                CONSTRAINT FK_Acc_VatReturnLines_Acc_VatReturns_VatReturnId FOREIGN KEY (VatReturnId) REFERENCES Acc_VatReturns (Id) ON DELETE CASCADE
                            );

                            CREATE INDEX IF NOT EXISTS IX_Acc_VatReturnLines_VatReturnId ON Acc_VatReturnLines(VatReturnId);
                            CREATE INDEX IF NOT EXISTS IX_Acc_VatReturnLines_LineNumber ON Acc_VatReturnLines(LineNumber);
                        "
                        cmd.ExecuteNonQuery()
                    End Using
                Finally
                    If accWasClosed Then accConn.Close()
                End Try
            End Using
        End Sub

    End Class

End Namespace
