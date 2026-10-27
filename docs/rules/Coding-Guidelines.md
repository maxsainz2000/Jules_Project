# Jules AI Coding Guidelines

## CRITICAL: No Empty Commits
- When creating new files, you MUST implement the actual logic and UI components before committing and pushing the PR. 
- Creating empty files as placeholders (e.g., from a plan.md) and submitting them is strictly forbidden. 
- Always verify that the file contents are saved and contain the required implementations before finishing the task.

## CRITICAL: Git Staging New Files
- When you create NEW files, you MUST explicitly stage them using `git add <file>` or `git add .` before you run `git commit`. 
- If you only run `git commit -am` or `git commit -a`, it will NOT track newly created files, resulting in PRs that omit all of your work.
- Always run `git status` right before committing to ensure all your new files are listed under "Changes to be committed".

## CRITICAL: Database Schema Column Length Constraints
- Always inspect the database schema definition (e.g. `AuditLogSchema.vb` or table DDL) before writing string literals to database columns.
- For example, `App_AuditLog.Action` is defined as `VARCHAR(8)`. Action literals must never exceed 8 characters (e.g. use `"DedFail"` [7 chars], never `"DeductFail"` [10 chars] which triggers MariaDB SQL Error 1406 `ER_DATA_TOO_LONG`).

## CRITICAL: Test Doubles and MediatR Mock Handlers
- When adding new MediatR queries or commands that existing services dispatch (such as `GetVatConfigurationQuery` or `GetCurrentStockQuery`), inspect ALL test classes and test doubles across `tests/MerchSys.Tests` that invoke those services (e.g. `PosIdentifierConcurrencyTests.vb`, `CartServiceTests.vb`, etc.).
- Ensure all test fakes (`FakeStockMediator`, mock handlers) handle the newly dispatched queries instead of throwing `NotSupportedException`.

## CRITICAL: xUnit Fixtures and Required Entity Properties in Tests
- Any test class taking `TestDatabaseFixture` in its constructor MUST explicitly declare `Implements IClassFixture(Of TestDatabaseFixture)`.
- When constructing EF entities in tests (such as `SalesTransactionLine`), always populate all required (`NOT NULL`) properties (e.g. `.ProductName = "Test Product"`).
- In services with collaborator services (e.g. `_priceChangeService` in `GoodsReceivingService`), guard optional calls with `If _priceChangeService IsNot Nothing Then` or supply a mock in tests to prevent `NullReferenceException`.

## CRITICAL: Database Test Fixture Lifecycle and Shared Schema
- All tests that interact with MariaDB MUST belong to `<Collection("IntegrationDatabase")>` and be decorated with `<Trait("Category", "Integration")>`.
- NEVER instantiate `New TestDatabaseFixture()` directly inside test classes, and NEVER call `Database.EnsureCreated()` or `Database.EnsureDeleted()` in test constructors, teardown, or `Dispose()`.
- The shared database fixture is managed exclusively by xUnit's collection fixture mechanism: implement `IClassFixture(Of TestDatabaseFixture)` and accept `fixture As TestDatabaseFixture` in the constructor. Instantiating ad-hoc fixture instances or calling `EnsureDeleted()` drops and recreates `vista_integrationtest`, crashing other tests executing in parallel.

## CRITICAL: BaseDbContext Relational Audit Logging & InMemory Incompatibility
- `BaseDbContext.SaveChangesAsync` requires a relational database provider because `InsertAuditRowAsync` calls `Database.GetDbConnection()` to insert `App_AuditLog` rows for all `IAuditedEntity` instances (like `RevenueRecord` and `ExpenseRecord`).
- The EF Core InMemory provider (`UseInMemoryDatabase`) is NOT relational and will throw:
  `InvalidOperationException: Relational-specific methods can only be used when the context is using a relational database provider.`
- Therefore, NEVER call `SaveChangesAsync` on a `BaseDbContext` subclass against `UseInMemoryDatabase`.
- When creating smoke harnesses or unit tests for services, use Moq or test doubles (e.g. `FakeVatReportingService`, `FakeFinancialOverviewService`) or resolve live relational contexts from the application's `IServiceProvider`.

## CRITICAL: Test Fakes and Test Doubles Must Reside in Test Projects
- NEVER place test doubles/fakes inside production projects under `#If DEBUG Then`.
- When test classes in `tests/MerchSys.Tests` reference these fakes, Release configuration builds will fail (`BC30002: Type 'Fake...' is not defined`) because `#If DEBUG` classes are omitted in Release mode.
- All test doubles, fakes, and mocks MUST reside within `tests/MerchSys.Tests` so both Debug and Release builds compile cleanly with 0 errors and 0 warnings.

## CRITICAL: Stage All View and Designer Code in UI Deliverables
- When implementing a feature that touches the UI, you MUST stage both the view code-behind (`View.vb`) and the designer file (`View.Designer.vb`) using `git add .` before committing.
- Dropping designer or view files results in missing UI controls, unattached event handlers, and failed verification.


## CRITICAL: WinForms Presenter Binding on Form Views (MainWindow.Presenter)
- When a shell form (like `MainWindow`) exposes a `Presenter` property so child views can request navigation or shell actions, the presenter's constructor MUST assign itself to the view:
  ```vb
  Dim mw = TryCast(_view, MainWindow)
  If mw IsNot Nothing Then
      mw.Presenter = Me
  End If
  ```
- If this assignment is omitted, `shell.Presenter` remains `Nothing`, causing child view navigation requests to be silently ignored.

## CRITICAL: Smoke Test Harnesses Must Execute Actual Verification (No Hardcoded Pass Strings)
- Smoke harnesses (e.g. `VatTileSmokeHarness`) must execute the actual validation logic (e.g. executing the calculation, invoking the presenter with manager and owner roles to assert navigation events fire or suppress).
- NEVER hardcode `"## ...: PASS"` strings into reports without performing the corresponding verification assertions.

## CRITICAL: Cross-Platform Identity (CA1416 Platform Warnings)
- In libraries targeting `net10.0` (platform-agnostic), calling `System.Security.Principal.WindowsIdentity.GetCurrent()` produces compiler warning `CA1416` ("This call site is reachable on all platforms...").
- Always use `Environment.UserName` instead, which is cross-platform, non-allocating, and compiler warning-free.

## CRITICAL: MediatR Closed Gap Reconciliation in Tests
- `tests/MerchSys.Tests/MediatRWiringTests.vb` contains `KnownNotificationGaps`.
- The test strictly enforces: *"A gap that has been closed must come off the list, or the list quietly becomes fiction."*
- Whenever you implement a handler for an event that was previously listed in `KnownNotificationGaps` (e.g. `ReceiptTamperDetectedEvent`), you MUST remove it from `KnownNotificationGaps` in `MediatRWiringTests.vb`.

## CRITICAL: Overridable DbContext DbSet Properties for Moq Interception
- In VB.NET, class properties are not virtual (`Overridable`) by default.
- When unit testing with `Moq` (`mockContext.SetupProperty(Function(c) c.MyDbSet, ...)`), Moq throws `NotSupportedException` if the `DbSet` property on the `DbContext` is not `Overridable`.
- Always declare test-mockable `DbSet` properties as `Public Overridable Property ... As DbSet(Of ...)`.

## CRITICAL: DbContext Explicit Overloaded Constructors for Moq / Castle DynamicProxy
- In VB.NET, optional constructor parameters (`Optional param As Type = Nothing`) do NOT emit overloaded constructor signatures in IL metadata.
- Castle DynamicProxy (used by Moq) searches for constructors matching the exact number and types of arguments passed into `New Mock(Of TDbContext)(options)`. When only one argument is passed, it fails with:
  `System.ArgumentException: Could not find a constructor that would match given arguments...`
- Therefore, always provide an explicit single-parameter constructor overload on DbContext subclasses:
  ```vb
  Public Sub New(options As DbContextOptions(Of AccountingDbContext))
      Me.New(options, Nothing, Nothing)
  End Sub
  ```
  And in test code using Moq, either pass all 3 arguments (`options, Nothing, Nothing`) or utilize this explicit overload.









## CRITICAL: Never Invoke Synchronous Modal Dialogs (MessageBox.Show) Without Abstraction or Test Guards
- In automated test environments (headless CI or `dotnet test`), invoking `MessageBox.Show(...)` opens a Win32 modal dialog that pauses execution indefinitely waiting for user interaction. This hangs `testhost.exe`.
- In testable Windows Forms panels or presenters, abstract user notification dialogs (e.g. via an injected delegate `Action(Of String, String, MessageBoxIcon)`, `INotificationService`, or an internal property/method that tests can silence or inspect).
- Never allow unit tests to directly execute code paths that open blocking modal message boxes.

## CRITICAL: Avoid VB.NET Namespace Doubling in Test Files
- The project `MerchSys.Tests.vbproj` defines `<RootNamespace>MerchSys.Tests</RootNamespace>`.
- Adding `Namespace Tests` inside test files creates the doubled namespace `MerchSys.Tests.Tests`, violating architectural namespace rules.
- Test classes directly in `tests/MerchSys.Tests` should omit `Namespace Tests` so their full namespace remains `MerchSys.Tests`.

## CRITICAL: Unique File Paths for Harness Reports (Parallel Test Thread Safety)
- When generating temporary files in runners, services, or test harnesses (e.g., `VatLedgerSchemaHarnessRunner`), never rely solely on second-granular timestamps (`yyyyMMddHHmmss`).
- xUnit executes test classes in parallel across multiple threads. Two tests running simultaneously in the same second will compute identical file paths in `%TEMP%`, causing intermittent `System.IO.IOException` ("The process cannot access the file because it is being used by another process").
- Always include a unique token or GUID in the filename:
  `$"vat-ledger-schema-report-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}.md"`

## CRITICAL: Moq View Mock Property Tracking in MVP Presenter Tests
- When testing Windows Forms MVP Presenters where the presenter writes to view properties (e.g., `_view.SelectedYear = ...`) and subsequently reads them back in the same workflow (e.g., `_service.GetMonthlySummaryAsync(_view.SelectedYear, ...)`), always call `_viewMock.SetupAllProperties()` during test arrangement.
- Without `SetupAllProperties()`, loose Moq mocks return default values (`0` or `Nothing`), causing unexpected parameter invocations and test failures.

## CRITICAL: Month-Scoped Date Range Queries with Timestamps
- When querying timestamped date columns (`DateTime.UtcNow`, `RecordDate`, `ExpenseDate`) for a month period, never construct the upper boundary using `startDate.AddMonths(1).AddDays(-1)` with a `<=` operator. This truncates the last day to `00:00:00`, silently omitting all transactions occurring later on that day.
- Always use an exclusive upper boundary:
  `Dim nextMonthDate = startDate.AddMonths(1)`
  `Where(Function(r) r.RecordDate >= startDate AndAlso r.RecordDate < nextMonthDate)`

## CRITICAL: Accounting Reporting Views Unconditional Access for Owner Role
- Financial and compliance reports (`VAT Relief Report`, `Tamper Audit Report`, `Sales Summary`, `Income Statement`, `Financial Overview`) are read-only reports intended for both Manager and Owner roles.
- In `MainWindowPresenter.vb`, always add these navigation items to `accountingGroup.Items` outside the `If isManager Then` guard.

## CRITICAL: Moq Event Raising with AddHandler in VB.NET
- When raising events on Moq mocks in VB.NET using `mock.Raise(Sub(m) AddHandler m.MyEvent, Nothing, ...)`, be aware that the `Nothing` immediately following `m.MyEvent` is part of the `AddHandler` statement syntax.
- For delegates with signature `(sender As Object, e As TEventArgs)`, Moq requires two arguments: the `sender` and `e`.
- Therefore, to pass `sender = Nothing` and your event argument `e = arg`, you must pass TWO values after the lambda:
  `mockView.Raise(Sub(m) AddHandler m.MyEvent, Nothing, Nothing, arg)`
- Omitting the sender argument results in `System.Reflection.TargetParameterCountException: Parameter count mismatch`.

## CRITICAL: Complete View Event Handler Unsubscription in MVP Presenters
- In WinForms MVP Presenters, the `View` property setter must symmetrically unsubscribe all events that it subscribes.
- When adding new view events (e.g., `ExportCsvRequested`, `ExportPdfRequested`), always add corresponding `RemoveHandler _view.EventName, AddressOf Handler` calls inside the `If _view IsNot Nothing Then` block.

## CRITICAL: Testing Async Sub Event Handlers with File I/O
- In Windows Forms MVP Presenters, event handlers wired to view events are typically `Async Sub`.
- In unit tests, NEVER rely on `Await Task.Yield()` or arbitrary sleeps when testing `Async Sub` methods that perform file I/O (`File.WriteAllBytesAsync`). `Task.Yield()` yields control only once and does not wait for the asynchronous I/O pipeline to finish.
- Attempting to read or delete the file immediately triggers `System.IO.IOException: The process cannot access the file because it is being used by another process`.
- Always synchronize test completion using a `TaskCompletionSource(Of Boolean)` wired to the terminal collaborator call (such as `_notificationService.ShowInfo(...)`):
  ```vb
  Dim tcs As New TaskCompletionSource(Of Boolean)()
  mockNotification.Setup(Sub(n) n.ShowInfo(It.IsAny(Of String)(), It.IsAny(Of String)())).Callback(Sub() tcs.TrySetResult(True))

  mockView.Raise(Sub(m) AddHandler m.ExportCsvRequested, Nothing, Nothing, testFilePath)

  Dim completed = Await Task.WhenAny(tcs.Task, Task.Delay(5000))
  Assert.Same(tcs.Task, completed)
  ```
  This guarantees that the asynchronous write operation has completely finished and released file locks before assertions and file cleanup run.

## CRITICAL: Multi-Item Sale Loop Idempotency Guards
- In handlers processing multi-item collections (like `SaleCompletedEvent.Items`), NEVER place a sale-level idempotency check (`If Not _dbContext.Table.Any(Function(r) r.SaleId = notification.SaleId)`) inside the per-item loop if records are saved during or across loop iterations.
- The first item's records will cause subsequent items to evaluate the condition to `False`, silently dropping all items after the first.
- Perform the idempotency check once at the start of the handler:
  `Dim alreadyHandled = Await _dbContext.SaleCogsRecords.AnyAsync(Function(r) r.SaleId = notification.SaleId, cancellationToken)`
  `If alreadyHandled Then Return`

## CRITICAL: Clean Commits (No Stray .tmp, .diff, .patch, or Backup Files)
- Never stage or commit 0-byte, `.tmp`, `.bak`, `.diff`, `.patch`, search/replace leftovers, or temporary scratch files into the git repository.
- Always run `git status` prior to committing to ensure only intended project source files are staged. Any PR containing temporary patch/diff files will be rejected immediately during QA.

## CRITICAL: BaseDbContext Non-Relational Provider Guard for In-Memory Tests
- `BaseDbContext.SaveChangesAsync` inserts audit rows into `App_AuditLog` via raw ADO `Database.GetDbConnection()` for any modified `IAuditedEntity` instances (like `RevenueRecord` and `ExpenseRecord`).
- In non-relational environments (such as unit tests using `UseInMemoryDatabase`), `Database.IsRelational()` is `False`, and calling relational methods throws `InvalidOperationException`.
- In `BaseDbContext.SaveChangesAsync`, guard relational audit row persistence:
  ```vb
  Dim auditRows = BuildAuditRows(entries, actor)

  If auditRows.Count = 0 OrElse Not Database.IsRelational() Then
      Return Await MyBase.SaveChangesAsync(cancellationToken)
  End If
  ```
  This allows in-memory unit tests touching audited entities to execute cleanly without requiring a live MariaDB database.

## CRITICAL: Dynamic Theme Synchronization in Presenters
- Any Presenter injecting `IThemeService` to style views must subscribe to `IThemeService.ThemeChanged` so views update dynamically when theme switches at runtime.
- Always symmetrically unhook `ThemeChanged` handlers in `Dispose()`.
- In unit tests where a presenter constructor invokes a view method during initial setup, ensure mock assertions account for constructor calls (e.g. `Times.Exactly(2)` or configure distinct initial mock state before raising events).
- In unit tests for classes that call `_serviceProvider.CreateScope()` (like `MainWindowPresenter.NavigateTo`), never pass a bare unconfigured `Mock(Of IServiceProvider)()`. Either build a provider with `New ServiceCollection().BuildServiceProvider()` or mock `IServiceScopeFactory` returning a mocked `IServiceScope`, otherwise `InvalidOperationException: No service for type IServiceScopeFactory has been registered` will be thrown.
- In unit tests requiring `LoginSessionService`, never mock it with Moq (`Mock(Of LoginSessionService)`). `LoginSessionService` is a concrete class with non-overridable properties (`CurrentUserRole`, `CurrentUsername`), and Moq throws `NotSupportedException`. Instantiate a real instance (`Dim sessionService As New LoginSessionService()`) and set test user state via `sessionService.SetUser(...)`.
- In unit tests where `MainWindowPresenter.NavigateTo` executes, the resolved view is cast via `DirectCast(sp.GetRequiredService(Of TView)(), UserControl)`. Never register a bare Moq interface proxy like `Mock(Of IOwnerDashboardView)().Object` in DI because it inherits from `System.Object`, causing `InvalidCastException`. Register the concrete view (`services.AddTransient(Of IOwnerDashboardView, OwnerDashboardView)()`) or a test `UserControl` subclass.
- When refining `ThemeService.ApplyTheme` rules (e.g. differentiating button variants between Accent and SurfaceBackground), update existing test `ThemeService_ApplyTheme_UpdatesControlColors` (e.g. set `button.Tag = "Accent"`) so legacy assertions do not break. Run `dotnet test` across the full test suite to guarantee 0 regressions.
- Never pause or stall to ask conversational questions before committing or opening a pull request. Implement all requirements, verify `git diff` is non-empty, run `dotnet test` to confirm clean execution, commit, and push the PR autonomously.
- In View classes taking a presenter dependency or optional presenter, always guard `_presenter.View = Me` with `If _presenter IsNot Nothing Then` (and preferably provide a parameterless `Public Sub New()` constructor calling `InitializeComponent()`) so the view can be instantiated in unit tests and at design-time without throwing `NullReferenceException`.
- In Form/UserControl Designer files, ensure every control referenced in `InitializeComponent()` (such as `pnlVatTile`, `lblVatPayable`, `lblVatDueDate`) is explicitly instantiated with `New System.Windows.Forms.Panel()`, `New System.Windows.Forms.Label()`, etc. at the start of `InitializeComponent()` before assigning any properties or adding child controls.
