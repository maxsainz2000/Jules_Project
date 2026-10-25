Imports System.Collections.Concurrent
Imports MediatR
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Exceptions
Imports MerchSys.SharedKernel.Queries
Imports Microsoft.EntityFrameworkCore

Namespace Services

    Public Class CartService
        Implements ICartService

        Private ReadOnly _db As POSDbContext
        Private ReadOnly _mediator As IMediator
        Private Shared ReadOnly _carts As New ConcurrentDictionary(Of String, CartDto)()

        ''' <summary>
        ''' A handful of attempts is plenty for four client laptops; more would only mask a real
        ''' problem with the numbering.
        ''' </summary>
        Private Const MaxTransactionNumberAttempts As Integer = 5

        Public Sub New(db As POSDbContext, mediator As IMediator)
            _db = db
            _mediator = mediator
        End Sub

        Public Function GetCart(cartId As String) As CartDto Implements ICartService.GetCart
            Return _carts.GetOrAdd(cartId, Function(id) New CartDto() With {.CartId = id})
        End Function

        Public Sub AddToCart(cartId As String, productId As Integer, productName As String, unitPrice As Decimal, quantity As Integer) Implements ICartService.AddToCart
            Dim cart = GetCart(cartId)
            SyncLock cart
                Dim existingLine = cart.Lines.FirstOrDefault(Function(l) l.ProductId = productId)
                If existingLine IsNot Nothing Then
                    existingLine.Quantity += quantity
                    existingLine.LineTotal = existingLine.UnitPrice * existingLine.Quantity
                Else
                    cart.Lines.Add(New CartLineDto With {
                        .ProductId = productId,
                        .ProductName = productName,
                        .UnitPrice = unitPrice,
                        .Quantity = quantity,
                        .LineTotal = unitPrice * quantity
                    })
                End If
                RecalculateCart(cart)
            End SyncLock
        End Sub

        Public Sub RemoveFromCart(cartId As String, productId As Integer) Implements ICartService.RemoveFromCart
            Dim cart = GetCart(cartId)
            SyncLock cart
                Dim lineToRemove = cart.Lines.FirstOrDefault(Function(l) l.ProductId = productId)
                If lineToRemove IsNot Nothing Then
                    cart.Lines.Remove(lineToRemove)
                    RecalculateCart(cart)
                End If
            End SyncLock
        End Sub

        Public Sub ClearCart(cartId As String) Implements ICartService.ClearCart
            Dim cart As CartDto = Nothing
            _carts.TryRemove(cartId, cart)
        End Sub

        Private Sub RecalculateCart(cart As CartDto)
            cart.TotalAmount = cart.Lines.Sum(Function(l) l.LineTotal)
            ' This is UI-only recalculation, exact VAT is computed at finalization
            cart.VatableSales = Math.Round(cart.TotalAmount / 1.12D, 2)
            cart.VatAmount = cart.TotalAmount - cart.VatableSales
        End Sub

        Public Async Function FinalizeTransactionAsync(cartId As String, paymentMethod As String, Optional creditAccountId As Integer? = Nothing) As Task(Of SalesTransaction) Implements ICartService.FinalizeTransactionAsync
            Dim cart = GetCart(cartId)

            If Not cart.Lines.Any() Then
                Throw New InvalidOperationException("Cannot finalize an empty cart.")
            End If

            Dim stockResult = Await _mediator.Send(New GetCurrentStockQuery())
            For Each line In cart.Lines
                Dim stock = stockResult.StockLevels.FirstOrDefault(Function(s) s.ProductId = line.ProductId)
                If stock Is Nothing OrElse stock.CurrentQuantity < line.Quantity Then
                    Throw New InsufficientStockException($"Insufficient stock for product {line.ProductName}. Requested: {line.Quantity}, Available: {If(stock IsNot Nothing, stock.CurrentQuantity, 0)}")
                End If
            Next

            Dim creditAccount As CreditAccount = Nothing

            If creditAccountId.HasValue Then
                creditAccount = Await _db.CreditAccounts.FindAsync(creditAccountId.Value)
                If creditAccount Is Nothing Then
                    Throw New InvalidOperationException("Credit account not found.")
                End If
                If creditAccount.IsBlocked Then
                    Throw New InvalidOperationException("Credit account is blocked.")
                End If
            End If

            Dim nextTxNumber = Await GenerateNextTransactionNumberAsync()

            Dim vatConfig = Await _mediator.Send(New GetVatConfigurationQuery())
            Dim vatDivisor As Decimal = If(vatConfig IsNot Nothing AndAlso vatConfig.VatRate > 0, 1D + vatConfig.VatRate, 1.12D)

            Dim vatableSales As Decimal = Math.Round(cart.TotalAmount / vatDivisor, 2, MidpointRounding.ToEven)
            Dim vatAmount As Decimal = cart.TotalAmount - vatableSales

            Dim transaction = New SalesTransaction With {
                .TransactionNumber = nextTxNumber,
                .TransactionDate = DateTime.Now,
                .TotalAmount = cart.TotalAmount,
                .PaymentMethod = paymentMethod,
                .IsVoid = False,
                .CreditAccount = creditAccount,
                .VatableSales = vatableSales
            }
            ' Note: SalesTransaction entity in this codebase does not have VatAmount but has VatableSales, VatExemptSales, ZeroRatedSales.

            For Each line In cart.Lines
                ' ProductId is load-bearing and must not be omitted. It used to be missing here, so
                ' every persisted line defaulted to 0: the sale-completion handler then deducted stock
                ' for product 0 and threw InsufficientStockException, and Accounting — which keys
                ' RevenueRecord on (SourceTransactionId, ProductId) — collapsed every line of a
                ' multi-line sale into a single row.
                transaction.Lines.Add(New SalesTransactionLine With {
                    .ProductId = line.ProductId,
                    .ProductName = line.ProductName,
                    .UnitPrice = line.UnitPrice,
                    .Quantity = line.Quantity,
                    .LineTotal = line.LineTotal
                })
            Next

            If creditAccount IsNot Nothing Then
                creditAccount.CurrentBalance += cart.TotalAmount
                If creditAccount.CurrentBalance > 0 Then
                    creditAccount.IsBlocked = True
                End If
            End If

            _db.SalesTransactions.Add(transaction)

            ' The number is derived from the current maximum, so two clients finalising at the same
            ' moment derive the same one and the unique index on TransactionNumber rejects the loser.
            ' That used to escape as an unhandled DbUpdateException and the sale was lost. Re-derive
            ' and retry instead — the number is the only thing that moves, so the retry re-inserts
            ' the same entity graph.
            For attempt = 1 To MaxTransactionNumberAttempts
                Dim duplicate As DbUpdateException = Nothing
                Try
                    Await _db.SaveChangesAsync()
                Catch ex As DbUpdateException When DuplicateKeyDetector.IsDuplicateKey(ex)
                    duplicate = ex
                End Try

                If duplicate Is Nothing Then
                    Exit For
                End If

                If attempt = MaxTransactionNumberAttempts Then
                    Throw New InvalidOperationException(
                        $"Could not allocate a unique transaction number after {MaxTransactionNumberAttempts} attempts.",
                        duplicate)
                End If

                transaction.TransactionNumber = Await GenerateNextTransactionNumberAsync()
            Next

            ClearCart(cartId)

            Return transaction
        End Function

        Public Async Function VoidTransactionAsync(transactionId As Integer) As Task Implements ICartService.VoidTransactionAsync
            Dim transaction = Await _db.SalesTransactions.Include(Function(t) t.CreditAccount).FirstOrDefaultAsync(Function(t) t.Id = transactionId)

            If transaction Is Nothing Then
                Throw New InvalidOperationException("Transaction not found.")
            End If

            If transaction.IsVoid Then
                Throw New InvalidOperationException("Transaction is already voided.")
            End If

            transaction.IsVoid = True

            If transaction.CreditAccount IsNot Nothing Then
                transaction.CreditAccount.CurrentBalance -= transaction.TotalAmount
                If transaction.CreditAccount.CurrentBalance <= 0 Then
                    transaction.CreditAccount.IsBlocked = False
                End If
            End If

            Await _db.SaveChangesAsync()
        End Function

        Public Async Function GetTransactionHistoryAsync() As Task(Of List(Of SalesTransaction)) Implements ICartService.GetTransactionHistoryAsync
            Dim transactionsAnonymous = Await _db.SalesTransactions _
                .Select(Function(t) New With {
                    t.Id,
                    t.TransactionNumber,
                    t.TransactionDate,
                    t.TotalAmount,
                    t.PaymentMethod,
                    t.IsVoid,
                    .CreditAccount = If(t.CreditAccount IsNot Nothing, New With { t.CreditAccount.Id, t.CreditAccount.CustomerName }, Nothing)
                }) _
                .OrderByDescending(Function(t) t.TransactionDate) _
                .ToListAsync()

            Dim transactions = New List(Of SalesTransaction)()
            For Each ta In transactionsAnonymous
                Dim t = New SalesTransaction With {
                    .Id = ta.Id,
                    .TransactionNumber = ta.TransactionNumber,
                    .TransactionDate = ta.TransactionDate,
                    .TotalAmount = ta.TotalAmount,
                    .PaymentMethod = ta.PaymentMethod,
                    .IsVoid = ta.IsVoid
                }
                If ta.CreditAccount IsNot Nothing Then
                    t.CreditAccount = New CreditAccount With {
                        .Id = ta.CreditAccount.Id,
                        .CustomerName = ta.CreditAccount.CustomerName
                    }
                End If
                transactions.Add(t)
            Next

            Return transactions
        End Function

        Private Async Function GenerateNextTransactionNumberAsync() As Task(Of String)
            Dim yearStr = DateTime.Now.Year.ToString()
            Dim prefix = $"TX-{yearStr}-"

            Dim latestTx = Await _db.SalesTransactions _
                .Where(Function(t) t.TransactionNumber.StartsWith(prefix)) _
                .OrderByDescending(Function(t) t.TransactionNumber) _
                .Select(Function(t) New With { t.TransactionNumber }) _
                .FirstOrDefaultAsync()

            Dim nextNum As Integer = 1
            If latestTx IsNot Nothing Then
                Dim lastNumStr = latestTx.TransactionNumber.Substring(latestTx.TransactionNumber.LastIndexOf("-"c) + 1)
                If Integer.TryParse(lastNumStr, nextNum) Then
                    nextNum += 1
                End If
            End If

            Return $"{prefix}{nextNum:D4}"
        End Function

    End Class

End Namespace
