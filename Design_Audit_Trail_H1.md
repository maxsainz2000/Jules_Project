# Design Note — Persistent Audit Trail (H1)

**Status: IMPLEMENTED (A5 resolved to (a) raw ADO). No UI — see non-goals.**

Phase: **Define & Plan**. H1 is a new cross-cutting capability with a schema change, so it gets a spec
before code, as the concurrency cluster (C5–C8) did.

Evidence baseline: `master` @ `79d3881`. Findings referenced from `CodeReview_Phases2-5_NonUI.md`.

---

## 1. Objective

AGENTS.md requires **"structured audit logging on all financial state changes"**. Today there is no
audit log at all — the requirement is unimplemented, and the review verified it (`H1`, tagged
*verified*). Build the log: an append-only record of **who changed which financial record, when, and
from what to what**, written in the same transaction as the change so it cannot disagree with it.

Secondary objective: the log is a prerequisite for **T9** (D1(b) — compensating for the sale↔stock
window), which needs somewhere to record a failed stock movement.

## 2. Verified current behaviour

- **No audit table exists.** `0001_initial_schema.sql` creates only `UserAccounts`; the review's
  `information_schema.TABLES … LIKE '%audit%'` query returned empty.
- **`BaseDbContext.vb:110-133` stamps row metadata, not a log.** It records the *current* state of a
  row. It cannot answer "what was the value before", and a delete erases the row and its metadata
  together.
- **Successful financial operations log nothing.** `PaymentService` has no logger;
  `VatReportingService` has none; `SaleCompletedHandler` logs only on exception.
- **The pieces for a log already exist:** `ICurrentUserProvider` (who — already resolved in
  `BaseDbContext.ResolveActor`), `IWriteContextScope`/`WriteContextKind` (System vs User), and
  `RoleGuardInterceptor` as the precedent for a data-layer interceptor.

## 3. Why the obvious mechanism does not work — found during implementation

The first design mapped `AuditLog` as an ordinary entity in all four module contexts. **That cannot
work**, and the reason is worth recording because it is not obvious:

`IRelationalDatabaseCreator.CreateTables()` is **not idempotent** — it aborts on the first table that
already exists. That is documented in `DatabaseBootstrapper` (`ShouldCreateTables`, `:87-93`, `:135-137`)
and relied on by the test fixture, which calls `CreateTables()` **unconditionally** for each of the four
contexts (`PersistenceIntegrationTests.vb:51-54`).

A table mapped into four models is therefore created by the first `CreateTables()` and **aborts the
second** — in the fixture, and on a fresh database in `DatabaseBootstrapper` itself. Two tempting fixes
do not work:

- Excluding it from `ShouldCreateTables` does not help: that guard decides *whether* to call
  `CreateTables()`, not *which tables it creates*.
- Letting `ApplyTablePrefix` run produces four different tables (`Pos_AuditLog`, `Inv_AuditLog`, …)
  instead of one shared log — which defeats the point.

So the audit row must be written **on the same connection and transaction as the change, without being
part of the model.** Two ways:

- **(a) Raw ADO on the context's own connection.** Insert with a `DbCommand` on
  `Database.GetDbConnection()`, enlisted in `Database.CurrentTransaction`. No model change, no fifth
  context. Hand-written but parameterised SQL, and the codebase already does exactly this in
  `StockBatchLock` and `LockReceiptSequenceAsync`, with `DataWriteGuard` as the authorization
  counterpart.
- **(b) A fifth `AuditDbContext`** owning `App_AuditLog` alone, constructed per save with
  `UseMySQL(Database.GetDbConnection())` + `UseTransaction(…)` so it shares the change's transaction.
  Keeps the insert in EF and lets the existing per-context bootstrap create the table — at the cost of a
  fifth context, which AGENTS.md's "one DbContext per module" does not anticipate.

**Both need an explicit transaction** on any save that produces an audit row. EF's implicit per-save
transaction is created *inside* `MyBase.SaveChangesAsync`, so a statement executed outside it would not
be atomic with the change — and an audit log that can disagree with the data is worse than none.
`SaveChangesAsync` therefore begins a transaction only when one is not already open, and joins the
caller's when it is (the rule the FIFO decrement already follows).

**Recommendation: (a)** — smaller change, no fifth context, established pattern. The trade is
hand-written INSERT SQL plus a transaction on audited saves.

## 4. Assumptions I'm making

1. **The audit log is append-only.** An audit log that can be silently edited is worth nothing, so it
   gets the same UPDATE/DELETE triggers as the BIR receipt archives, reusing
   `ReceiptArchiveTriggerBootstrapper`.
2. **No UI in this pass.** The requirement is the log, not a viewer; Phase 7 UX is unwired and has no
   forward spec. It is queryable by SQL, and by Developer Tools later.
3. **The table is central** (`App_AuditLog`), created by an idempotent `CREATE TABLE IF NOT EXISTS`
   step that both the bootstrapper and the test fixture call — one source of truth for the DDL.
4. **Writes are captured at the data layer**, in `BaseDbContext.SaveChangesAsync`, so a handler cannot
   forget to log. Same reasoning `RoleGuardInterceptor` follows for roles.
5. **T9 is not part of this change.** It becomes possible afterwards.

## 5. Design

### 5.1 Table

`App_AuditLog`:

| Column | Purpose |
|---|---|
| `Id` | surrogate key |
| `OccurredAt` | `DATETIME(6)`, UTC |
| `Actor` | username, or `System` when there is no session |
| `WriteContext` | `User` / `System` — separates operator action from background work |
| `Module` | `Pos` / `Inv` / `Pur` / `Acc` |
| `EntityType` | CLR type name |
| `EntityId` | primary key as text |
| `Action` | `Insert` / `Update` / `Delete` |
| `ChangesJson` | see 5.3 |

Indexed on `OccurredAt`, `Actor`, and `(EntityType, EntityId)`.

### 5.2 Where the rows are written

In `BaseDbContext.SaveChangesAsync`, inside the same transaction as the change. The row data is
captured **before** `MyBase.SaveChangesAsync` (afterwards `OriginalValue` equals `CurrentValue` and the
previous values are gone), then inserted. A failing audit insert rolls the whole transaction back — the
change and its log commit together or not at all.

### 5.3 What `ChangesJson` contains — capture what the row will no longer hold

- **Update** → only the **changed columns**, as `{"Column": {"from": …, "to": …}}`. The unchanged
  columns are still readable in the row; the previous values are not, and they are the point.
- **Delete** → **all columns** at deletion, because nothing else records them afterwards.
- **Insert** → the **key alone**.

**Known limitation, and it is inherent to writing the log in the same batch:** for an entity with a
store-generated key, EF assigns that key *during* the insert, so an Insert row carries the pre-insert
value (`0`) rather than the assigned key. The row's own `CreatedAt`/`CreatedBy` already record the
creation, so the Insert row's job is "a record of this type was created, when, by whom". Capturing the
assigned key would mean a second save, which is what §3 rules out.

### 5.4 What gets audited — opt-in marker, not every table

`IAuditedEntity` on the financial records only. Auditing every `IAuditable` entity would flood the log
with reference and derived writes (`ProductCategory`, `ProductPriceHistory`, `FinancialSnapshot`,
`ReorderSuggestion`), which is not what "financial state changes" means.

The audited set (22 entities — the table below is authoritative):

| Module | Entities |
|---|---|
| POS | `SalesTransaction`, `SalesTransactionLine`, `SalesReturn`, `OfficialReceipt`, `OfficialReceiptArchive`, `ReceiptIntegrity`, `ReceiptIntegrityArchive`, `CreditAccount`, `CreditPayment`, `VatConfiguration` |
| Inventory | `StockBatch`, `ShrinkageRecord` |
| Purchasing | `PurchaseOrder`, `PurchaseOrderLine`, `GoodsReceipt`, `GoodsReceiptLine`, `AccountsPayableEntry` |
| Accounting | `RevenueRecord`, `ExpenseRecord`, `VatReturn`, `VatReturnLine`, `FinancialPeriod` |

`VatConfiguration` and `FinancialPeriod` are included deliberately: changing the VAT rate or closing a
period is financially significant, and neither leaves any other trace.

### 5.5 Immutability

Extend the existing trigger script with `app_audit_log_no_update` / `_no_delete` and add them to
`ReceiptArchiveTriggerBootstrapper.RequiredTriggers`. INSERT stays allowed. Reuses the Layer 3
mechanism from PR #126, so the log is protected by the database, not just by application code.

## 6. Open decisions

- **A5 — mechanism (a) raw ADO or (b) a fifth `AuditDbContext`.** See §3. **RESOLVED: (a),** the
  recommendation — the implementation writes the audit rows with a `DbCommand` on the context's own
  connection, enlisted in its current transaction (`BaseDbContext.InsertAuditRowAsync`).
- ~~A1 — where the rows are written.~~ Settled: `BaseDbContext.SaveChangesAsync`, so a handler cannot
  forget and the write is transactional.
- ~~A2 — the audited set.~~ Settled: the 20 entities in §5.4.
- ~~A3 — Insert payload.~~ Settled: key only.
- ~~A4 — retention.~~ Settled: none this pass.

## 7. Tasks

- [x] **A-T1 — table.** Idempotent create step, called by `DatabaseBootstrapper` and the test fixture.
- [x] **A-T2 — the write.** `IAuditedEntity`, the `SaveChangesAsync` hook (transaction-if-needed,
  capture-before, insert-after), the `WriteContext`/module plumbing.
- [x] **A-T3 — mark the audited entities.** One line per entity, per the table in §5.4.
- [x] **A-T4 — immutability triggers.** Extended the script + `RequiredTriggers`.
- [x] **A-T5 — verify.** Full suite, Debug/Release, fresh-database bootstrap test
  (`AuditTrailTests.AFreshDatabaseBootstrapsEveryModuleSchemaAndTheAuditTable`), smoke test.

## 8. Success criteria

1. A financial record update writes exactly one audit row, in the same transaction, with the actor,
   module, entity, and the changed columns with their previous values.
2. A rolled-back change writes **no** audit row.
3. A delete captures the values that no longer exist anywhere else.
4. A fresh database still creates all four module schemas **and** `App_AuditLog`.
5. The audit table rejects UPDATE and DELETE; INSERT succeeds.
6. Existing 100 tests still pass; build stays 0 warnings / 0 errors.

## 9. Non-goals

- An audit viewer UI (Phase 7).
- Business-context ("why") capture — a `Reason` column if wanted later.
- Retention/pruning (A4).
- T9 — enabled by this, implemented separately.

---

## What I need from you

**A5 only:** raw ADO (recommended) or a fifth context. Then I start at **A-T1**.
