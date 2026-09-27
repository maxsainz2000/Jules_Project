1. **Create Interfaces for Views**
   - Create `IPurchaseOrderListView` in `MerchSys.Purchasing.Views.Interfaces`.
   - Create `IPurchaseOrderEditorView` (or define the editor interface if needed). Wait, the requirement says "bottom editor panel" in the same `PurchaseOrderListView.vb`. Let's clarify if the Editor is a separate control or if `PurchaseOrderListView.vb` implements BOTH interfaces or just one that exposes everything, or we have an Editor View interface implemented by a sub-panel.
   - I will define `IPurchaseOrderListView` and `IPurchaseOrderEditorView` in `src/MerchSys.Purchasing/Views/IPurchaseOrderListView.vb`.

2. **Implement Presenters**
   - Create `PurchaseOrderEditorPresenter.vb`. Manages state for POLineItem list (auto-recalculating LineTotal), vendor list, running total. Has methods to Add/Remove lines, prepare for new PO, load existing PO. Interacts with `IPurchaseOrderEditorView`.
   - Create `PurchaseOrderListPresenter.vb`. Manages PO listing, status/search filtering, and role-based visibility. Has methods to trigger New/Edit/Submit/Delete/Cancel. Holds a reference to the `PurchaseOrderEditorPresenter`. Interacts with `IPurchaseOrderListView`.

3. **Implement Views**
   - Create `PurchaseOrderListView.vb` (and `.Designer.vb`) as a WinForms `UserControl`.
   - Implement both `IPurchaseOrderListView` and `IPurchaseOrderEditorView` (if the editor is part of the same control, which the prompt implies: "Contains filter toolbar... main PO DataGridView... and a bottom editor panel."). Or better, have `PurchaseOrderListView` implement `IPurchaseOrderListView` and expose properties for the editor, but since MVP implies presenter per view, we can define `PurchaseOrderEditorView` as a separate `UserControl` nested inside, or just map both presenters to the same form if it implements both. The requirement says: "Views/PurchaseOrderListView.vb (and .Designer.vb): WinForms UserControl implementing the View interfaces." (plural). So it will implement both `IPurchaseOrderListView` and `IPurchaseOrderEditorView`.
   - Code-behind wires UI events (SelectionChanged, Click) to Presenter methods.

4. **Verify and build**
   - Run a build to ensure the code compiles without errors.
   - Run tests if applicable (though `dotnet test` might find 0 tests as per memory, we must include it in the plan).

5. **Pre-commit and PR**
   - Complete pre-commit steps to ensure proper testing, verification, review, and reflection are done.
   - Commit and push the PR.
