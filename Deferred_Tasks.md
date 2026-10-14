# Jules AI Deferred Tasks Ledger

This ledger acts as a central tracking point for any tasks that Jules AI encountered difficulties with, such as incomplete implementation, missing dependencies, impossible constraints within the specifications, or errors. 

**This file will be automatically populated by the 24/7 Antigravity Agent when it detects issues during the verification gate, and presented for your manual review.**

## Tasks Deferred

| Date | Phase / ID | Description of Issue / Jules Feedback | Agent Recommendation | Status |
|---|---|---|---|---|
| _Example_ | _INV-02_ | _Spec demands local SQLite for caching, violating MariaDB-only rule_ | _Rewrite Spec INV-02 to use MemoryCache_ | _Pending Review_ |
| 2026-10-13 | Security / ACC-01 | `manager` and `owner` are seeded with the committed password `Vista2026!` in **every** configuration (`MariaDbSchemaInitializer.EnsureDefaultAccounts`), so every install ships with two known credentials. The forced first-login change (`LoginPresenter` → `RequiresPasswordChange`) narrows but does not close the window between install and first login. The `developer` account (role 3) is now Debug-only; these two are the production bootstrap and were deliberately left alone. | Make the initial password **operator-supplied or generated** instead of hardcoded — read an optional bootstrap password from configuration/environment on first run, or log a one-time generated password, then force the change. Needs a first-run provisioning decision (who sets it, recovery if lost), so it is not a one-liner. | Pending Review |
| 2026-10-13 | Accounting / ACC-10 | **Returns are attributed to the original sale's VAT period, not the return's.** `SaleReturnedAccountingHandler` reduces `RevenueRecord.OutputVat` in place and never changes `RecordDate` (set to the sale date), and `VatReportingService` sums by `RecordDate` — so a return in month N+1 mutates month N's record and the current period never sees the credit. If month N is already **Filed**, regeneration throws `VatReturnLockedException` and the credit is **stranded**; `AmendVatReturnAsync` only flips the status and never re-reads the ledger. No credit-note/adjustment concept exists. | Introduce a discrete **`VatAdjustmentRecord`** for the closed-period case, keyed by the return date (`SaleReturnedEvent.ReturnedAt`) and gated on the original period's `VatReturns.FilingStatus` — **mutually exclusive** with the in-place mutation, or the credit is counted twice. For open periods the in-place reduction + regeneration is adequate. **Blocked on a tax-policy decision:** whether BIR expects the reversal in the credit period or the original period must be confirmed with the accountant before implementing. Also fix the `Accounting-Spec.md:146` drift (the described BIR line-number mapping is not implemented). | Pending Review |
| 2026-10-13 | Returns / INV-02 | A restocked sales return is created with **no expiry date** — `StockReturnedHandler` passes `expiryDate:=Nothing` — so returned pesticides, seeds and feeds never appear in the expiry monitor (INV-12). The return path has **no batch lineage**: nothing records which batch a sale consumed, so the original batch's expiry cannot be recovered. A guessed date would be worse than a missing one — it would put a wrong date in the monitor and could trigger a false write-off. | Take the date from the **operator at return time**, who has the physical item: add an optional per-product expiry to the return flow (`ProcessReturnRequestedEventArgs` → `ISalesReturnService.ProcessReturnAsync` → `SaleReturnedEvent.ReturnedItem.ExpiryDate` → `StockReturnedHandler`), surfaced only for products with `HasExpiry = True`. The event/handler plumbing is small; the return dialog is the larger half. The alternative — recording batch consumption at sale time — is the deeper fix but touches the (currently unwired) checkout path. | Pending Review |

---

> [!NOTE]
> **Workflow Reminder**
> 1. Jules fails or defers a task.
> 2. The local Git Action listener wakes Antigravity.
> 3. Antigravity drafts the issue here (and updates `Specs/` if needed).
> 4. You must approve the update.
> 5. The next Jules session begins.
