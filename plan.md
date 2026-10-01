1.  **Create `IGoodsReceivingView.vb`**:
    *   Define the interface for the Goods Receiving view (MVP).
    *   Include properties for events like `OnLoad`, `OnPOSelected`, `OnConfirmReceipt`.
    *   Include methods for binding PO dropdown, binding the receiving grid lines, and showing messages/errors.
2.  **Create `GoodsReceivingPresenter.vb`**:
    *   Define `POSelectorItem` and `GRLineItem` helper classes as per specs.
    *   Implement the presenter logic: loading Submitted POs, populating `GRLineItem`s when a PO is selected (defaulting `QtyReceived` to `QtyOrdered`), handling grid value changes to recalculate `HasDiscrepancy`, and processing `ConfirmReceipt`.
    *   Inject `IGoodsReceivingView`, `IPurchaseOrderService`, and `IGoodsReceivingService`.
3.  **Create `GoodsReceivingView.Designer.vb` and `GoodsReceivingView.vb`**:
    *   Implement the `IGoodsReceivingView` interface as a WinForms `UserControl`.
    *   Add UI components: PO selector `ComboBox`, "Confirm Receipt" `Button`, `DataGridView` for lines, and an empty state placeholder panel.
    *   Configure `DataGridView` columns: Read-only product info, editable received qty/cost, expiry date picker, discrepancy notes. Add DataError handling and cell formatting for discrepancy highlights (e.g., amber rows, red text).
    *   Wire UI events to the presenter actions.
4.  **Register dependencies in `PurchasingServiceCollectionExtensions.vb`**:
    *   Register `IGoodsReceivingView` (Transient) and `GoodsReceivingPresenter` (Transient).
5.  **Complete pre-commit steps to ensure proper testing, verification, review, and reflection are done.**
6.  **Submit the changes.**
