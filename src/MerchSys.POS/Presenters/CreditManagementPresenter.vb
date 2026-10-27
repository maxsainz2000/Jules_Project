Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Persistence
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System.Linq
Imports MerchSys.POS.Services
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.POS.Views

Namespace Presenters

    Public Class CreditManagementPresenter
        Private ReadOnly _creditService As ICreditService
        Private ReadOnly _dbContext As POSDbContext
        Private _view As ICreditManagementView
        Private ReadOnly _conflictPresenter As IConflictPresenter
        Private _allAccounts As List(Of CreditAccount)
        Private _overdueAccountIds As HashSet(Of Integer)

        Public Sub New(creditService As ICreditService, dbContext As POSDbContext, Optional conflictPresenter As IConflictPresenter = Nothing)
            _creditService = creditService
            _dbContext = dbContext
            _allAccounts = New List(Of CreditAccount)()
            _overdueAccountIds = New HashSet(Of Integer)()
            _conflictPresenter = conflictPresenter
        End Sub

        Public Sub New(creditService As ICreditService, dbContext As POSDbContext)
            _creditService = creditService
            _dbContext = dbContext
            _allAccounts = New List(Of CreditAccount)()
            _overdueAccountIds = New HashSet(Of Integer)()
        End Sub

        Public Property View As ICreditManagementView
            Get
                Return _view
            End Get
            Set(value As ICreditManagementView)
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.Load, AddressOf OnLoad
                    AddHandler _view.FilterChanged, AddressOf OnFilterChanged
                    AddHandler _view.SearchTextChanged, AddressOf OnSearchTextChanged
                    AddHandler _view.AccountSelected, AddressOf OnAccountSelected
                    AddHandler _view.AddAccount, AddressOf OnAddAccount
                    AddHandler _view.RecordPayment, AddressOf OnRecordPayment
                End If
            End Set
        End Property

        Private Sub OnLoad(sender As Object, e As EventArgs)
            LoadDataAsync()
        End Sub

        Private Sub LoadDataAsync()
            If _view Is Nothing Then Return

            _creditService.GetTotalsAsync().ContinueWith(
                Sub(totalsTask)
                    If totalsTask.IsFaulted Then
                        Dim ex = totalsTask.Exception.GetBaseException()
                        _view.ErrorMessage = "Failed to load data: " & ex.Message
                        Return
                    End If
                    Dim totals = totalsTask.Result

                    _creditService.ListAccountsAsync().ContinueWith(
                        Sub(accountsTask)
                            If accountsTask.IsFaulted Then
                                Dim ex = accountsTask.Exception.GetBaseException()
                                _view.ErrorMessage = "Failed to load data: " & ex.Message
                                Return
                            End If
                            _allAccounts = accountsTask.Result

                            _creditService.GetOverdueAccountsAsync().ContinueWith(
                                Sub(overdueTask)
                                    If overdueTask.IsFaulted Then
                                        Dim ex = overdueTask.Exception.GetBaseException()
                                        _view.ErrorMessage = "Failed to load data: " & ex.Message
                                        Return
                                    End If
                                    Dim overdueAccounts = overdueTask.Result

                                    _view.TotalAR = totals.TotalOutstandingBalance
                                    _view.OverdueCount = overdueAccounts.Where(Function(a) a.CurrentBalance > 0).Count()
                                    _view.BlockedCount = _allAccounts.Where(Function(a) a.IsBlocked).Count()

                                    _overdueAccountIds = New HashSet(Of Integer)(overdueAccounts.Select(Function(a) a.Id))
                                    ApplyFilterAndSearch()
                                End Sub, TaskScheduler.FromCurrentSynchronizationContext())
                        End Sub, TaskScheduler.FromCurrentSynchronizationContext())
                End Sub, TaskScheduler.FromCurrentSynchronizationContext())
        End Sub

        Private Sub ApplyFilterAndSearch()
            If _view Is Nothing OrElse _allAccounts Is Nothing Then Return

            Dim filtered = _allAccounts.AsEnumerable()

            If Not String.IsNullOrEmpty(_view.FilterMode) AndAlso _view.FilterMode <> "All" Then
                If _view.FilterMode = "Blocked" Then
                    filtered = filtered.Where(Function(a) a.IsBlocked)
                ElseIf _view.FilterMode = "Overdue" Then
                    filtered = filtered.Where(Function(a) _overdueAccountIds.Contains(a.Id))
                End If
            End If

            If Not String.IsNullOrEmpty(_view.SearchTerm) Then
                Dim term = _view.SearchTerm.ToLowerInvariant()
                filtered = filtered.Where(Function(a) Not String.IsNullOrEmpty(a.CustomerName) AndAlso a.CustomerName.ToLowerInvariant().Contains(term))
            End If

            _view.Accounts = filtered.ToList()
        End Sub

        Private Sub OnFilterChanged(sender As Object, e As EventArgs)
            ApplyFilterAndSearch()
        End Sub

        Private Sub OnSearchTextChanged(sender As Object, e As EventArgs)
            ApplyFilterAndSearch()
        End Sub

        Private Sub OnAccountSelected(sender As Object, e As AccountSelectedEventArgs)
            If _view Is Nothing OrElse e Is Nothing Then Return

            Dim accountId = e.AccountId
            Dim account = _allAccounts.FirstOrDefault(Function(a) a.Id = accountId)
            If account IsNot Nothing Then
                _view.SelectedAccount = account
            End If

            Dim historyTask = _creditService.GetAccountHistoryAsync(accountId)

            historyTask.ContinueWith(
                Sub(t)
                    If t.IsFaulted Then
                        Dim ex = t.Exception.GetBaseException()
                        _view.ErrorMessage = "Failed to load account history: " & ex.Message
                        Return
                    End If

                    Dim payments = t.Result

                    ' Using DbContext sync for EF where to avoid task factory for DbSet in this simple case
                    ' But to avoid blocking UI thread with EF queries, let's wrap it in Task.Run.
                    Dim salesTask = Task.Factory.StartNew(
                        Function()
                            Return _dbContext.SalesTransactions. _
                                Where(Function(s) s.CreditAccount.Id = accountId AndAlso s.PaymentMethod = "Credit"). _
                                ToList()
                        End Function)

                    salesTask.ContinueWith(
                        Sub(st)
                            If st.IsFaulted Then
                                Dim ex = st.Exception.GetBaseException()
                                _view.ErrorMessage = "Failed to load sales history: " & ex.Message
                                Return
                            End If

                            Dim sales = st.Result
                            Dim history As New List(Of CreditTransactionItem)()

                            For Each p In payments
                                history.Add(New CreditTransactionItem With {
                                    .TransactionDate = p.CreatedAt,
                                    .Type = "Payment",
                                    .ReferenceNumber = "PAY-" & p.Id.ToString(),
                                    .Amount = p.Amount,
                                    .Details = "Payment via " & p.PaymentMethod
                                })
                            Next

                            For Each s In sales
                                history.Add(New CreditTransactionItem With {
                                    .TransactionDate = s.TransactionDate,
                                    .Type = "Purchase",
                                    .ReferenceNumber = s.TransactionNumber,
                                    .Amount = s.TotalAmount,
                                    .Details = "Credit Purchase"
                                })
                            Next

                            _view.TransactionHistory = history.OrderByDescending(Function(h) h.TransactionDate).ToList()
                        End Sub, TaskScheduler.FromCurrentSynchronizationContext())
                End Sub, TaskScheduler.FromCurrentSynchronizationContext())
        End Sub

        Private Sub OnAddAccount(sender As Object, e As AddAccountEventArgs)
            If _view Is Nothing OrElse e Is Nothing Then Return

            If String.IsNullOrWhiteSpace(e.CustomerName) Then
                _view.ErrorMessage = "Customer name is required."
                Return
            End If

            Dim newAccount As New CreditAccount With {
                .CustomerName = e.CustomerName,
                .CurrentBalance = 0,
                .IsBlocked = False,
                .TotalCreditExtended = 0
            }

            _creditService.CreateAccountAsync(newAccount).ContinueWith(
                Sub(t)
                    If t.IsFaulted Then
                        Dim ex = t.Exception.GetBaseException()
                        _view.ErrorMessage = "Failed to create account: " & ex.Message
                    Else
                        _view.Message = "Account created successfully."
                        LoadDataAsync()
                    End If
                End Sub, TaskScheduler.FromCurrentSynchronizationContext())
        End Sub

        Private Async Sub OnRecordPayment(sender As Object, e As RecordPaymentEventArgs)
            If _view Is Nothing OrElse e Is Nothing Then Return

            If e.Amount <= 0 Then
                _view.ErrorMessage = "Payment amount must be greater than zero."
                Return
            End If

            If String.Equals(e.PaymentMethod, "Credit", StringComparison.OrdinalIgnoreCase) Then
                _view.ErrorMessage = "Cannot use 'Credit' as a payment method for a credit payment."
                Return
            End If

            Dim account = _allAccounts.FirstOrDefault(Function(a) a.Id = e.AccountId)
            If account IsNot Nothing AndAlso e.Amount > account.CurrentBalance Then
                _view.ErrorMessage = "Payment amount cannot exceed the current balance."
                Return
            End If

            Try
                Dim work As Func(Of Task) = Function()
                    Return _creditService.RecordPaymentAsync(e.AccountId, e.Amount, e.PaymentMethod)
                End Function
                Dim refresh As Func(Of Task) = Function()
                    LoadDataAsync()
                    Return Task.CompletedTask
                End Function

                Dim success = Await ConcurrencyHelper.ExecuteWithConflictPromptAsync(work, refresh, _conflictPresenter)
                If success Then
                    _view.Message = "Payment recorded successfully."
                    LoadDataAsync()
                    Dim accountSelectedEventArgs As New AccountSelectedEventArgs With {
                        .AccountId = e.AccountId
                    }
                    OnAccountSelected(Me, accountSelectedEventArgs)
                End If
            Catch ex As Exception
                _view.ErrorMessage = "Failed to record payment: " & ex.Message
            End Try
        End Sub
    End Class

End Namespace
