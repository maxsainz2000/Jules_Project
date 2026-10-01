1. **Create `IGoodsReceivingView.vb`**
   - In `src/MerchSys.Purchasing/Views/`, create the interface for the view.
   - It will need a way to bind a dropdown list of POs, bind the lines for the selected PO to a grid, and events for PO selection changing, discrepancy notes changing, and confirming receipt.

2. **Create `GoodsReceivingPresenter.vb`**
   - In `src/MerchSys.Purchasing/Presenters/`, create the presenter class.
   - Create helper classes `POSelectorItem` and `GRLineItem`. `GRLineItem` needs to auto-recalculate `HasDiscrepancy` when quantities differ.
   - Implement loading of Submitted POs via `IPurchaseOrderService.SearchAsync("", PurchaseOrderStatus.Submitted)`.
   - On PO selection, load PO lines and populate the editable grid.
   - Validation logic for discrepancy notes.
   - Call `IGoodsReceivingService.ReceiveGoodsAsync` to process the receipt.

3. **Create `GoodsReceivingView.vb` and `GoodsReceivingView.Designer.vb`**
   - In `src/MerchSys.Purchasing/Views/`, create the WinForms UserControl.
   - The UI contains a `ComboBox` for PO selection, a Confirm button, a Status bar, a placeholder panel, and a `DataGridView` for receiving items.
   - Grid columns: Product (read-only), Qty Ordered (read-only), Qty Received (editable), Unit Cost (editable), Expiry Date (**using the custom `DataGridViewDateTimePickerColumn`**), Discrepancy Notes (editable), and a warning indicator.
   - Set up amber highlight on discrepancy rows and red styling for notes.

4. **Update `PurchasingServiceCollectionExtensions.vb`**
   - Register `IGoodsReceivingView` mapped to `GoodsReceivingView` (Transient).
   - Register `GoodsReceivingPresenter` (Transient).

5. **Stage New Files**
   - Run `git add src/MerchSys.Purchasing/UI/Controls/*` and `git add src/MerchSys.Purchasing/Views/*` and `git add src/MerchSys.Purchasing/Presenters/*`.

6. **Compile and Pre-commit**
   - Run `dotnet build`.
   - Follow pre-commit instructions.
