1. **Service Verification**: Check `AccountsPayableService` and `IAccountsPayableService`. The method `GetAllAsync()` has already been added and returns all AP entries with Vendor and PurchaseOrder navigation ordered by InvoiceDate descending. So no code changes are needed here.
2. **Extensions Registration**: Register `IAccountsPayableService -> AccountsPayableService` (Scoped) and `APLedgerPresenter` (Transient) in `PurchasingServiceCollectionExtensions.vb`. Also register `IAPLedgerView -> APLedgerView` (Transient). `AccountsPayableService` is already registered, so only add `IAPLedgerView`, `APLedgerView` and `APLedgerPresenter`.
3. **Presenter Creation (`APLedgerPresenter.vb`)**:
   - Create `APLedgerPresenter.vb` inside `src/MerchSys.Purchasing/Presenters/`.
   - Add helper classes `APLedgerRow` (for grid display) and `VendorSelectorItem`.
   - Implement filter logic (All/Outstanding/Overdue/Paid + vendor).
   - Implement inline payment dialog state and commands.
   - Add Boolean properties: `IsAllFilterActive`, `IsOutstandingFilterActive`, `IsOverdueFilterActive`, `IsPaidFilterActive`.
   - Apply IsManager check for payment commands using `ISessionService`.
4. **Interfaces and View Creation (`IAPLedgerView.vb` & `APLedgerView.vb`)**:
   - Create `IAPLedgerView.vb` in `src/MerchSys.Purchasing/Views/`.
   - Create `APLedgerView.vb` and `APLedgerView.Designer.vb` in `src/MerchSys.Purchasing/Views/Purchasing/` as WinForms UserControl.
   - Provide summary header with total outstanding.
   - Filter toolbar with status buttons (All, Outstanding, Overdue, Paid) and a vendor ComboBox.
   - DataGridView with columns (`VendorName`, `InvoiceNumber`, `InvoiceDate`, `DueDate`, `TotalAmount`, `AmountPaid`, `Balance`, `IsPaid`, `IsOverdue`).
   - Implement row highlighting: overdue (amber), paid (green).
5. **Payment Dialog Creation (`APLedgerPaymentDialog.vb`)**:
   - Create `APLedgerPaymentDialog.vb` and `APLedgerPaymentDialog.Designer.vb` in `src/MerchSys.Purchasing/Views/Dialogs/`.
   - It will collect payment amount and confirm.
6. **Pre-commit and Test**:
   - Run `dotnet build`.
   - Complete pre commit steps to ensure proper testing, verification, review, and reflection are done.
7. **Submit**: Create PR.
