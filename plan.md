1. **Fix MainWindow.Designer.vb**
   - Completely remove `pnlSidebar` and `btnDashboard` from `MainWindow.Designer.vb` to prevent the empty 200px space issue.

2. **Fix Memory Leak in ModuleDetailPanel**
   - Update `UpdateVisiblePanel` in `ModuleDetailPanel.vb` to properly dispose of the old controls in `pnlItems` before clearing it.

3. **Verify and Build**
   - Build using `dotnet build src/MerchSys.slnx`.

4. **Complete pre-commit steps**
   - Review and submit.
