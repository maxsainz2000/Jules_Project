Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.POS.Services
Imports MerchSys.POS.Views

Namespace Presenters

    Public Class TransactionHistoryPresenter

        Private ReadOnly _db As POSDbContext
        Private ReadOnly _cartService As ICartService
        Private ReadOnly _receiptService As IReceiptService
        Private ReadOnly _salesReturnService As ISalesReturnService
        Private _view As ITransactionHistoryView

        Public Sub New(db As POSDbContext, cartService As ICartService, receiptService As IReceiptService, salesReturnService As ISalesReturnService)
            _db = db
            _cartService = cartService
            _receiptService = receiptService
            _salesReturnService = salesReturnService
        End Sub

        Public Property View As ITransactionHistoryView
            Get
                Return _view
            End Get
            Set(value As ITransactionHistoryView)
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.Load, AddressOf OnSearchRequested
                    AddHandler _view.SearchRequested, AddressOf OnSearchRequested
                    AddHandler _view.TransactionSelected, AddressOf OnTransactionSelected
                    AddHandler _view.PrintReceiptRequested, AddressOf OnPrintReceiptRequested
                    AddHandler _view.ProcessReturnRequested, AddressOf OnProcessReturnRequested
                    AddHandler _view.VoidTransactionRequested, AddressOf OnVoidTransactionRequested
                End If
            End Set
        End Property

        Private Sub OnSearchRequested(sender As Object, e As EventArgs)
            If _view Is Nothing Then Return

            Dim scheduler = TaskScheduler.FromCurrentSynchronizationContext()

            Dim query = _db.SalesTransactions.Include(Function(t) t.Lines).Include(Function(t) t.Receipt).AsQueryable()

            If _view.SearchStartDate.HasValue Then
                Dim sd = _view.SearchStartDate.Value
                query = query.Where(Function(t) t.TransactionDate >= sd)
            End If

            If _view.SearchEndDate.HasValue Then
                Dim ed = _view.SearchEndDate.Value
                query = query.Where(Function(t) t.TransactionDate <= ed)
            End If

            If Not String.IsNullOrWhiteSpace(_view.SearchTransactionNumber) Then
                Dim num = _view.SearchTransactionNumber
                query = query.Where(Function(t) t.TransactionNumber.Contains(num))
            End If

            If Not String.IsNullOrWhiteSpace(_view.SearchPaymentMethod) Then
                Dim pmt = _view.SearchPaymentMethod
                query = query.Where(Function(t) t.PaymentMethod = pmt)
            End If

            query = query.OrderByDescending(Function(t) t.TransactionDate)

            Dim task = query.ToListAsync()
            task.ContinueWith(Sub(t)
                                  If t.IsFaulted Then
                                      _view.ErrorMessage = "Error loading transactions: " & t.Exception.GetBaseException().Message
                                      Return
                                  End If

                                  Dim returnsTask = _salesReturnService.GetReturnHistoryAsync()
                                  returnsTask.ContinueWith(Sub(rt)
                                                               If rt.IsFaulted Then
                                                                   _view.ErrorMessage = "Error loading returns: " & rt.Exception.GetBaseException().Message
                                                                   Return
                                                               End If

                                                               Dim allReturns = rt.Result
                                                               Dim resultList As New List(Of TransactionSummaryItem)()

                                                               For Each tx In t.Result
                                                                   Dim item As New TransactionSummaryItem With {
                                                                       .TransactionId = tx.Id,
                                                                       .TransactionNumber = tx.TransactionNumber,
                                                                       .TransactionDate = tx.TransactionDate,
                                                                       .PaymentMethod = tx.PaymentMethod,
                                                                       .TotalAmount = tx.TotalAmount,
                                                                       .IsVoid = tx.IsVoid,
                                                                       .ReturnStatus = "None"
                                                                   }

                                                                   Dim txReturns = allReturns.Where(Function(r) r.SalesTransactionId = tx.Id).ToList()
                                                                   If txReturns.Any() Then
                                                                       item.ReturnStatus = "Returned"
                                                                   End If
                                                                   resultList.Add(item)
                                                               Next

                                                               _view.Transactions = resultList

                                                           End Sub, scheduler)
                              End Sub, scheduler)
        End Sub

        Private Sub OnTransactionSelected(sender As Object, e As TransactionSelectedEventArgs)
            If _view Is Nothing Then Return
            Dim scheduler = TaskScheduler.FromCurrentSynchronizationContext()

            Dim txId = e.TransactionId

            _db.SalesTransactions.Include(Function(t) t.Lines).Include(Function(t) t.Receipt).FirstOrDefaultAsync(Function(t) t.Id = txId).ContinueWith(
                Sub(t)
                    If t.IsFaulted Then
                        _view.ErrorMessage = "Error loading transaction details: " & t.Exception.GetBaseException().Message
                        Return
                    End If

                    Dim tx = t.Result
                    If tx Is Nothing Then Return

                    Dim returnsTask = _salesReturnService.GetReturnsForTransactionAsync(txId)
                    returnsTask.ContinueWith(
                        Sub(rt)
                            If rt.IsFaulted Then
                                _view.ErrorMessage = "Error loading returns: " & rt.Exception.GetBaseException().Message
                                Return
                            End If

                            Dim existingReturns = rt.Result

                            ' Since SalesReturn does not currently store returned quantities per line, we set AvailableForReturn to the original Quantity.
                            ' In a real-world scenario, this would require querying past returns line-by-line if supported.
                            Dim detailLines As New List(Of TransactionDetailLine)()
                            For Each line In tx.Lines
                                detailLines.Add(New TransactionDetailLine With {
                                    .LineId = line.Id,
                                    .ProductId = line.ProductId,
                                    .ProductName = line.ProductName,
                                    .UnitPrice = line.UnitPrice,
                                    .Quantity = line.Quantity,
                                    .LineTotal = line.LineTotal,
                                    .ReturnedQuantity = 0,
                                    .AvailableForReturn = line.Quantity
                                })
                            Next

                            Dim returnSummaries As New List(Of ReturnSummaryItem)()
                            For Each r In existingReturns
                                returnSummaries.Add(New ReturnSummaryItem With {
                                    .ReturnId = r.Id,
                                    .ReturnDate = r.CreatedAt,
                                    .Reason = r.Reason,
                                    .RefundAmount = 0, ' Without line items, difficult to calc. Service layer doesn't return this info on GetReturnsForTransactionAsync.
                                    .IsRestocked = r.IsRestocked
                                })
                            Next

                            _view.SelectedTransaction = New TransactionSummaryItem With {
                                .TransactionId = tx.Id,
                                .TransactionNumber = tx.TransactionNumber,
                                .TransactionDate = tx.TransactionDate,
                                .PaymentMethod = tx.PaymentMethod,
                                .TotalAmount = tx.TotalAmount,
                                .IsVoid = tx.IsVoid
                            }
                            _view.SelectedTransactionLines = detailLines
                            _view.SelectedTransactionReturns = returnSummaries

                            If tx.Receipt IsNot Nothing Then
                                _receiptService.PrintReceiptAsync(tx.Receipt.Id).ContinueWith(
                                    Sub(pr)
                                        If pr.IsFaulted Then
                                            _view.ErrorMessage = "Error generating receipt preview: " & pr.Exception.GetBaseException().Message
                                        Else
                                            _view.ReceiptPreview = pr.Result
                                        End If
                                    End Sub, scheduler)
                            Else
                                _view.ReceiptPreview = "No receipt generated."
                            End If

                        End Sub, scheduler)
                End Sub, scheduler)
        End Sub

        Private Sub OnPrintReceiptRequested(sender As Object, e As PrintReceiptRequestedEventArgs)
            If _view Is Nothing Then Return
            Dim scheduler = TaskScheduler.FromCurrentSynchronizationContext()

            Dim txId = e.TransactionId

            _receiptService.GetReceiptByTransactionAsync(txId).ContinueWith(
                Sub(t)
                    If t.IsFaulted Then
                        _view.ErrorMessage = "Error checking receipt: " & t.Exception.GetBaseException().Message
                        Return
                    End If

                    Dim receipt = t.Result
                    If receipt Is Nothing Then
                        _receiptService.GenerateReceiptAsync(txId).ContinueWith(
                            Sub(gt)
                                If gt.IsFaulted Then
                                    _view.ErrorMessage = "Error generating receipt: " & gt.Exception.GetBaseException().Message
                                    Return
                                End If

                                _receiptService.PrintReceiptAsync(gt.Result.Id).ContinueWith(
                                    Sub(pr)
                                        If pr.IsFaulted Then
                                            _view.ErrorMessage = "Error generating receipt preview: " & pr.Exception.GetBaseException().Message
                                        Else
                                            _view.ReceiptPreview = pr.Result
                                            _view.Message = "Receipt generated successfully."
                                        End If
                                    End Sub, scheduler)
                            End Sub, scheduler)
                    Else
                        _receiptService.PrintReceiptAsync(receipt.Id).ContinueWith(
                            Sub(pr)
                                If pr.IsFaulted Then
                                    _view.ErrorMessage = "Error generating receipt preview: " & pr.Exception.GetBaseException().Message
                                Else
                                    _view.ReceiptPreview = pr.Result
                                    _view.Message = "Receipt printed."
                                End If
                            End Sub, scheduler)
                    End If

                End Sub, scheduler)
        End Sub

        Private Sub OnProcessReturnRequested(sender As Object, e As ProcessReturnRequestedEventArgs)
            If _view Is Nothing Then Return
            Dim scheduler = TaskScheduler.FromCurrentSynchronizationContext()

            If String.IsNullOrWhiteSpace(e.Reason) Then
                _view.ErrorMessage = "Return reason is required."
                Return
            End If

            If e.ReturnQuantities Is Nothing OrElse Not e.ReturnQuantities.Any(Function(kv) kv.Value > 0) Then
                _view.ErrorMessage = "No items selected to return."
                Return
            End If

            If _view.SelectedTransaction IsNot Nothing AndAlso _view.SelectedTransactionLines IsNot Nothing Then
                For Each kvp In e.ReturnQuantities
                    Dim line = _view.SelectedTransactionLines.FirstOrDefault(Function(l) l.LineId = kvp.Key)
                    If line IsNot Nothing AndAlso kvp.Value > line.AvailableForReturn Then
                        _view.ErrorMessage = $"Return quantity for {line.ProductName} exceeds available quantity."
                        Return
                    End If
                Next
            End If

            _salesReturnService.ProcessReturnAsync(e.TransactionId, e.ReturnQuantities, e.Reason, e.IsRestocked).ContinueWith(
                Sub(t)
                    If t.IsFaulted Then
                        _view.ErrorMessage = "Error processing return: " & t.Exception.GetBaseException().Message
                        Return
                    End If

                    _view.Message = "Return processed successfully."

                    ' Refresh search results and selected transaction
                    OnSearchRequested(Me, EventArgs.Empty)
                    OnTransactionSelected(Me, New TransactionSelectedEventArgs With {.TransactionId = e.TransactionId})
                End Sub, scheduler)
        End Sub

        Private Sub OnVoidTransactionRequested(sender As Object, e As VoidTransactionRequestedEventArgs)
            If _view Is Nothing Then Return
            Dim scheduler = TaskScheduler.FromCurrentSynchronizationContext()

            _cartService.VoidTransactionAsync(e.TransactionId).ContinueWith(
                Sub(t)
                    If t.IsFaulted Then
                        _view.ErrorMessage = "Error voiding transaction: " & t.Exception.GetBaseException().Message
                        Return
                    End If

                    _view.Message = "Transaction voided successfully."
                    OnSearchRequested(Me, EventArgs.Empty)
                    OnTransactionSelected(Me, New TransactionSelectedEventArgs With {.TransactionId = e.TransactionId})
                End Sub, scheduler)
        End Sub

    End Class

End Namespace
