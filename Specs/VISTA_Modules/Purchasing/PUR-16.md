# PUR-16: Vendor-Product Catalog & PO Auto-configuration

## Goal
Implement a relationship between Vendors and Products (Vendor Catalog). During Purchase Order creation, clicking "Add Line" should present a dropdown of products filtered specifically to what the selected Vendor supplies.

## Step-by-Step Requirements

1. **Entities & EF Configuration**
   - Create `MerchSys.Purchasing/Entities/VendorProduct.vb` inheriting from `SoftDeletableEntity`.
   - Create `MerchSys.Purchasing/Data/Configurations/VendorProductConfiguration.vb` to map to table `Pur_VendorProducts`. Add a composite unique index on `(VendorId, ProductId)` where `IsDeleted = 0`.
   - Add `DbSet(Of VendorProduct)` to `PurchasingDbContext.vb`.
   
2. **Database Migrations**
   - In `MerchSys.App/Data/DatabaseInitializer.vb`, add manual MariaDB DDL to create `Pur_VendorProducts` and its index.
   - Create a dummy artifact file `MerchSys.Purchasing/Migrations/20260527110000_AddVendorProductCatalog.vb`.

3. **Services**
   - Create `MerchSys.Purchasing/Dtos/VendorProductDto.vb`.
   - Create `MerchSys.Purchasing/Services/IVendorProductService.vb` and its implementation `VendorProductService.vb`.
   - In `VendorProductService.vb`, inject `ISessionService` and enforce the Manager role for all mutating actions (Create/Update/Delete). Owner role must be denied.
   - Register the service in `PurchasingServiceCollectionExtensions.vb`.

4. **Cross-Module Query**
   - Create a MediatR query `GetProductsForCatalogQuery.vb` in `MerchSys.SharedKernel/Queries/`.
   - Create the handler `GetProductsForCatalogQueryHandler.vb` inside the `MerchSys.Inventory` module (`src/MerchSys.Inventory/Handlers/`). It must query the central MariaDB via `MySqlConnector` (do not use SQLite).

5. **PO Editor Presenter**
   - Update `PurchaseOrderEditorPresenter.vb` and `PurchaseOrderListPresenter.vb`. 
   - Introduce an `ObservableCollection` called `VendorCatalog`.
   - Auto-populate unit cost and product name when a product is selected.
   - Add a pre-save validation blocking any save if `ProductId = 0`.

6. **Vendor Catalog Management UI**
   - Create `VendorCatalogPresenter.vb` for master-detail catalog management with Manager role guards.
   - Create `VendorCatalogView` (WinForms UserControl). Do not use WPF/XAML.
   - Register the view in DI and add it to `MainWindowPresenter` navigation.

7. **PO List View Updates**
   - In `PurchaseOrderListView.vb` and `.Designer.vb`, swap the Product ID textbox column for a `DataGridViewComboBoxColumn` bound to the vendor catalog.
