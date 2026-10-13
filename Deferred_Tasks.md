# Jules AI Deferred Tasks Ledger

This ledger acts as a central tracking point for any tasks that Jules AI encountered difficulties with, such as incomplete implementation, missing dependencies, impossible constraints within the specifications, or errors. 

**This file will be automatically populated by the 24/7 Antigravity Agent when it detects issues during the verification gate, and presented for your manual review.**

## Tasks Deferred

| Date | Phase / ID | Description of Issue / Jules Feedback | Agent Recommendation | Status |
|---|---|---|---|---|
| _Example_ | _INV-02_ | _Spec demands local SQLite for caching, violating MariaDB-only rule_ | _Rewrite Spec INV-02 to use MemoryCache_ | _Pending Review_ |
| 2026-10-13 | Returns / INV-02 | A restocked sales return is created with **no expiry date** — `StockReturnedHandler` passes `expiryDate:=Nothing` — so returned pesticides, seeds and feeds never appear in the expiry monitor (INV-12). The return path has **no batch lineage**: nothing records which batch a sale consumed, so the original batch's expiry cannot be recovered. A guessed date would be worse than a missing one — it would put a wrong date in the monitor and could trigger a false write-off. | Take the date from the **operator at return time**, who has the physical item: add an optional per-product expiry to the return flow (`ProcessReturnRequestedEventArgs` → `ISalesReturnService.ProcessReturnAsync` → `SaleReturnedEvent.ReturnedItem.ExpiryDate` → `StockReturnedHandler`), surfaced only for products with `HasExpiry = True`. The event/handler plumbing is small; the return dialog is the larger half. The alternative — recording batch consumption at sale time — is the deeper fix but touches the (currently unwired) checkout path. | Pending Review |

---

> [!NOTE]
> **Workflow Reminder**
> 1. Jules fails or defers a task.
> 2. The local Git Action listener wakes Antigravity.
> 3. Antigravity drafts the issue here (and updates `Specs/` if needed).
> 4. You must approve the update.
> 5. The next Jules session begins.
