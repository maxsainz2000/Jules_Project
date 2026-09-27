1. **Update `PurchasingDbContext.vb`**
   - Add DbSet properties for `Vendor`, `PurchaseOrder`, `PurchaseOrderLine`, `GoodsReceipt`, `GoodsReceiptLine`, and `AccountsPayableEntry`.
   - Wire `PurchasingSeedData.Seed(modelBuilder)` into `OnModelCreating`.
2. **Update/Create EF Core Configurations**
   - **`VendorConfiguration.vb`**: Map to `Vendors` (so it becomes `Pur_Vendors`), set a unique index on `Name`, and configure a cascade delete restriction to `PurchaseOrders`.
   - **`PurchaseOrderConfiguration.vb`**: Map to `PurchaseOrders` (becomes `Pur_PurchaseOrders`), set a unique index on `OrderNumber`, and configure cascade deletes for `Lines` and `GoodsReceipts`. Wait, I need to check if there are navigation properties. If there are no navigation properties (as per memory), I can't configure cascade deletes using EF Core navigation properties on the relationship. Let me re-read the memory: "When referencing entities across different modules or aggregates, use plain integer IDs (e.g., SourcePurchaseOrderId) without creating Entity Framework navigation properties." But inside the *same* module, do they use navigation properties? Let me check `PurchaseOrder.vb` and `Vendor.vb`. They don't have navigation properties. If they don't have navigation properties, EF Core relationships might be configured via `HasMany().WithOne()` or not at all. Let me check the entities again.
   Wait, if there are no navigation properties, I can still configure the foreign keys in the Configuration classes using `HasOne(Of Vendor)().WithMany().HasForeignKey(...)`. I will add this.
   - **`PurchaseOrderLineConfiguration.vb`**: Map to `PurchaseOrderLines` (wait, the prompt says "map to Pur_PurchaseOrderLines", but wait: if the context prepends `Pur_`, then I should map to `PurchaseOrderLines` as per the memory: "Because module table prefixes (e.g., 'Inv_') are dynamically prepended in the OnModelCreating loop of module DbContexts, EF Core builder.ToTable() configurations inside IEntityTypeConfiguration classes must specify the unprefixed table name (e.g., 'Products', not 'Inv_Products')."). So for `PurchaseOrderLine` I will use `.ToTable("PurchaseOrderLines")`. Set precision `(18,4)` on `UnitCost` and `(18,2)` on `LineTotal`.
   - **`GoodsReceiptConfiguration.vb`**: Map to `GoodsReceipts`, set a unique index on `ReceiptNumber`, cascade delete to `Lines`.
   - **`GoodsReceiptLineConfiguration.vb`**: Map to `GoodsReceiptLines`, set precision `(18,4)` on `UnitCost`.
   - **`AccountsPayableConfiguration.vb`**: Map to `AccountsPayable`, set precision `(18,2)` on monetary columns, add composite index on `VendorId` + `IsPaid`.
3. **Add `PurchasingSeedData.vb`**
   - Seed 3 sample vendors: AgriChem Supplies (5-day lead), FarmFresh Seeds Corp. (7-day lead), Golden Feeds Trading (3-day lead).
4. **Complete pre-commit steps**
   - Complete pre-commit steps to ensure proper testing, verification, review, and reflection are done.
5. **Submit changes**
   - Push to branch and submit.
