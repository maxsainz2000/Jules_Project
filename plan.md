1. **Create ErrorStatePanel control**:
   - Create `src/MerchSys.App/Controls/ErrorStatePanel.vb` with properties: `Title`, `Message`, `RetryText`. Include `RetryClicked` event. ApplyTheme method to theme it like the `EmptyStatePanel`.
   - Create `src/MerchSys.App/Controls/ErrorStatePanel.Designer.vb` declaring labels and a retry button.

2. **Wire ErrorStatePanel into Data Views**:
   - For `FinancialOverviewView`, `IncomeStatementView`, `SalesSummaryView`, `TamperAuditReportView`, `VatReliefReportView`, `VatReturnView`, `VatSettingsView`, `OwnerDashboardView`:
     - In the `.Designer.vb` file: instantiate `Me.pnlErrorState = New MerchSys.App.Controls.ErrorStatePanel()` ONCE at the start of `InitializeComponent()`. Add configuration: `Dock = Fill`, `Location = (0, 0)`, `Name = "pnlErrorState"`, `TabIndex = 26`, `Visible = False`. Add it to `Controls` (`Me.Controls.Add(Me.pnlErrorState)`).
     - In the `.vb` file: in all constructors (both parameterless and parameterized), wire `pnlErrorState.RetryClicked` to the corresponding refresh event (`ViewLoaded`, `ReloadRequested`, or `RefreshRequested`).
     - In the `ErrorMessage` property setter (if applicable), set `pnlErrorState.Visible = True`, update its `Message`, bring to front, and hide conflicting elements.
     - When `ErrorMessage` is empty or cleared, or loading is true, set `pnlErrorState.Visible = False`.

3. **Add Unit Tests**:
   - In `tests/MerchSys.Tests/StateAndFeedbackTests.vb`, add `ErrorStatePanel_SetProperties_FiresRetryEvent` and `ErrorStatePanel_ApplyTheme_UpdatesColors`.

4. **Pre-commit step**:
   - Complete pre-commit steps to make sure proper testing, verifications, reviews and reflections are done.
