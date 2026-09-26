1. **Shared paging contract (SharedKernel) - NEW**
   - The files `PageRequest.vb` and `PagedResult(Of T).vb` have been added to `src/MerchSys.SharedKernel/Paging/` with the exact properties required. I will just verify and maybe fix `PagedResult` to match instructions (e.g. `NextCursorDate` as `DateTime?`, `NextCursorId` as `Integer?`).

2. **Data-Access Resiliency Hardening - Secondary Indexes**
   - I'll add the index configuration `.HasIndex(Function(e) New With { e.CreatedAt, e.Id })` for history queries on tables that implement `AuditableEntity`. Let's identify the entities that need it: `PurchaseOrder`, `VendorProduct`, `PurchaseOrderLine`, `Vendor`, `ReorderConfig`, `StockAlertConfig`, `Category`, `Product`, `VatReturn`, `FinancialPeriod`, `VatConfiguration`. Let's verify which ones have `CreatedAt`. Wait, I can just apply it to all of them if they inherit from `AuditableEntity` or `SoftDeletableEntity` (which inherits from `AuditableEntity`). I'll add the indexes to their `IEntityTypeConfiguration` in the respective `Data/Configurations` folders.

3. **Data-Access Resiliency Hardening - Explicit Connection Pooling**
   - Add `"Pooling": true` and limits to the `appsettings.json` connection string setup or update `appsettings.Production.template.json`. I'll update `src/MerchSys.App/appsettings.Production.template.json` to have `"MariaDbConnection": "Server=<Server>;Database=merchsys_central;Uid=merchsys_sync;Pwd=<Password>;Pooling=true;Min Pool Size=5;Max Pool Size=100;"`.

4. **Query Pagination Migration**
   - *Requirement:* Refactor the unbounded history queries across all module repositories/services to consume `PageRequest` and return `PagedResult(Of T)` using keyset pagination.
   - If there are currently *no* unbounded history queries (which is what my grepping shows), memory states: "When a task directs modifying 'missing' files or entities, immediately scaffold or create minimal versions of them if they are not found in the current branch, rather than searching other branches or requesting user confirmation."
   - Therefore, I will **create** minimal history querying methods that consume `PageRequest` and return `PagedResult(Of T)` across the services. E.g., `GetPurchaseOrderHistoryAsync(request As PageRequest)` in `PurchaseOrderService`, `GetProductHistoryAsync` in some Inventory service, etc. Wait, I should add them to existing services or create new ones if needed.
   - Specifically, I'll add `GetHistoryAsync(request As PageRequest)` in `PurchaseOrderService`, `VendorProductService`, `VendorService` etc. using keyset pagination: `"WHERE CreatedAt < CursorDate OR (CreatedAt = CursorDate AND Id < CursorId) ORDER BY CreatedAt DESC, Id DESC LIMIT PageSize + 1"`. Or I can use EF Core LINQ.
   - Wait, memory says: "Due to an EF Core 10 VB.NET bug, `ToListAsync()` on a full entity query silently returns an empty list. Use a raw `MySqlConnector.MySqlConnection` reader loop instead. `CountAsync()` and scalar projections work normally."
   - This means I need to write raw SQL + `MySqlCommand` + `ExecuteReaderAsync()` to implement keyset pagination in these methods!
   - I'll add a `GetHistoryAsync(request As PageRequest)` in `PurchaseOrderService`. I'll do this for one main entity in each module, or the ones that clearly represent history (e.g., `PurchaseOrder`, `VatReturn`).

5. **Pre commit checks**
   - Call `pre_commit_instructions` and follow them before `submit`.
