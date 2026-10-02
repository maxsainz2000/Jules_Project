Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views
Imports MerchSys.Purchasing.Views.Dialogs
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Enums
Imports Microsoft.Extensions.DependencyInjection

Namespace Presenters
    Public Class APLedgerRow
        Public Property Id As Integer
        Public Property VendorId As Integer
        Public Property VendorName As String
        Public Property OrderNumber As String
        Public Property InvoiceDate As DateTime
        Public Property DueDate As DateTime
        Public Property TotalAmount As Decimal
        Public Property AmountPaid As Decimal
        Public Property Balance As Decimal
        Public Property IsPaid As Boolean
        Public Property IsOverdue As Boolean
    End Class

    Public Class VendorSelectorItem
        Public Property Id As Integer
        Public Property Name As String

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

    Public Class APLedgerPresenter
        Private ReadOnly _apService As IAccountsPayableService
        Private ReadOnly _vendorService As IVendorService
        Private ReadOnly _sessionService As ISessionService
        Private ReadOnly _serviceProvider As IServiceProvider

        Private _view As IAPLedgerView
        Private _allEntries As List(Of AccountsPayableDto) = New List(Of AccountsPayableDto)()

        Public Property View As IAPLedgerView
            Get
                Return _view
            End Get
            Set(value As IAPLedgerView)
                _view = value
                If _view IsNot Nothing Then
                    AddHandler _view.Load, AddressOf OnViewLoad
                    AddHandler _view.FilterChanged, AddressOf OnFilterChanged
                    AddHandler _view.VendorChanged, AddressOf OnFilterChanged
                    AddHandler _view.RecordPaymentClicked, AddressOf OnRecordPaymentClicked
                End If
            End Set
        End Property

        Public Sub New(apService As IAccountsPayableService, vendorService As IVendorService, sessionService As ISessionService, serviceProvider As IServiceProvider)
            _apService = apService
            _vendorService = vendorService
            _sessionService = sessionService
            _serviceProvider = serviceProvider
        End Sub

        Private Async Sub OnViewLoad(sender As Object, e As EventArgs)
            Await LoadVendorsAsync()
            Await LoadDataAsync()
        End Sub

        Private Async Function LoadVendorsAsync() As Task
            Dim vendors = Await _vendorService.GetAllAsync()
            Dim selectorItems = New List(Of VendorSelectorItem) From {
                New VendorSelectorItem With { .Id = 0, .Name = "All Vendors" }
            }
            selectorItems.AddRange(vendors.Select(Function(v) New VendorSelectorItem With { .Id = v.Id, .Name = v.Name }))

            _view.Vendors = selectorItems
            _view.SelectedVendorId = 0
        End Function

        Private Async Function LoadDataAsync() As Task
            _allEntries = Await _apService.GetAllAsync()
            ApplyFilters()
        End Function

        Private Sub OnFilterChanged(sender As Object, e As EventArgs)
            ApplyFilters()
        End Sub

        Private Sub ApplyFilters()
            Dim filtered = _allEntries.AsEnumerable()

            If _view.SelectedVendorId > 0 Then
                filtered = filtered.Where(Function(x) x.VendorId = _view.SelectedVendorId)
            End If

            If _view.IsOutstandingFilterActive Then
                filtered = filtered.Where(Function(x) Not x.IsPaid)
            ElseIf _view.IsOverdueFilterActive Then
                filtered = filtered.Where(Function(x) Not x.IsPaid AndAlso x.DueDate < DateTime.UtcNow.Date)
            ElseIf _view.IsPaidFilterActive Then
                filtered = filtered.Where(Function(x) x.IsPaid)
            End If

            Dim rows = filtered.Select(Function(x) New APLedgerRow With {
                .Id = x.Id,
                .VendorId = x.VendorId,
                .VendorName = x.VendorName,
                .OrderNumber = x.OrderNumber,
                .InvoiceDate = x.InvoiceDate,
                .DueDate = x.DueDate,
                .TotalAmount = x.TotalAmount,
                .AmountPaid = x.AmountPaid,
                .Balance = x.TotalAmount - x.AmountPaid,
                .IsPaid = x.IsPaid,
                .IsOverdue = Not x.IsPaid AndAlso x.DueDate < DateTime.UtcNow.Date
            }).ToList()

            _view.LedgerRows = rows
            _view.TotalOutstanding = _allEntries.Where(Function(x) Not x.IsPaid).Sum(Function(x) x.TotalAmount - x.AmountPaid)
        End Sub

        Private Async Sub OnRecordPaymentClicked(sender As Object, e As EventArgs)
            If _sessionService.CurrentUserRole <> UserRole.Manager AndAlso _sessionService.CurrentUserRole <> UserRole.Developer Then
                MessageBox.Show("Only managers or developers can record payments.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim selectedId = _view.SelectedEntryId
            If selectedId <= 0 Then
                MessageBox.Show("Please select an entry to record a payment.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim entry = _allEntries.FirstOrDefault(Function(x) x.Id = selectedId)
            If entry Is Nothing OrElse entry.IsPaid Then
                MessageBox.Show("This entry is already fully paid.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Using dialog = CType(_serviceProvider.GetRequiredService(Of IAPLedgerPaymentDialog)(), Form)
                Dim paymentView = DirectCast(dialog, IAPLedgerPaymentDialog)
                paymentView.InvoiceDetails = $"Invoice: {entry.OrderNumber} - {entry.VendorName}"
                paymentView.Balance = entry.TotalAmount - entry.AmountPaid
                paymentView.PaymentAmount = entry.TotalAmount - entry.AmountPaid ' Default to full balance

                If dialog.ShowDialog() = DialogResult.OK Then
                    Dim amount = paymentView.PaymentAmount
                    If amount <= 0 OrElse amount > (entry.TotalAmount - entry.AmountPaid) Then
                        MessageBox.Show("Invalid payment amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If

                    Await _apService.RecordPaymentAsync(selectedId, amount)
                    Await LoadDataAsync()
                End If
            End Using
        End Sub
    End Class
End Namespace
