Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Data
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Events
Imports MerchSys.POS.Entities

Namespace Services

    Public Class PaymentService
        Implements IPaymentService

        Private ReadOnly _dbContext As POSDbContext
        Private ReadOnly _eventBus As IEventBus

        Public Sub New(dbContext As POSDbContext, eventBus As IEventBus)
            _dbContext = dbContext
            _eventBus = eventBus
        End Sub

        Public Async Function ProcessPaymentAsync(transactionId As Integer, amountTendered As Decimal, method As String, referenceNumber As String) As Task(Of PaymentResultDto) Implements IPaymentService.ProcessPaymentAsync
            Dim transaction = Await _dbContext.SalesTransactions _
                .Include(Function(t) t.Lines) _
                .Include(Function(t) t.CreditAccount) _
                .FirstOrDefaultAsync(Function(t) t.Id = transactionId)

            If transaction Is Nothing Then
                Return New PaymentResultDto With { .Success = False, .ErrorMessage = "Transaction not found." }
            End If

            If transaction.IsVoid Then
                Return New PaymentResultDto With { .Success = False, .ErrorMessage = "Transaction is voided." }
            End If

            Dim changeAmount As Decimal = 0

            If method = "Credit" Then
                If transaction.CreditAccount Is Nothing Then
                    Return New PaymentResultDto With { .Success = False, .ErrorMessage = "Credit account not associated with transaction." }
                End If

                If transaction.CreditAccount.IsBlocked Then
                    Return New PaymentResultDto With { .Success = False, .ErrorMessage = "Credit account is blocked." }
                End If

                ' The whole amount goes to credit. If amountTendered is more than 0, it's not well defined how to handle mixed. Let's assume full credit.
                changeAmount = 0

                transaction.CreditAccount.CurrentBalance += transaction.TotalAmount
                transaction.CreditAccount.TotalCreditExtended += transaction.TotalAmount
                transaction.CreditAccount.LastTransactionDate = DateTime.Now

                If transaction.CreditAccount.CurrentBalance > 0 Then
                    transaction.CreditAccount.IsBlocked = True
                End If
            Else
                If amountTendered < transaction.TotalAmount Then
                    Return New PaymentResultDto With { .Success = False, .ErrorMessage = "Amount tendered is less than total amount." }
                End If
                changeAmount = amountTendered - transaction.TotalAmount
            End If

            transaction.PaymentMethod = method

            Dim receiptNumber = transaction.TransactionNumber ' Using TransactionNumber as basic receipt number

            Dim saleCompletedEvent = New SaleCompletedEvent With {
                .SaleId = transaction.Id,
                .SaleDate = transaction.TransactionDate,
                .Items = transaction.Lines.Select(Function(l) New SaleCompletedEvent.SaleCompletedItem With {
                    .ProductId = l.ProductId,
                    .QuantitySold = l.Quantity
                }).ToList()
            }

            Await _dbContext.SaveChangesAsync()

            Await _eventBus.PublishAsync(saleCompletedEvent)

            Return New PaymentResultDto With {
                .Success = True,
                .TransactionId = transaction.Id,
                .ChangeAmount = changeAmount,
                .ReceiptNumber = receiptNumber
            }
        End Function
    End Class

End Namespace
