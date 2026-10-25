# Code Review — Phases 2–5, Non-UI Layers

**Base:** `master` @ `21e24ad`
**Date:** 2026-10-13
**Scope:** data access, services, domain entities, MediatR handlers and integration, security and
authorization, across `MerchSys.SharedKernel`, `MerchSys.Purchasing`, `MerchSys.Inventory`,
`MerchSys.POS`, `MerchSys.Accounting`, `MerchSys.App`.
**Explicitly out of scope:** the WinForms presenter/view layer (Phase 7 UX will rework it, so
reviewing it now means reviewing code that is about to move), and Phases 6–7 which are incomplete
(ACC-12..23, INT-05..24, UX-01..45 are all unstarted).

**Method:** four parallel read-only passes — concurrency/transactions, data access/queries,
security/authorization, architecture/domain — followed by direct verification of every claim that
could be checked against the live database or by probe. Findings are marked **[verified]** where I
reproduced them myself this pass, and **[reported]** where the evidence is a file:line citation I
did not independently re-execute.

---

## Summary

| Severity | Count |
|---|---|
| Critical | 9 |
| High | 8 |
| Medium | 13 |
| Suggestion | 7 |

The headline: **the project's documented concurrency and audit designs are largely not
implemented.** `AGENTS.md` mandates `SELECT ... FOR UPDATE`, "structured audit logging on all
financial state changes", and "never silently overwrite" on concurrency conflicts. None of the
three holds across the codebase.

---

## Critical

### C1. One sale publishes two events; Accounting double-counts revenue **[verified]**
`VatAwareReceiptService.vb:88` publishes `SaleCompletedWithVatEvent` **and** `:102` publishes
`SaleCompletedEvent`. `GoodsReceivingService.vb:105,122` does the same for goods receipts.
Accounting has handlers for **both** of each pair.

The two handlers differ in a way that makes this worse than a duplicate:
- `SaleCompletedWithVatHandler.vb:19-22` is idempotent — it looks up by
  `(SourceTransactionId, ProductId)` and updates if found.
- `SaleCompletedAccountingHandler.vb:37` was **not** — it called
  `_dbContext.RevenueRecords.Add(revenueRecord)` unconditionally, with no lookup.

MediatR gives no handler ordering guarantee. If the legacy handler ran second it inserted a
**second `RevenueRecord`** for the same sale and product, and a second COGS `ExpenseRecord`. There
is no unique index on `(SourceTransactionId, ProductId)` to stop it. Same structure for expenses
via `GoodsReceivedWithVatHandler` / `GoodsReceivedAccountingHandler`.

**Reachability — corrected after tracing the callers.** This is **latent, not live** in the
currently-wired flow. `ReceiptService.GenerateReceiptAsync:48-50` throws
`InvalidOperationException("Transaction already has a receipt.")` when a receipt exists, so a
reprint fails *before* reaching the publish calls at `:88`/`:102`. And the wired checkout calls
`PaymentService.ProcessPaymentAsync` (publishes only the legacy event) — never
`VatAwareReceiptService.GenerateReceiptAsync`. It becomes live the moment the receipt flow is
wired into checkout, or the guard in `ReceiptService` is relaxed.

**Reproduced, not inferred.** Publishing `SaleCompletedEvent` twice against the fixed handler
records one row; against the original handler it recorded **two** (Expected 1, Actual 2). See
`SaleCompletedAccountingTests`.

### C1b. `VatAwareReceiptService` publishes a VAT event with no identity **[verified]**
`VatAwareReceiptService.vb:70-86` builds `SaleCompletedWithVatEvent` without setting `.SaleId` and
without setting `.ProductId` on its items, so both defaulted to `0`. `SaleCompletedWithVatHandler`
upserts on `(SourceTransactionId, ProductId)`, so **every sale's VAT revenue collapsed onto a
single row keyed `(0, 0)`** — the VAT breakdown was overwritten on each sale rather than recorded
per transaction. This one *is* live whenever `GenerateReceiptAsync` runs.

### C2. COGS was always zero, and revenue was never recorded at all **[verified]**
`GetProductCostQueryHandler.vb:12-15` returned `FifoUnitCost = 0D` unconditionally — a stub with no
dependencies at all. Consumed at `SaleCompletedAccountingHandler.vb:28,31`, so every
`RevenueRecord.TotalCogs` and COGS `ExpenseRecord` was zero.

Investigating it surfaced a **larger, adjacent gap**: `RevenueRecord.TotalRevenue`, `UnitPrice` and
`GrossProfit` were **never written by anything**. They are *read* by
`FinancialOverviewService.vb:40-49,70` (Today/MTD/YTD revenue and profit) and
`IncomeStatementPresenter.vb:115` — so every revenue and gross-profit figure in the application
reported zero. Fixing COGS alone would have made gross profit *worse* (0 − COGS), which is why both
had to be fixed together.

**Why the obvious fix was wrong.** The natural repair is to implement the cost query against current
stock. That returns the cost of the *remaining* batches — the next cost, not the consumed one —
which is both systematically biased and non-deterministic, because it depends on whether Inventory
or Accounting happens to handle the event first. It would have looked correct while being wrong.

**The actual fix.** Nothing in the model records which batches a sale consumed, so the true FIFO cost
is knowable *only* at deduction time. `DeductStockFIFOAsync` now accumulates
`deductAmount × batch.UnitCost` and returns it as `FIFODeductionResult.TotalCost`; Inventory
publishes a new `StockDeductedEvent` carrying that cost; `StockDeductedAccountingHandler` books it.
Revenue is carried on `SaleCompletedEvent.SaleCompletedItem.UnitPrice`. The stub, its query and its
result type are deleted.

Both accounting handlers upsert the same `RevenueRecord` and recompute `GrossProfit` from the
persisted row, so the final value is correct whichever runs last. Covered by tests for **both
interleavings**, each also publishing twice to prove idempotency.

### C3. `GetInventoryValuationQuery` has no handler **[reported]**
Sent from `FinancialOverviewService.vb:55,135`; no `IRequestHandler(Of GetInventoryValuationQuery,…)`
exists. `Task.WhenAll` will fault — the Financial Overview screen throws.

### C4. `StockReturnedEvent` has no handler — returns never restock **[verified]**
Published at `SalesReturnService.vb:82-83`; grep finds **no** `INotificationHandler(Of
StockReturnedEvent)` anywhere. A sales return silently does not return stock to inventory.

### C5. FIFO stock deduction is not serialized **[verified]**
`StockService.DeductStockFIFOAsync` (`StockService.vb:30-63`) is an unlocked read-modify-write:
read batches → sum and check → decrement in memory → one `SaveChangesAsync`. No transaction, no
row lock. `SELECT ... FOR UPDATE` — which `AGENTS.md` mandates for exactly this — appears nowhere
in the repository.

Three consequences under 4 concurrent clients:
- Two clients can both pass the sufficiency check.
- The `RowVersion` token *does* catch one (all 12 tokens verified working, see "Verified correct"),
  but `SaleCompletedHandler.vb:26-30` catches only `InsufficientStockException`, so
  `DbUpdateConcurrencyException` propagates **after the POS sale is committed** — the sale succeeds
  and stock silently fails to decrement.
- `SaleCompletedHandler` loops items, each saving independently, so a mid-loop failure leaves a
  partial deduction with no rollback.

Same class in `ShrinkageService.vb:30-91` (unlocked `QuantityRemaining -=`).

### C6. `PurchaseOrder` has no concurrency token; double-close books double liability **[reported]**
`PurchaseOrder` carries no `RowVersion` (only `PurchaseOrderLine` does). `SubmitAsync`,
`ReceiveAsync`, `VerifyAsync` and `CloseAsync` (`PurchaseOrderService.vb:106-152`) are all
`If po.Status <> X Then Throw` followed by a write — check-then-act with no guard.
`CloseAsync:140-149` creates an `AccountsPayableEntry`, so two clients closing the same order book
the liability twice.

### C7. Transaction-number generation is a check-then-act race **[reported]**
`CartService.vb:177-196` reads the max `TX-yyyy-nnnn`, adds one, and inserts at `:110-111`.
`SalesTransactionConfiguration.vb:22-23` puts a UNIQUE index on `TransactionNumber`. Two laptops
finalising simultaneously derive the same number; the unique violation is unhandled and the sale is
lost.

### C8. Receipt integrity hash chain can fork **[reported]**
`ReceiptIntegrityService.vb:40-66` reads the last integrity row and inserts using its hash as
`PreviousHash`. Two concurrent receipts read the same tail and fork the chain — each validates
alone, but `ValidateChainAsync` (`:106`) then reports a break in the tamper-evidence chain.
Related: `:141-168` retries on `DbUpdateConcurrencyException` only, so the first receipt of a new
year (where two clients both `Add` a `ReceiptSequence`) hits a **unique violation it does not
catch** and receipt generation fails outright.

### C9. `ProductId` is never set on sales lines **[reported]**
`CartService.vb:94-101` builds `SalesTransactionLine` without `.ProductId`, leaving the entity
default of 0. Downstream, `PaymentService.vb:71`, `VatAwareReceiptService.vb:97` and
`DailySummaryService.vb:49` all read `ProductId` — so FIFO deduction, COGS and receipt lines are
keyed on product 0. (Reachable once the sales view is wired; `PosPanel` is currently a stub.)

---

## High

### H1. No persistent audit trail, though `AGENTS.md` requires one **[verified]**
"Structured audit logging on all financial state changes" is a stated requirement. There is **no
audit table** in the schema (queried `information_schema.TABLES … LIKE '%audit%'` — empty), and
`0001_initial_schema.sql` creates only `UserAccounts`. `BaseDbContext.vb:110-133` stamps
`CreatedAt/CreatedBy/ModifiedAt/ModifiedBy` on `IAuditable` entities — that is row metadata, not an
audit log. Successful financial operations log nothing: `PaymentService.vb:21-86` has no logger,
`VatReportingService.vb` has none, `SaleCompletedHandler.vb:25` logs only on exception.

**Fixed 2026-10-14 (PR #132, branch `feat/audit-trail`).** `App_AuditLog` is written transactionally by
`BaseDbContext.SaveChangesAsync` for the opt-in `IAuditedEntity` financial records, and is DB-level
immutable via triggers. The write could not be an EF entity (see §3 of the design note — `CreateTables()`
is not idempotent); full design and verification in `Design_Audit_Trail_H1.md`.

### H2. Hardcoded default credentials seeded unconditionally **[reported]**
`MariaDbSchemaInitializer.vb:309` hashes `"VistaDeveloper2026!"` and `:330-331` `"Vista2026!"`.
`EnsureDeveloperAccount` is called at `:56` with **no `#If DEBUG` guard**, so every install —
including production — gets a `developer` account (role 3, a Manager superset) whose password is
committed to the repository. A forced first-login change (`LoginPresenter.vb:33`) narrows but does
not close the window.

### H3. BIR receipt-archive immutability is not active **[verified]**
`02-pos-receipt-integrity-triggers.sql` defines the two `SIGNAL … 'BIR-archive-immutable'` triggers,
but `information_schema.TRIGGERS` for `vista_central` is **empty**. The triggers must be applied
manually after first launch and nothing verifies they exist. NIRC §235 immutability is currently
unenforced.

### H4. Archive rows are editable in application code **[reported]**
`ImmutableReceiptInterceptor.vb:29` guards only `OfficialReceipt` and `ReceiptIntegrity`.
`OfficialReceiptArchive` and `ReceiptIntegrityArchive` are unguarded, so BIR-retention rows can be
modified or deleted through EF.

### H5. VAT rate hardcoded as 1.12 **[Fixed]**
`GoodsReceivingService.vb:63` and `CartService.vb:60` divide by `1.12D` instead of reading
`VatConfiguration`. A rate change requires a code change, and the POS and Purchasing paths cannot
diverge correctly.

### H6. Data-layer role guard has bypass surfaces **[reported]**
`RoleGuardInterceptor.vb:21-36` blocks Owner writes only when `WriteContextKind.User` and only on
`SaveChanges`/`SaveChangesAsync`. `ExecuteUpdateAsync` bypasses `SaveChanges` interception entirely
(used at `ExpiryTrackingService.vb:110-112`). `DataWriteGuard.vb:28` is opt-in and **fail-open**
(`If … Is Nothing Then Return`) and is called from only three sites in `AuthenticationService`.

### H7. Find-or-create against unique indexes without handling the race **[reported]**
`VatReportingService.vb:24-92` (`VatReturnConfiguration.vb:25`),
`FinancialOverviewService.vb:106-147` (`FinancialSnapshotConfiguration.vb:20`) and
`GoodsReceivingService.vb:38-49` (duplicate `ReceiptNumber`) all insert into unique-indexed columns
without catching the resulting violation. Concurrent first-write loses.

### H8. `SaleCompletedAccountingHandler` hardcodes `PaymentMethod.Cash` **[Fixed]**
`SaleCompletedAccountingHandler.vb:40`. Every sale is recorded as cash regardless of actual payment
method, so credit sales are misclassified in the ledger.

---

## Medium

| # | Finding | Location |
|---|---|---|
| M1 | **Shadow column still in existing databases** — `pur_goodsreceiptlines.GoodsReceiptId1` **[verified]**. The model fix is correct (a fresh schema has no shadow column), but `DatabaseBootstrapper` has no migrations and only calls `CreateTables()` on a fully empty database, so the orphan column will never be dropped. Needs a manual `DROP COLUMN`. | live schema |
| M2 | Unlocked read-modify-write on AP balances — `entry.AmountPaid += amount`; the service does not catch the conflict, only `APLedgerPresenter.vb:161` does. `amount` is never checked against the remaining balance. | `AccountsPayableService.vb:36-46` |
| M3 | Failed-login lockout is non-atomic — read `FailedLoginAttempts + 1`, then a raw `UPDATE`; two concurrent bad logins both write 1, defeating the 5-attempt lockout. | `AuthenticationService.vb:79-91` |
| M4 | `ExecuteUpdateAsync` skips audit stamping and the concurrency token. | `ExpiryTrackingService.vb:110-112` |
| M5 | Expired-batch read happens **outside** the transaction that writes; the `ShrinkageRecord` value is computed from the stale quantity. | `ExpiryTrackingService.vb:87-115` |
| M6 | Three independent `SaveChanges` for one receipt (VAT snapshot, receipt, integrity) with no transaction — partial state if a later step fails. | `VatAwareReceiptService.vb:61-67` |
| M7 | N+1: `FindAsync` per batch; mediator query per line; per-item service calls in event handlers. | `ShrinkageService.vb:52-57`, `SaleCompletedAccountingHandler.vb:22-24`, `SaleCompletedWithVatHandler.vb:19-22`, `GoodsReceivedHandler.vb:18-19`, `SaleCompletedHandler.vb:21-23` |
| M8 | Keyset pagination has no supporting index — `Vendor`/`VendorProduct` do not implement `IPaginatedEntity`, so `ApplySecondaryIndexes` never adds the `(CreatedAt, Id)` index the paging predicate seeks on. Confirmed absent from the live schema. | `VendorService.vb:42-46`, `VendorProductService.vb:50-54` |
| M9 | Unbounded grid query — `Include(Lines).Include(Receipt)` then `ToListAsync()` with no `Take`. | `TransactionHistoryPresenter.vb:51,75` |
| M10 | 28 tables have no `IsDeleted` **[verified]**, including financial/inventory records that `AGENTS.md` says must be soft-deletable: `inv_stockbatches`, `inv_shrinkagerecords`, `acc_revenuerecords`, `acc_expenserecords`, `pur_goodsreceipts`, `pur_accountspayable`, `pos_salesreturns`, `pur_goodsreceiptlines`, `pos_salestransactionlines`. (Receipt and archive tables are legitimately exempt — they are immutable by design.) | live schema |
| M11 | Six tables lack audit columns: `pos_receiptsequence`, `pos_vatconfigurations`, `pur_reorderconfigs`, `inv_productpricehistory`, `pos_officialreceiptarchive`, `pos_receiptintegrityarchive`. | live schema |
| M12 | PO number generation sorts `OrderNumber` as a **string** then `Long.TryParse`s it, but numbers are written as `"PO-YYYY-NNNN"` — the parse yields 0, so numbering can duplicate. | `ReorderService.vb:92-102, 216-227` |
| M13 | Sales-return quantity is only compared to the original line quantity, with no record of prior returns — repeatable over-refund. `CreditAccount.CurrentBalance -=` is unlocked. | `SalesReturnService.vb:75-96` |

---

## Suggestions

- `VatConfigurationLoader` caches the rate per process and `Invalidate()` is local only — the other
  three laptops keep a stale rate until restart.
- `AsNoTracking` appears exactly once in `src` (`GetVatConfigurationQueryHandler.vb:18`); every paged
  read path tracks entities it never writes.
- Client-side aggregation of whole date ranges: `FinancialOverviewService.vb:33-35,79-81` loads YTD
  twice; `SalesSummaryService.vb:19-21,119-121`. Push to server-side `Sum`/`GroupBy`.
- Redundant `Where(Not p.IsDeleted)` where the global filter already applies, plus loading all
  Products on the UI thread with no `Take`. — `ProductManagementPresenter.vb:83-102`
- MVP violation: presenter injects a `DbContext` and references
  `System.Windows.Forms.IWin32Window` directly. — `ProductManagementPresenter.vb:33,56-57`
- `ReceiptArchivalService.vb:87` sets `.TransactionId = 0`, losing the archive → sale link.
- No tests cover handlers, FIFO, VAT or receipt numbering — the four areas with the most defects.
- Index gaps: `inv_shrinkagerecords` has PK only though history sorts by `CreatedAt`;
  `inv_productpricehistory` indexes `(ChangedAt, Id)` but queries filter `ProductId`.

---

## Verified correct (coverage)

Checked this pass and found sound — recorded so the next reviewer does not redo it:

- **All 12 concurrency tokens genuinely auto-update** — every `RowVersion` column in `vista_central`
  carries `ON UPDATE CURRENT_TIMESTAMP(6)` **[verified]**. The token mechanism works; the defects
  above are about *handling* the exception it raises, not detecting it.
- **Module boundaries hold** — every `.vbproj`: modules reference `SharedKernel` only, `App`
  references all five; no cross-module `Imports` anywhere in the tree; and **no cross-module foreign
  keys** exist in the live schema **[verified]**.
- **No injection surface** — no string-concatenated SQL anywhere; the four by-design raw-ADO files
  are fully parameterised; embedded `.sql` resources are compiled, SHA-256 hash-checked
  (`MariaDbSchemaInitializer.vb:38-46`) and not user-influenceable.
- **Argon2id is correctly implemented** — m=19456 KiB, t=2, p=1, 16-byte random salt, 32-byte hash,
  constant-time comparison, PHC round-trip (`PasswordHashHelper.vb:18-98`). Meets OWASP guidance.
- **Sessions** — genuinely in-memory, 20-minute timeout within the mandated 15–30, cleared on
  logout and timeout.
- **`FindAsync` honours the soft-delete filter** — verified by probe against EF Core 10 **[verified]**.
  The write paths that use `FindAsync` cannot modify soft-deleted rows.
- **No secrets committed** — `appsettings.Production.json` is untracked; no DB password in the repo;
  the connection string is never logged or displayed.
- **All package versions pinned** — no wildcards in any `.vbproj`.
- **Relationship mappings all name their navigation** — the shadow-column bug class is closed in the
  model (10 relationships checked); only the legacy column in M1 remains.
- **Transactional paths that are correct** — `ReceiptArchivalService.vb:75-123` (real transaction
  with rollback and a correctly reset interceptor bypass), `MariaDbSchemaInitializer.vb:94-118`
  (transactional DDL).
- **FIFO ordering itself is right** — `OrderBy(ReceiptDate)` ascending with correct partial-batch
  deduction; the problem is the missing lock, not the algorithm.
- **VAT rounding is consistent** — `MidpointRounding.ToEven` in both POS and Purchasing.
- **`VatConfigurationLoader`** correctly uses `IServiceScopeFactory` — no captive `DbContext`.
- **Receipt numbering format** is correct: `OR-{year}-{seq:D4}`.

---

## Not verified — needs runtime confirmation

- **C1's actual outcome depends on MediatR handler ordering**, which is not guaranteed. The
  double-insert is provable from the code (`SaleCompletedAccountingHandler` has no lookup), but I did
  not execute two handlers against one event to observe it. Worth a test.
- **`PaymentService` and `VatAwareReceiptService` both publish `SaleCompletedEvent`.** If both run
  for one sale, stock is deducted twice. I could not confirm from the code whether the shipped flow
  calls both — `PosPanel` is a stub, so the cart/checkout composition is not yet wired.
- **CVE status of pinned packages** — no advisory lookup was performed.
- **MariaDB 11.4 LTS** — everything above was verified on XAMPP **10.4.32**. `AGENTS.md` specifies
  11.4.x LTS, which has still never been exercised.

---

## Recommended remediation order

1. **C1** (double-counted revenue) — corrupts financial data and is non-deterministic.
2. **C2, C3, C4, C9** — the module is not functionally complete without them; C3 throws outright.
3. **C5, C6, C7, C8** — the concurrency class. Fix as one design change: a shared
   transaction-and-lock helper plus a single concurrency-conflict policy, rather than eight local
   patches. `AGENTS.md` already specifies the design (`SELECT ... FOR UPDATE`, and the mandated
   "Data changed elsewhere — refresh and retry" message).
4. **H2** (committed credentials) — trivial to fix, unacceptable to ship.
5. **H1, H3, H4** — the audit and BIR-compliance controls. H3 is a one-command fix plus a startup
   verification so it cannot silently regress.
6. **H5, H6, H7, H8**, then the Medium and Suggestion tiers.

**Process note.** Every one of these sits in a phase the roadmap marks complete. Phase 1 was 100%
checked *and* independently reviewed and still yielded six defects; this pass found 37 more in
Phases 2–5. The checkmarks track *delivered*, not *verified* — and the two areas with the most
findings (concurrency and audit) are precisely the two where `AGENTS.md` is most specific about what
was required.
