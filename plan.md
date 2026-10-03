1. **Modify `CreditAccount.vb` and `SalesTransactionLine.vb`:**
   - Add properties `TotalCreditExtended As Decimal` and `LastTransactionDate As DateTime?` to `CreditAccount.vb`.
   - Add `ProductId As Integer` to `SalesTransactionLine.vb`.
   - Verify modifications using `read_file`.

2. **Update `DatabaseInitializer.vb`:**
   - Add manual SQL migrations to `Pos_CreditAccounts` to add `TotalCreditExtended` and `LastTransactionDate`.
   - Add manual SQL migrations to `Pos_SalesTransactionLines` to add `ProductId`.
   - Verify the edits to `DatabaseInitializer.vb` were applied correctly using `read_file`.

3. **Define `IPaymentService` Interface:**
   - Create `src/MerchSys.POS/Services/IPaymentService.vb`
   - Define `PaymentResultDto` class.
     - `Success As Boolean`
     - `TransactionId As Integer`
     - `ChangeAmount As Decimal`
     - `ReceiptNumber As String`
     - `ErrorMessage As String`
   - Define `IPaymentService` interface with `Function ProcessPaymentAsync(transactionId As Integer, amountTendered As Decimal, method As String, referenceNumber As String) As Task(Of PaymentResultDto)`
   - Verify the new file's creation and contents using `read_file`.

4. **Implement `PaymentService`:**
   - Create `src/MerchSys.POS/Services/PaymentService.vb`
   - Implement `IPaymentService`
   - Inject `POSDbContext` and `IEventBus`.
   - `ProcessPaymentAsync`:
     - Load `SalesTransaction` using EF Core `Include(Function(t) t.Lines)` and `Include(Function(t) t.CreditAccount)`.
     - Check if transaction exists and is not voided.
     - Validate amount tendered. For Cash/GCash/BankTransfer, `amountTendered >= TotalAmount`. For Credit, `amountTendered` can be 0 or more.
     - Calculate `ChangeAmount = amountTendered - TotalAmount`. Set to 0 if negative.
     - Update payment method on transaction.
     - If Credit:
       - Ensure `CreditAccount` is not null and not blocked.
       - Add `TotalAmount` to `CurrentBalance`.
       - Add `TotalAmount` to `TotalCreditExtended`.
       - Set `LastTransactionDate = DateTime.Now`.
       - If `CurrentBalance > 0`, set `IsBlocked = True`.
     - Publish `SaleCompletedEvent` via `IEventBus`.
       - Map `Lines` to `SaleCompletedItem` using `ProductId` and `QuantitySold` based on the confirmed event definition.
     - Save changes using `POSDbContext`.
     - Set the `ReceiptNumber` property of `PaymentResultDto` to `transaction.TransactionNumber`.
     - Return `PaymentResultDto`.
   - Verify the newly created file using `read_file`.

5. **Testing:**
   - Run `dotnet test src/MerchSys.slnx` to ensure the changes are correct and have not introduced regressions.

6. **Pre-commit step:**
   - Complete pre-commit steps to ensure proper testing, verification, review, and reflection are done.

