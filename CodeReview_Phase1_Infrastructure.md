# Code Review — Phase 1: Infrastructure & Foundation

**Project:** VISTA / MerchSys (VB.NET 10, WinForms MVP, centralized MariaDB 11.4 LTS)
**Scope reviewed:** `Roadmap.md` Phase 1 (INFRA-01 … INFRA-34) against the code in `src/`
**Reviewer role:** Code Review (independent, evidence-based)
**Build status verified:** `dotnet build MerchSys.slnx` → **Build succeeded. 0 Warning(s), 0 Error(s)** (dotnet 10.0.301)
**Date:** 2026-10-11

---

## 1. Method

- Read every Phase-1 source file in `src/MerchSys.SharedKernel` and the Phase-1 surface of `src/MerchSys.App`, plus the relevant entity/configuration files in the four module projects.
- Cross-checked each deliverable against `Specs/VISTA_Modules/Infrastructure/Infrastructure-Spec.md` and the verification checklist embedded in that spec.
- Verified wiring by grepping for DI registrations and usages (not just declarations).
- Compiled the solution to confirm the code is buildable.

**Important context discovered:** the migration stub `0002_seed_reference_data.sql` contains the comment *"Seed reference data is intentionally empty for this simplified reproduction"*, and `0001_initial_schema.sql` is 517 bytes (one table). Several INFRA specs describe a much larger original system. Where the spec claims and the repository diverge, this review reports the divergence against the **code that actually ships in this repository**, which is what the acceptance criteria must be judged against.

---

## 2. Verdict

The foundation is **structurally sound and it compiles cleanly**, but it is **not production-ready**. The core layering is correct (SharedKernel → modules → App, no cross-module references, MediatR contracts, MVP presenters). However there are **five blocking defects** — the most serious being that the four EF `DbContext`s and the raw-ADO services point at **different databases**, and the `IEventBus` abstraction that the POS services depend on **has no implementation and no DI registration**. Several Phase-1 features (Owner Dashboard, role-based navigation, connection indicator) are present as **unwired or mock code** while the spec and checklist mark them complete.

**Recommendation: do not sign off Phase 1 as complete.** Fix C1–C5 and H1–H4, then re-run the verification checklist.

| Severity | Count |
|---|---|
| Critical (blocker) | 5 |
| High | 9 |
| Medium | 18 |
| Low / Nit | 10 |

---

## 3. Critical findings (blockers)

### C1 — DbContexts and ADO services target *different databases*; connection string is hardcoded
**Files:** `src/MerchSys.App/Data/DatabaseConfig.vb:15-31`, `src/MerchSys.App/Program.vb:75`, `src/MerchSys.App/Services/AuthenticationService.vb:19`

`AddModuleDbContexts` accepts a `connectionString` parameter and **never uses it**:

```vb
Public Sub AddModuleDbContexts(services As IServiceCollection, connectionString As String)
    Dim hardcodedConnStr = "Server=127.0.0.1;Port=3306;Database=villon_farm_supply;"
    services.AddDbContext(Of PurchasingDbContext)(Sub(sp, opts)
        ... opts.UseMySQL(hardcodedConnStr).AddInterceptors(interceptor)
```

Meanwhile every raw-ADO service (`AuthenticationService`, `MariaDbSchemaInitializer`, `ConnectionHealthMonitor`) reads `Sync:MariaDbConnection`, which resolves to `merchsys_central`. Consequences:

- **The app runs schema-init, authentication and the health probe against `merchsys_central`, but all EF data operations against `villon_farm_supply`.** They are different databases.
- The hardcoded string has **no user/password** and **no pooling**, defeating INFRA-34 requirement B (`Pooling=true;Min Pool Size=1;Max Pool Size=100`).
- It ignores the operator-supplied production configuration entirely, breaking the centralized client-server architecture in `AGENTS.md` (clients must connect to the host laptop over the LAN, not `127.0.0.1`).

**Fix:** delete the hardcoded literal; pass the resolved connection string through (ideally the pooling-appended one from `ConnectionStringLoader`).

### C2 — `IEventBus` has no implementation and is not registered in DI
**Evidence:** grep for `Implements IEventBus` across `src/**/*.vb` → **0 matches**; grep for an `IEventBus` DI registration → **0 matches**.

`CreditService`, `PaymentService`, `SalesReturnService` and `VatAwareReceiptService` all take `IEventBus` as a constructor dependency. Any attempt to resolve those services will throw `InvalidOperationException: No service for type 'IEventBus'`. INFRA-04 specified this abstraction; only the interface exists.

**Fix:** implement a thin `MediatREventBus` (`Implements IEventBus`) delegating to `IMediator.Publish`, and register it in `MediatRConfig.AddMediatRServices`. Add a DI smoke test that resolves every registered service.

### C3 — Optimistic concurrency tokens are missing on the core mutable tables
**Files:** `src/MerchSys.Inventory/Entities/StockBatch.vb`, `src/MerchSys.POS/Entities/CreditAccount.vb`, `src/MerchSys.POS/Entities/SalesTransaction.vb`

`AGENTS.md` requires concurrency tokens on `Inv_StockBatches.QuantityRemaining`, `Pur_AccountsPayable.OutstandingBalance`, `Pos_CreditAccounts.OutstandingBalance`. In practice:

- `StockBatch` inherits `AuditableEntity`; `CreditAccount` / `SalesTransaction` inherit `SoftDeletableEntity`. **None declares `RowVersion`.**
- The `ConcurrencyAwareEntity` class that INFRA-26 claims to have created **does not exist** in `SharedKernel/Entities`.
- `RowVersion` is only declared on 8 auxiliary entities (VatReturn, ReorderSuggestion, PurchaseOrderLine, ReorderConfig, StockAlertConfig, FinancialPeriod, VatConfiguration, ReceiptSequence) — i.e. the config/lookup tables, **not** the financial/inventory hot spots.
- `BaseDbContext.IgnoreNonTokenRowVersionConvention` actively *removes* any `RowVersion` that is not explicitly mapped with `.IsRowVersion()`, so the guarantee depends entirely on per-entity configuration.

Net effect: INFRA-26's optimistic concurrency control is **not in force where it matters most** (stock quantities, AP balances, credit balances).

### C4 — Production config path contradicts the operator runbook
**File:** `src/MerchSys.App/Configuration/ConnectionStringLoader.vb:10-13`

```vb
Public Function GetProductionConfigPath() As String
    Return Path.Combine(AppContext.BaseDirectory, "appsettings.Production.json")
```

`AppContext.BaseDirectory` is the `bin` output folder. The INFRA-06 runbook and the verification checklist both instruct operators to place the file at `%LOCALAPPDATA%\VISTA\appsettings.Production.json`. An operator who follows the runbook will place the file where the app never looks; `AddProductionOverlay` is `optional:=True`, so it fails **silently**, `Sync:MariaDbConnection` stays empty, schema init is skipped (`Program.vb:115`), and login then fails with a confusing error.

**Fix:** reconcile the code and the runbook (pick one location), and fail loudly (log an error and show the operator a clear "production configuration not found" dialog) rather than silently continuing.

### C5 — Two competing schema-management mechanisms run at startup
**Files:** `src/MerchSys.App/Data/MariaDbSchemaInitializer.vb`, `src/MerchSys.App/Data/DatabaseInitializer.vb`, `Program.vb:118-121`

`Program.Main` runs **both** `MariaDbSchemaInitializer.Initialize(...)` *and* `DatabaseInitializer.Initialize(...)`. The second calls `EnsureCreated()` on all four DbContexts and then executes inline `ALTER TABLE … ADD COLUMN IF NOT EXISTS` DDL.

- `EnsureCreated()` builds the schema from the EF model and **ignores the migration scripts**; if the database is empty it creates a schema that may differ from the migration-derived one (no `RowVersion` columns, different defaults). If the database already exists it is a silent no-op — so the two mechanisms mask each other.
- The inline ALTERs duplicate the intent of `Data/Migrations/Central/*.sql`, creating **two sources of truth**.
- INFRA-24 claims the embedded migration ports "the complete SQLite baseline schema (40 tables, keys, triggers, constraints)" and seeds 4 categories / 3 vendors / 20 products. The actual `0001_initial_schema.sql` creates **one table** (`UserAccounts`); `0002_seed_reference_data.sql` is an empty stub. The BIR receipt-immutability triggers INFRA-24 claims to apply at startup are **not** in the embedded set.
- INFRA-27 lists `MerchSys.App/Data/DatabaseInitializer.vb` as *deleted*, yet the file exists and is invoked on the startup path.

**Fix:** pick one mechanism. Preferred: keep `MariaDbSchemaInitializer` + versioned embedded scripts, delete `DatabaseInitializer`, and port the real schema/triggers into the migration set.

---

## 4. High-severity findings

### H1 — Owner Dashboard ships hardcoded mock data (INFRA-16)
**File:** `src/MerchSys.App/Presenters/OwnerDashboardPresenter.vb:28-52`

```vb
Await Task.Delay(1000)                       ' "simulate data fetching delay"
_view.Kpi1Value = "$1,250,000"
_view.Kpi1Interpretation = "Gross Revenue is up 5% this month ..."
```

The dashboard displays invented USD figures and sleeps for a second to look busy. Test 17 requires live KPIs (vendors, open POs, stock value, sales, net income) with plain-language interpretations. The presenter also never disposes its 60-second `Timer`, and the catch block swallows all exceptions. The view is registered in DI but **never navigated to** — `MainWindowPresenter` has no Owner Dashboard entry — so it is dead code.

### H2 — Role-based navigation regression (INFRA-16)
**File:** `src/MerchSys.App/Presenters/MainWindowPresenter.vb:50-87`

`RebuildModuleCollections` only gates VAT Settings and VAT Returns behind Manager/Developer. Everything else — Purchasing Panel, Inventory Panel, POS Panel, Accounting Panel, Vendor Product Catalog — is added for **all** roles, including Owner. Tests 12 and 18 (Owner sees only read-only views) will fail. Additionally the navigation is a set of placeholders ("Purchasing Panel", "Inventory Panel", "POS Panel", "Accounting Panel"); the spec's Owner Dashboard, Transaction History, Purchase Orders, Accounts Payable and Stock Dashboard entries are absent.

### H3 — INFRA-28 UI is dead code
**Files:** `Startup/ConnectionConfig.vb`, `Views/ConnectionStatusIndicator.vb`, `Behaviors/DisableOnOfflineBehavior.vb`

`ConnectionStatusIndicator` and `ConnectionStatusPresenter` are registered in DI but **never added to any form** (`MainWindow.Designer.vb` has only `pnlSidebar` and `pnlContent`; there is no status slot). `DisableOnOfflineBehavior` is defined but **no control anywhere sets `IsDisabledWhenOffline`**, so the "grey out mutation buttons when offline" behaviour never activates. The feature compiles and does nothing.

### H4 — Audit columns are hardcoded to `"System"`; the audit trail is meaningless
**File:** `src/MerchSys.SharedKernel/Data/BaseDbContext.vb:61-83`

```vb
auditable.CreatedBy = "System"   ' Or fetch from some current user service
auditable.ModifiedBy = "System"
```

`AGENTS.md` and the BIR requirements demand that audit records capture **who** made a financial change. Every row is stamped `"System"`, so `CreatedBy` / `ModifiedBy` carry no information. The interceptor has no access to `ISessionService`. This also affects the Developer/Manager/Owner accountability story.

### H5 — The DA5 write guard is bypassed by raw ADO.NET
**Files:** `Data/RoleGuardInterceptor.vb`, `AuthenticationService.vb`, `DatabaseInitializer.vb`

`RoleGuardInterceptor` only intercepts EF `SaveChanges`/`SaveChangesAsync`. INFRA-25/INFRA-33 migrated "80+ direct connection ADO.NET query workarounds" to `MySqlConnector`, and `AuthenticationService.ChangePasswordAsync` / the lockout writes also use raw `MySqlCommand`. **None of those write paths are guarded.** The INFRA-20 "data-layer enforcement" claim therefore holds only for EF-originated writes — the guard is porous by construction. There is no integration test demonstrating rejection of an Owner write.

### H6 — Information disclosure on login (OWASP DA2)
**File:** `src/MerchSys.App/Services/AuthenticationService.vb:63-69`

```vb
If Not user.IsActive Then
    Return ... .ErrorMessage = "Account is disabled."
End If
If user.LockedUntil.HasValue AndAlso user.LockedUntil.Value > DateTime.UtcNow Then
    Return ... .ErrorMessage = "Account is locked. Please try again later."
```

The disabled check runs **before** password verification, and the message distinguishes "disabled" from "wrong password". Verification Test 15 asserts the opposite ("login fails with generic error, no information leakage"). Also, the lockout message omits the remaining minutes that Test 13 expects ("Try again in 15 minute(s)").

### H7 — Logout semantics are inconsistent and the idle-timeout re-login path is unsafe
**Files:** `Presenters/MainWindowPresenter.vb:118-121`, `Program.vb:150-196`, `Views/MainWindow.vb`

- The sidebar Log Out button routes to `MainWindowPresenter.OnLogoutRequested`, which calls `Application.Restart()` — a full process restart.
- The idle-timeout path calls `ForceLogout()` → `_mainWindowForm.Close()`, sets `_forceLogoutRequested = True`, and `RunApplicationFlow` loops back to re-run `Application.Run(...)` on `MainWindow` — but `MainWindow` is a **singleton** that `Close()` has already disposed. Re-login after an idle timeout will hit an `ObjectDisposedException`.
- Two different logout behaviours (restart vs. in-process loop) is a design smell; pick one.

### H8 — Password hash format diverges from the spec and is not self-describing
**File:** `src/MerchSys.App/Services/PasswordHashHelper.vb`

The hash is stored as `Base64(salt ‖ hash)`. INFRA-15 and Test 10 state the stored value must be a PHC string beginning `$argon2id$v=19$m=19456,t=2,p=1$`. The implementation cannot produce that, so the recorded acceptance evidence does not match the code. Because the KDF parameters are not stored, the cost parameters can never be raised without invalidating every existing hash. Additionally, `EnsureDefaultAccounts` computes **one** `hash` value and uses it for both `manager` and `owner`, so the two accounts share a salt (defeating per-user salting). Argon2id itself, `RandomNumberGenerator.Fill`, and `CryptographicOperations.FixedTimeEquals` are used correctly.

### H9 — There is no test project
`find` for `*.csproj` / `*Test*.vbproj` returns nothing. Phase 1 ships with **zero automated tests**; verification is entirely manual and is recorded in the checklist as prose. For a system with BIR/financial correctness requirements and known VB.NET/EF Core traps (`AGENTS.md` lists 12 of them), this is the single highest-leverage gap. At minimum add tests for: `PasswordHashHelper` round-trip, `WriteContextScope` nesting, `RoleGuardInterceptor` rejection, `SplitSqlStatements` edge cases, and `PagedResult` cursor math.

---

## 5. Medium-severity findings

| # | Finding | File(s) |
|---|---|---|
| M1 | `AuditInterceptor` is dead code duplicating `BaseDbContext.SaveChangesAsync`; it will drift. Delete or make it the single implementation. | `SharedKernel/Data/AuditInterceptor.vb` |
| M2 | `SyncConfig.AddSyncServices` is dead (body fully commented, never called). | `Startup/SyncConfig.vb` |
| M3 | `DatabaseInitializer` survives despite INFRA-27 declaring it deleted (see C5). | `Data/DatabaseInitializer.vb` |
| M4 | There is **no root `.gitignore`**. The verification checklist claims "`.gitignore` at the project root … line 2". `src/.gitignore` does cover `appsettings.Production.json`, but the checklist is inaccurate. | repo root |
| M5 | `appsettings.Example.json` is referenced by INFRA-28 but does not exist. | `MerchSys.App/` |
| M6 | `UserAccounts.Username` unique index has no `NOCASE` collation, contradicting INFRA-15; `manager` vs `Manager` become distinct users. | `0001_initial_schema.sql` |
| M7 | `ConnectionHealthMonitor.Start` runs the first probe on `Task.Run`, so `StateChanged` is raised on a thread-pool thread; correctness depends on every subscriber marshalling. `OnTimerTick` is `Async Sub` (fire-and-forget). | `Services/ConnectionHealthMonitor.vb:63,83` |
| M8 | `DisableOnOfflineBehavior.UpdateControlState` sets `control.Enabled = isOnline`, which will **re-enable controls that were disabled for role reasons** when the connection recovers. | `Behaviors/DisableOnOfflineBehavior.vb:81-93` |
| M9 | `OwnerDashboardPresenter` owns a `Timer` but implements no `IDisposable`; leaks on close. | `Presenters/OwnerDashboardPresenter.vb` |
| M10 | **No `Option Strict On` anywhere** in the solution. VB defaults to `Option Strict Off`, so narrowing conversions and late binding compile silently — a poor fit for financial code. | all `.vbproj` |
| M11 | MVP violations: `ConnectionStatusPresenter` depends on the concrete `ConnectionStatusIndicator`; `MainWindowPresenter` casts `_view` to concrete `MainWindow` and calls `Application.Restart()`. | presenters |
| M12 | `BaseDbContext.ApplySecondaryIndexes` adds a `(CreatedAt, Id)` index to **every** `IAuditable` entity, including tiny lookup tables, and can duplicate indexes defined in entity configurations. INFRA-34 asked for indexes on the large transactional tables. | `SharedKernel/Data/BaseDbContext.vb:37-47` |
| M13 | `MariaDbSchemaInitializer.SplitSqlStatements` is a naive line-based splitter: it does not understand semicolons inside string literals or `--`/`/* */` comments, and requires the delimiter to be at end-of-line. Fragile for real DDL. | `Data/MariaDbSchemaInitializer.vb:121-155` |
| M14 | `EnsureDeveloperAccount` lacks `INSERT IGNORE`/upsert, so two clients starting simultaneously can raise a duplicate-key error and abort startup. It also hardcodes the default credential `VistaDeveloper2026!` in source. | `Data/MariaDbSchemaInitializer.vb:165-184` |
| M15 | When `Sync:MariaDbConnection` is empty, schema init is skipped **silently** and the app proceeds to a login that cannot work. | `Program.vb:115-126` |
| M16 | `MerchSys.POS` references `Microsoft.EntityFrameworkCore.InMemory` — a test provider shipped in a production module. | `MerchSys.POS.vbproj` |
| M17 | Business/data libraries (`Purchasing`, `Inventory`, `Accounting`) target `net10.0-windows` with `UseWindowsForms=true`, so WinForms leaks into non-UI layers. | module `.vbproj` |
| M18 | Inconsistent target frameworks: `SharedKernel`/`POS`/`Accounting` = `net10.0`, `Inventory`/`Purchasing` = `net10.0-windows`. | module `.vbproj` |

---

## 6. Low-severity findings / nits

- **L1** `Program` is a `Module` holding mutable process state (`_idleMonitor`, `_mainWindowForm`, `_warningScope`, `_forceLogoutRequested`); hard to test, single-instance only.
- **L2** `ConnectionHealthMonitorLocator` is a mutable static service locator (acknowledged in spec, but still an anti-pattern; prefer constructor injection).
- **L3** `LoginSessionService` retains the whole `UserAccount` (including `PasswordHash`) in memory. INFRA-15 called for in-memory *tokens*; store only id/username/role.
- **L4** Silent `Catch ex As Exception` with empty bodies (`OwnerDashboardPresenter`, `LoginPresenter`); no `ILogger` in the presenters, so failures vanish.
- **L5** `MainWindow`: `UserIdentityText` writes to the window *title* (Test 20 expects a sidebar header); `DashboardButtonVisible` is a deprecated no-op; `SyncActiveModuleVisuals` is empty.
- **L6** `WinFormsIdleMonitor` and `ConnectionHealthMonitor` are singletons that are `Stop()`ed but never `Dispose()`d at shutdown.
- **L7** `MainWindow` declares `Public Shadows Event Load` and re-raises it from a constructor handler — works, but confusing and shadows a base member.
- **L8** `AddNavigationButtons` disposes and rebuilds every button on **every** navigation (`MainWindowPresenter.NavigateTo`), causing flicker and handler churn; layout is hardcoded pixel arithmetic rather than a `FlowLayoutPanel`.
- **L9** `MainWindowPresenter._currentViewScope` is disposed only on the next navigation — never on logout/window close.
- **L10** Documentation drift: the verification checklist references WPF-era paths (`WinForms_Applications\MerchSys\...`, `.Designer code` / `MainWindow.Designer code`) that do not exist in this repository, and describes behaviours (Sync indicator, Activity Rail) that are not present.

---

## 7. Security review (OWASP DA Top 10)

| Control | Status | Notes |
|---|---|---|
| DA1 Injection | ✅ | EF parameterised queries and `MySqlParameter` used throughout; no concatenated SQL with user input observed. |
| DA2 AuthN & Session | ⚠️ | Argon2id correct; lockout implemented. **H6** information disclosure; no session token/timeout beyond the idle monitor; password policy not enforced in the service layer. |
| DA3 Sensitive data | ⚠️ | Production config correctly gitignored and untracked (**verified**). Password hash retained in memory (L3). Config encryption deferred (backlog #19). |
| DA4 Crypto | ⚠️ | Argon2id + `FixedTimeEquals` good; **H8** non-standard, non-self-describing storage format and shared salt. |
| DA5 Authorization | ❌ | **H5** — the guard covers EF writes only; raw-ADO write paths (the majority after INFRA-25/33) are unguarded. **H2** — UI exposes write modules to Owner. |
| DA6 No default creds | ⚠️ | First-login password change enforced; but default credentials are hardcoded in source (M14) and `developer`/`manager`/`owner` are seeded on every fresh database. |
| DA7 Logging | ❌ | **H4** audit columns are `"System"`; presenters swallow exceptions (L4). |
| DA9 Security logging | ⚠️ | Failed logins are counted but not logged with source/timestamp to a durable audit trail. |
| DA10 SSRF/other | n/a | Desktop client. |

---

## 8. Architecture review

**Correct:**
- Module boundaries are respected — verified by grep: each module references **only** `SharedKernel`; `MerchSys.App` references all five. ✅
- One `DbContext` per module with a shared `BaseDbContext`, table prefixes, global soft-delete filter, audit convention. ✅
- MediatR contracts (`Events/`, `Queries/`) live in `SharedKernel`; `MediatRConfig` registers all six assemblies. ✅
- MVP pattern is broadly followed (interface-per-view, presenter owns view via interface). ✅
- `WriteContextScope` uses `AsyncLocal` with a correctly restored parent frame — good design for the guard's context model. ✅

**Concerns:**
- The **DI composition root is overloaded**: `Program.Main` contains schema bootstrap, health monitor lifecycle, idle-monitor wiring, and a login/logout loop, and doubles as the coordinator for the session-timeout dialog. Extract a `ShellCoordinator`/`ApplicationHost`.
- **Two schema mechanisms** (C5) and **two config sources** (C1) are the root causes of the two most dangerous defects.
- **`WriteContextKind.System` is a wide-open bypass.** Any code path can enter `System` and write as Owner. It is currently justified for lockout counters and seeding, but there is no test or lint preventing its misuse.
- The spec/roadmap are **out of sync with the code**: INFRA-10 and INFRA-30 describe WPF Master-Detail/Activity-Rail UI that does not exist in the WinForms codebase; INFRA-27 claims deletions that were not performed. `Roadmap.md` still marks INFRA-17/18/21/22/23 as `[-]`.

---

## 9. Traceability — INFRA-01 … INFRA-34

| Item | Roadmap | Review verdict |
|---|---|---|
| INFRA-01 Solution & projects | ✅ | **OK.** 6 projects, correct references, WinForms-only, pinned packages, no WPF artifacts. No test project (H9). |
| INFRA-02 SharedKernel foundation | ✅ | **OK.** Base/Auditable/SoftDeletable entities, interfaces, enums present and correct. |
| INFRA-03 DbContexts + BaseDbContext | ✅ | **Partial.** Soft-delete filter, audit convention, `IgnoreNonTokenRowVersionConvention` present; but audit hardcodes `"System"` (H4) and global indexes over-apply (M12). SQLite→MariaDB since migrated. |
| INFRA-04 MediatR event bus | ✅ | **Broken.** Contracts present; `IEventBus` has no implementation/registration (C2). |
| INFRA-05/06/09/12/13/21/22 Sync layer | ✅ / `[-]` | **Decommissioned** by INFRA-27 — correctly absent. Roadmap should be reconciled (21/22/23 still `[-]`). |
| INFRA-07 VAT event payloads | ✅ | **OK.** `VatTreatment` + VAT events with XML docs. |
| INFRA-08 MariaDB receipt triggers | ✅ | **Not deployed at startup** (C5) — triggers live only in `Plans/.../sql`, not in the embedded migration set. |
| INFRA-10 Sync status indicator | ✅ | **N/A / removed** with sync. Spec describes WPF; not present. |
| INFRA-11 Production config | ✅ | **Defective.** Path mismatch (C4), no root `.gitignore` (M4), empty/missing example file (M5). |
| INFRA-14 Receipt schema alignment | ✅ | SQL present in `Plans/.../sql`; not part of embedded migrations. |
| INFRA-15 Login & auth | ✅ | **Partial.** Argon2id correct; hash format diverges (H8), info disclosure (H6), no NOCASE collation (M6). |
| INFRA-16 Owner dashboard & read-only | ✅ | **Not delivered.** Mock KPIs (H1), role nav regression (H2), dashboard unreachable. |
| INFRA-17 Pomelo→MySqlConnector | `[-]` | **Done in code** (MySqlConnector 2.5.0 present, Pomelo absent); no `Directory.Build.props` exists despite spec. |
| INFRA-18 Audit detector tooling | `[-]` | Tooling only; `.claude/skills/vista-audit` not present in repo. |
| INFRA-19 Session timeout (+FIX) | ✅ | **Mostly OK.** MVP refactor done, `IDisposable` done, mouse-move throttle done. Idle re-login path unsafe (H7); checklist items unchecked. |
| INFRA-20 DA5 write rejection | ✅ | **Partial.** Guard implemented but bypassable (H5); no tests; checklist items unchecked. |
| INFRA-24 Central MariaDB bootstrap | ✅ | **Not implemented as described** (C5): 1-table migration, empty seed stub, no triggers, `DatabaseInitializer` still active. |
| INFRA-25 MariaDB provider migration | ✅ | Provider swap done; ADO paths converted. |
| INFRA-26 Concurrency tokens | ✅ | **Not in force on core tables** (C3). |
| INFRA-27 Sync decommission | ✅ | Sync files gone ✅, but `DatabaseInitializer.vb` (listed as deleted) remains and is invoked (M3/C5). |
| INFRA-28 Connection indicator | ✅ | **Dead code** (H3). |
| INFRA-29 Runbooks & backup | ✅ | Runbooks + scripts present under `Plans/.../runbooks`. |
| INFRA-30 Activity Rail shell | ✅ | **Not present**; `MainWindow` is a simple 2-panel form. |
| INFRA-31 RowVersion mapping correction | ✅ | Convention present; depends on C3 being fixed to be meaningful. |
| INFRA-32 Soft-delete vs unique | ✅ | Policy comments present in services (spot-checked); not exhaustively re-verified here. |
| INFRA-33 MariaDB SQL compatibility | ✅ | Boolean `TINYINT(1)` conversion and `DATETIME(6)` mapping present in `BaseDbContext`. |
| INFRA-34 Server-side paging | ✅ | `PageRequest` / `PagedResult` present. Pooling requirement defeated by C1. |

---

## 10. Prioritised remediation plan

**Before Phase 1 sign-off (blockers):**
1. **C1** — remove the hardcoded connection string; route one resolved, pooling-enabled connection string to the DbContexts *and* the ADO services.
2. **C2** — implement and register `IEventBus`; add a DI-resolution smoke test.
3. **C5** — delete `DatabaseInitializer`; make the embedded migration set the single schema source; port the real tables and triggers.
4. **C4** — align the production-config path between code and runbook; fail loudly when it is missing.
5. **C3** — reintroduce a `ConcurrencyAwareEntity` (or per-entity `RowVersion`) on the AGENTS.md-mandated tables and verify with a concurrency test.

**Then (high):**
6. **H1/H2/H3** — implement the real Owner dashboard KPIs, restore role-based navigation, and either wire up or remove the INFRA-28 indicator/behaviour.
7. **H4** — inject `ISessionService` into the audit path so `CreatedBy`/`ModifiedBy` record the real user.
8. **H5** — extend the write guard to raw-ADO paths (or funnel them through a guarded repository).
9. **H6/H7/H8** — generic auth errors, unify logout, adopt PHC-format Argon2id with per-user salts.
10. **H9** — stand up a test project; cover auth, scope nesting, guard rejection, SQL splitting and paging.

**Housekeeping:**
11. Delete `AuditInterceptor` and `AddSyncServices`; enable `Option Strict On` (start with SharedKernel); remove `EFCore.InMemory` from POS; normalise target frameworks; add a root `.gitignore`; reconcile the roadmap (`INFRA-17/18/21/22/23`) and the stale WPF-era checklist.

---

## 11. What was verified, and how

- `dotnet build MerchSys.slnx` → **0 warnings, 0 errors** (dotnet 10.0.301, `net10.0`).
- Project-reference graph extracted from all `.vbproj` files — module boundaries confirmed.
- DI registrations and usages confirmed by grep (not by reading declarations alone): `IEventBus`, `AuditInterceptor`, `AddInterceptors`, `OwnerDashboardView`, `ConnectionStatusIndicator`, `DisableOnOfflineBehavior`, `AddSyncServices`.
- `appsettings.Production.json` confirmed **gitignored** (`git check-ignore` → `src/.gitignore:4`) and **untracked** (`git ls-files` → no match) — no credential leak.
- Root `.gitignore` confirmed **absent**; `src/.gitignore` present.

*End of review.*
