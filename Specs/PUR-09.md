# PUR-09: PO Management (WinForms MVP)

## Context
Implement PUR-09 (PO Management) using Windows Forms and the MVP pattern in VB.NET. 
MIGRATE FROM WPF/MVVM to WINFORMS MVP: Instead of WPF XAML and ViewModels, you MUST use Windows Forms.

## Requirements

1. **PurchaseOrderListView (WinForms UserControl) & IPurchaseOrderListView & PurchaseOrderListPresenter**
   - DataGridView showing all POs (OrderNumber, Vendor, Status, OrderDate, TotalAmount).
   - Filter by status (All, Draft, Submitted, Received, Verified, Closed).
   - Search box for OrderNumber or Vendor Name.
   - Buttons: New PO, Edit, Submit, Delete.
   - Double-click on a row opens edit mode (same as clicking Edit).

2. **PurchaseOrderEditorView (WinForms UserControl or Form) & IPurchaseOrderEditorView & PurchaseOrderEditorPresenter**
   - Vendor dropdown.
   - Line items DataGridView (Product, Qty, UnitCost, auto-calc LineTotal).
   - Add/Remove buttons for line items.
   - Running total at the bottom.
   - Expected delivery date picker.
   - Notes textbox.
   - Save Draft / Submit buttons.

3. **Strict Validation (CRITICAL FOR QA)**
   - **Editing Non-Drafts:** You MUST ensure that the UI prevents editing a non-draft Purchase Order. The `PurchaseOrderListPresenter.OnEditClickedAsync()` or the view itself must verify that the PO's status is `Draft` before opening the editor. If the PO is not `Draft`, either disable the Edit button or show an error message and do not open the editor. If the editor is opened, it must not allow saving changes to a non-draft PO (the service will throw an `InvalidOperationException` if you try).
   - **Submit Logic:** Submitting must transition the PO status to `Submitted`.
   - **Role-Based Visibility (CRITICAL FOR QA):** You MUST implement role-based logic using `UserContext.CurrentRole`. If the current user role is `Owner` (read-only), you MUST hide or disable the New, Edit, Submit, and Delete buttons in the `PurchaseOrderListView` and ensure the editor is read-only.


4. **Architecture Rules**
   - Follow strict WinForms MVP.
   - NO WPF. NO MVVM. NO XAML.
   - Language: VB.NET.
