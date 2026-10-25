# Design Note — Concurrency cluster C5–C8

**Status: APPROVED (2026-10-14). D1–D4 resolved below; T1–T3 implemented. T4–T9 outstanding.**

Evidence baseline: `master` @ `79d3881` (PRs #126–#128 merged). Build 0 warnings / 0 errors;
91 tests pass (49 unit + 42 integration). Findings referenced from `CodeReview_Phases2-5_NonUI.md`.

Phase: **Define & Plan** (spec → plan → tasks → implement). This note is the spec + plan; the task
list at the end is the breakdown. Implementation starts only after the **Open Decisions** below are
answered.

---

## 1. Objective

Make the four remaining concurrency defects (C5–C8) behave correctly under the deployment's stated
worst case — **up to 4 client laptops writing to one central MariaDB** — and make every concurrency
failure **visible to the operator** instead of silent. Success is not "no exceptions"; it is "no
lost update, no partial write, and a conflict the operator can act on".

## 2. Verified current behaviour (evidence)

| # | Claim | Evidence |
|---|---|---|
| C5 | FIFO deduction is an unlocked read-modify-write: read batches → sum → check → decrement in memory → one `SaveChangesAsync`. No transaction, no row lock. `SELECT ... FOR UPDATE` (which AGENTS.md mandates) appears **nowhere in `src/`**. | `StockService.vb:30-69` |
| C5 | The handler loops items and each `DeductStockFIFOAsync` saves independently, so a mid-loop failure leaves a **partial deduction with no rollback**. It catches only `InsufficientStockException`. | `SaleCompletedHandler.vb:28-44` |
| C5 | Same unlocked pattern in shrinkage. | `ShrinkageService.vb:41-64`, `:82` |
| C6 | `PurchaseOrder` carries **no** `RowVersion` (only `PurchaseOrderLine` does). `Submit`/`Receive`/`Verify`/`Close` are check-then-act with no guard. `Close` also creates an `AccountsPayableEntry` — two clients closing the same order book the liability twice. | `PurchaseOrder.vb` (grep: no match); `PurchaseOrderService.vb:106-152` |
| C7 | Transaction number is check-then-act: read max `TX-yyyy-nnnn`, add one, insert. A UNIQUE index exists on the column, so the loser hits a **unique violation that is not handled** and the sale is lost. | `CartService.vb:183-202`; `SalesTransactionConfiguration.vb:22-23` |
| C8 | Receipt integrity chain reads the tail row's hash and inserts using it as `PreviousHash`. Two concurrent receipts read the same tail and **fork the chain** — each validates alone, then `ValidateChainAsync` reports a break. | `ReceiptIntegrityService.vb:46-47`, `:62-63` |
| C8 | The receipt-sequence retry catches **only** `DbUpdateConcurrencyException`, so the first receipt of a new year (two clients both `Add` a `ReceiptSequence`) hits a **unique violation it does not catch** and receipt generation fails outright. | `ReceiptIntegrityService.vb:141-169` |

Two facts that shape the design:

- **The optimistic token already works.** All 12 `RowVersion` columns carry
  `ON UPDATE CURRENT_TIMESTAMP(6)`; the token *does* raise `DbUpdateConcurrencyException`. Every C5–C8
  defect is about **handling and serialization**, not detection.
- **`PaymentService` commits the sale before the stock handler runs** (`SaveChangesAsync` at `:77`,
  publish at `:79`), so the sale and the stock movement are in **different transactions**. See
  Open Decision **D1** — this is the one part of C5 a lock inside Inventory cannot fix.

## 3. Assumptions I'm making

1. **AGENTS.md is authoritative over the current code.** It mandates `SELECT ... FOR UPDATE` inside
   the FIFO decrement, so the design uses pessimistic locking rather than an optimistic-retry-only
   scheme. If you would rather keep it optimistic-only, say so — see D2.
2. **4 concurrent clients is the design target** (not hundreds). Short critical sections are
   acceptable; lock contention at this scale is negligible.
3. **Isolation is MariaDB's default (`REPEATABLE READ`).** The design relies on a locking read
   returning the latest committed row.
4. **Checkout is still unwired** (`PosPanel.vb` is a stub), so this is *fix-before-it-ships*, not an
   incident. That is why a design note is affordable now.
5. **No migrations.** A schema change (C6) reaches an existing database only by a hand-run ALTER,
   documented in runbook 01 §4b — as `SalesReturn.SerializedLines` already is.
6. **Module boundaries stay as they are** unless you explicitly decide otherwise (relevant to D1).

→ Correct any of these now, or I will proceed with them.

## 4. Design

### 4.1 C5 — FIFO deduction

**Proposal.** Give `DeductStockFIFOAsync` an explicit transaction with a deterministic locking read:

1. `BeginTransactionAsync`.
2. Lock the candidate batches with a raw locking read, **ordered deterministically**:
   `SELECT Id FROM Inv_StockBatches WHERE ProductId = @p AND QuantityRemaining > 0
   ORDER BY ReceiptDate, Id FOR UPDATE`.
   The `ORDER BY` is not cosmetic: two clients locking overlapping batch sets in different orders
   deadlock; a shared order makes the second block on the first row instead.
3. Load the same rows through EF (tracked) and run the existing FIFO logic unchanged. The lock is
   already held, so this read sees the state the lock protects.
4. `SaveChangesAsync`; `CommitAsync`.

**Implementation constraint.** The locking read must go through the provider-agnostic
`DbConnection`/`DbCommand` API. `Database.GetDbConnection()` returns a `MySql.Data` connection —
casting it to `MySqlConnector.MySqlConnection` throws `InvalidCastException` (a defect this project
already paid for once).

**Alternative considered and rejected:** expressing the lock via `FromSqlRaw("... FOR UPDATE")` so EF
tracks the rows in one round trip. Rejected because EF composes over raw SQL by wrapping it in a
derived table, and MySQL rejects `FOR UPDATE` there. Two queries is the boring, reliable option.

**Tradeoff.** A pessimistic lock holds a row lock for the duration of the decrement. At 4 clients
this is microseconds; the alternative (optimistic retry) trades that for spurious failures under
contention. See D2.

### 4.2 C5 — atomic item set, and the sale↔stock window

**Proposal.** `SaleCompletedHandler` wraps **all** items in one transaction:

- Deduct every item, `SaveChangesAsync` once, commit — so a failure on item 3 rolls back items 1–2
  instead of leaving them deducted.
- Publish `StockDeductedEvent` **after** commit (already the pattern in the returns flow, PR #125).
  Today a mid-loop failure deducts item 1 and never publishes the event, so **COGS is never booked
  for it** while its revenue is — a silent accounting inconsistency, not just a stock one.
- Catch `DbUpdateConcurrencyException` and surface AGENTS.md's wording, *"Data changed elsewhere —
  refresh and retry"*, instead of letting it escape as an unhandled exception.

**Not fixable here:** the sale is committed in the POS context before this handler runs, so a
deduction failure still leaves a committed sale with no stock movement. **D1.**

### 4.3 C5 — shrinkage

Same treatment as 4.1 in `ShrinkageService.RecordShrinkageAsync`; the unlocked `QuantityRemaining -=`
at `:41-64` is the identical defect. Smaller blast radius (a manual inventory action, not a sale), so
it can follow the same PR or a follow-up.

### 4.4 C6 — `PurchaseOrder` concurrency token

**Proposal.** Add `RowVersion As DateTime` to `PurchaseOrder` and map it explicitly:

```vb
builder.Property(Function(e) e.RowVersion).IsRowVersion().IsConcurrencyToken().HasColumnType("TIMESTAMP(6)")
```

The mapping is **mandatory**, not stylistic: `BaseDbContext.IgnoreNonTokenRowVersionConvention`
removes any `RowVersion` that is not explicitly a token, and an unmapped one fails model validation
for the **whole Purchasing module**. Copy the pattern from `PurchaseOrderLineConfiguration.vb:19-22`.

Then handle `DbUpdateConcurrencyException` in the four transitions (`Submit`/`Receive`/`Verify`/`Close`)
and surface the same "refresh and retry" message. `CloseAsync` already writes the status and the AP
entry in a single `SaveChangesAsync`, so the token alone makes a double-close impossible.

**Schema note.** Existing databases need
`ALTER TABLE Pur_PurchaseOrders ADD COLUMN RowVersion TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);`
— the `ON UPDATE` clause must be written **explicitly** in an ALTER (MariaDB only applies it
implicitly to the first `TIMESTAMP` column of a new table). Document in runbook 01 §4b.

### 4.5 C7 — transaction number

**Proposal.** Make the number authoritative at write time rather than at cart-build time, and handle
the unique violation: catch the duplicate-key failure, regenerate, and retry a bounded number of
times. The more robust alternative is a dedicated sequence row locked with `FOR UPDATE` (the pattern
`ReceiptSequence` already uses), but that is a larger change to the cart flow.

**Open:** the exact retry point depends on where the insert happens relative to the number
generation; I will confirm the call path during implementation rather than guess here. See D3.

### 4.6 C8 — receipt integrity

**Proposal, two parts.**

- **C8b (small, independent).** Catch the duplicate-key failure in `GetNextReceiptNumberAsync`'s retry
  loop, not just `DbUpdateConcurrencyException`. Without this the first receipt of a new year fails
  outright.
- **C8a (design change).** Serialize receipt issuance per year by holding `FOR UPDATE` on that year's
  `ReceiptSequence` row across **both** number generation and the integrity append. That is the
  smallest lock that covers the chain tail without depending on the table being non-empty (locking
  the "last integrity row" has no row to lock on the first receipt of a year). It does mean combining
  two currently separate operations, which is why it is called out. See D4.

## 5. Open decisions (need your answer)

**D1 — sale↔stock atomicity (the important one).** A lock inside Inventory cannot make "sale
committed" and "stock deducted" atomic; they are separate transactions on separate contexts
(`PaymentService.vb:77` then `:79`). Options:

- **(a) One transaction across the POS and Inventory contexts.** The only *fully* correct option. It
  requires a shared `DbConnection`/transaction across two module contexts — i.e. crossing the
  "MediatR only, one DbContext per module" boundary. `System.Transactions` ambient enlistment is not
  reliable across Oracle's `MySql.Data` provider, so this would be an explicit shared-connection
  design at the App composition root. **Architecturally significant.**
- **(b) Compensate and surface.** Accept the window; on a deduction failure, persist an explicit
  "stock not deducted" marker, surface it to the operator, and provide a retry. Requires somewhere to
  record the failure — which is the H1 audit trail that does not exist yet.
- **(c) Validate before commit.** Pre-check (and briefly lock) stock before saving the sale, then
  deduct after. Narrows the window; does not close it.

**My recommendation: (c) + (b) now, and (a) as an explicit architectural decision before checkout
ships.** (a) is correct but changes the architecture AGENTS.md prescribes, so it is your call, not
mine.

**D2 — `FOR UPDATE` vs optimistic-only.** AGENTS.md mandates `FOR UPDATE`. I will implement it unless
you tell me otherwise. The counter-argument worth hearing: the tokens already detect every conflict,
so an optimistic retry loop would also be *correct* — it would just fail more often under contention
and would not literally satisfy the spec. Given the professor's spec is the authority, I recommend
following it.

**D3 — C7 retry point.** Retry-on-duplicate at the insert (small, changes the cart flow slightly) vs a
locked sequence row (more robust, larger change). I lean retry-on-duplicate for the smaller diff.

**D4 — C8a scope.** Combining number generation and the integrity append into one locked operation is
a real change to the receipt flow. Confirm you want it in this pass, or split it out.

## 5a. Decisions taken (2026-10-14)

| # | Decision |
|---|---|
| D1 | (c) + (b) now; (a) stays an open architectural question to settle before checkout ships. |
| D2 | `FOR UPDATE`, as AGENTS.md mandates. |
| D3 | Retry-on-duplicate. |
| D4 | C8b now; C8a in this pass. |

**D1 is only partly delivered by T1–T3.** They fix the deduction — serialized, atomic across a
sale's items, and never announced when it rolls back — but they do **not** close the sale↔stock
window, for two reasons found while implementing:

- **(c) belongs at the cart checkout, not at payment.** `CartService.vb:116-117` is where the sale is
  first persisted; `PaymentService` only finalises payment on an already-committed transaction. A
  pre-commit stock check therefore has to sit before `CartService`'s save → **T8**.
- **(b) needs somewhere to record the failure**, which is the H1 audit trail that does not exist yet
  → **T9**, blocked on H1.

## 6. Task breakdown (ordered by dependency)

- [x] **T1 — FIFO decrement transaction + `FOR UPDATE`.** `StockService.vb`, `StockBatchLock.vb`.
  - Acceptance: concurrent deductions of the same product serialize; the loser sees the first's
    result, not a lost update.
  - Verified: `StockConcurrencyTests.ADeductionWaitsForAnotherClientsLockInsteadOfFailing` — and the
    test was confirmed to **fail** with the lock removed, so it is discriminating.
- [x] **T2 — `SaleCompletedHandler` atomic item set + post-commit publish + conflict surfacing.**
  - Verified: `AMidSaleFailureLeavesNoDeductionAndPublishesNothing`.
- [x] **T3 — `ShrinkageService` same treatment.**
  - Verified: `ShrinkageDeductsFifoAndReportsTheValue`.
- [x] **T4 — C6 `PurchaseOrder` token.** `PurchaseOrder.vb`, `PurchaseOrderConfiguration.vb`,
  runbook 01 §4b, tests.
  - Acceptance: two concurrent `CloseAsync` calls → one succeeds, one surfaces the conflict; only one
    AP entry exists.
  - Verified: `PurchaseOrderConcurrencyTests` — two tests (close and submit), both confirmed to **fail**
    with the token removed. Applied to `vista_central`; the runbook carries the hand-run ALTER.
  - Note: the translation covers all seven saves the token can affect (`UpdateDraft`, `Submit`,
    `Receive`, `Verify`, `Close`, `RecalculateTotal`, `Delete`), not just the four transitions —
    otherwise the token would have replaced a silent overwrite with a raw provider error.
- [x] **T5 — C7 transaction number.** `CartService.FinalizeTransactionAsync` re-derives and retries
  on a duplicate key, bounded at 5 attempts. `DuplicateKeyDetector` matches MariaDB's
  "Duplicate entry" text rather than catching the provider's exception type, which POS does not
  reference.
  - Verified: `PosIdentifierConcurrencyTests.ConcurrentSalesAllocateDistinctTransactionNumbers`.
- [x] **T6 — C8b sequence retry + C8a chain serialization.**
  - C8b: the retry loop now also catches the duplicate key, and detaches the rejected insert so the
    retry is not the same failing insert for ever.
  - C8a: the append takes `FOR UPDATE` on that year's `Pos_ReceiptSequence` row, and **both the tail
    read and `ValidateChainAsync` now walk the integrity table's own `Id`** — append order.
  - **Finding while implementing:** the fork had *two* causes. Serializing the append alone was not
    enough, because the chain was built in append order but validated in `ReceiptId` order — two
    different sequences (a receipt is created before its integrity row is appended). The test caught
    this: it still failed with the lock in place until the ordering was corrected too.
  - Verified: `ConcurrentIntegrityAppendsDoNotForkTheChain` and
    `ConcurrentReceiptNumbersAreAllDistinct`; the chain test was confirmed to **fail** with the lock
    removed.
- [ ] **T7 — verify.** Full suite + Debug/Release build; live smoke test; runbook + memory updated.
- [x] **T8 — D1(c) pre-commit stock check.** Must sit at the cart checkout (`CartService.vb:116`),
  via the existing `GetCurrentStockQuery`, **not** at payment. POS-side; the checkout path is
  unwired, so it is service-level testable only.
- [x] **T9 — D1(b) compensate and surface.** Blocked on **H1** — there is no audit table to record
  the failure in.

Each task touches ≤5 files. T1–T3 are one PR (the C5 theme); T4, T5, T6 can be separate PRs.

## 7. Success criteria

1. Two clients selling the same product concurrently both succeed, with the second reading the
   first's committed quantities — no lost update, no spurious failure.
2. A failure part-way through a sale's item set leaves **no** deduction and **no** `StockDeductedEvent`.
3. Two clients closing the same purchase order produce exactly **one** `AccountsPayableEntry`.
4. Two concurrent sales never collide on `TransactionNumber`; a collision is retried, never lost.
5. Two concurrent receipts never fork the integrity chain, and the first receipt of a new year
   succeeds.
6. Every concurrency conflict reaches the operator as *"Data changed elsewhere — refresh and retry"*.
7. `SELECT ... FOR UPDATE` appears in `src/` — satisfying the AGENTS.md requirement that is currently
   unimplemented.
8. Existing 91 tests still pass; build stays 0 warnings / 0 errors.

## 8. Non-goals

- The H1 audit trail (separate finding — but D1(b) depends on it).
- Reworking the module boundary or the MediatR event contracts beyond what D1 requires.
- Optimising the critical section for more than 4 clients.

---

## What I need from you

Answer **D1** (especially), **D2**, **D3**, **D4** — or just say "follow your recommendations" and I
will proceed with (c)+(b) for D1, `FOR UPDATE` for D2, retry-on-duplicate for D3, and C8b-now/C8a-in-
this-pass for D4. Then I start at **T1** and work down, one PR per theme.
