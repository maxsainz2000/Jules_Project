1. **Update `CreditPaymentEvent`:** Edit `src/MerchSys.SharedKernel/Events/CreditPaymentEvent.vb` to include properties like `CreditAccountId` and `Amount` so the event has meaningful data.
2. **Create `CreditBlockedException`:** Create a new exception class in `src/MerchSys.SharedKernel/Exceptions/CreditBlockedException.vb` (or inside the Services namespace as per requirement). The prompt says `MerchSys.POS/Services/CreditService.vb: implementation including CreditBlockedException` so I will place it in `src/MerchSys.POS/Services/CreditBlockedException.vb`.
3. **Create `ICreditService.vb`:** Create `src/MerchSys.POS/Services/ICreditService.vb` with methods:
   - `CreateAccountAsync`
   - `UpdateAccountAsync`
   - `GetAccountAsync`
   - `ListAccountsAsync`
   - `ChargeAccountAsync` (includes credit extension check)
   - `RecordPaymentAsync`
   - `GetAccountHistoryAsync`
   - `GetTotalsAsync`
   - `GetOverdueAccountsAsync`
4. **Create `CreditService.vb`:** Create `src/MerchSys.POS/Services/CreditService.vb` implementing `ICreditService`. Use `POSDbContext` and `IEventBus`.
   - In `ChargeAccountAsync`, enforce the hard block rule: `If account.IsBlocked OrElse account.CurrentBalance > 0 Then Throw New CreditBlockedException(...)`.
   - In `RecordPaymentAsync`, deduct from `CurrentBalance`, update `LastTransactionDate`, create a `CreditPayment` record, update `account.IsBlocked` appropriately, save, and publish `CreditPaymentEvent`.
5. **Run tests/pre-commit steps** to verify.
6. **Submit.**
