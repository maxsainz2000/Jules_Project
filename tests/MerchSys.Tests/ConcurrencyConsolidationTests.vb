Imports System
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Windows.Forms
Imports Microsoft.EntityFrameworkCore
Imports Moq
Imports Xunit
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Persistence
Imports MerchSys.Purchasing.Dtos
Imports MerchSys.Purchasing.Presenters
Imports MerchSys.Purchasing.Services
Imports MerchSys.Purchasing.Views

Public Class ConcurrencyConsolidationTests
    <Fact>
    Public Async Function ExecuteWithConflictPromptAsync_ReturnsTrue_WhenNoException() As Task
        ' Arrange
        Dim mockPresenter = New Mock(Of IConflictPresenter)()
        Dim workCalled = False
        Dim refreshCalled = False

        Dim work As Func(Of Task) = Async Function()
                                        workCalled = True
                                        Await Task.Yield()
                                    End Function
        Dim refresh As Func(Of Task) = Async Function()
                                           refreshCalled = True
                                           Await Task.Yield()
                                       End Function

        ' Act
        Dim result = Await ConcurrencyHelper.ExecuteWithConflictPromptAsync(work, refresh, mockPresenter.Object)

        ' Assert
        Assert.True(result)
        Assert.True(workCalled)
        Assert.False(refreshCalled)
        mockPresenter.Verify(Function(p) p.PromptAsync(), Times.Never())
    End Function

    <Fact>
    Public Async Function ExecuteWithConflictPromptAsync_ReturnsFalse_AndNoRefresh_WhenConflictAndCancel() As Task
        ' Arrange
        Dim mockPresenter = New Mock(Of IConflictPresenter)()
        mockPresenter.Setup(Function(p) p.PromptAsync()).ReturnsAsync(False)
        Dim refreshCalled = False

        Dim work As Func(Of Task) = Async Function()
                                        Await Task.Yield()
                                        Throw New DbUpdateConcurrencyException("Conflict")
                                    End Function
        Dim refresh As Func(Of Task) = Async Function()
                                           refreshCalled = True
                                           Await Task.Yield()
                                       End Function

        ' Act
        Dim result = Await ConcurrencyHelper.ExecuteWithConflictPromptAsync(work, refresh, mockPresenter.Object)

        ' Assert
        Assert.False(result)
        Assert.False(refreshCalled)
        mockPresenter.Verify(Function(p) p.PromptAsync(), Times.Once())
    End Function

    <Fact>
    Public Async Function ExecuteWithConflictPromptAsync_ReturnsFalse_AndRefreshes_WhenConflictAndRefresh() As Task
        ' Arrange
        Dim mockPresenter = New Mock(Of IConflictPresenter)()
        mockPresenter.Setup(Function(p) p.PromptAsync()).ReturnsAsync(True)
        Dim refreshCalled = False

        Dim work As Func(Of Task) = Async Function()
                                        Await Task.Yield()
                                        Throw New DbUpdateConcurrencyException("Conflict")
                                    End Function
        Dim refresh As Func(Of Task) = Async Function()
                                           refreshCalled = True
                                           Await Task.Yield()
                                       End Function

        ' Act
        Dim result = Await ConcurrencyHelper.ExecuteWithConflictPromptAsync(work, refresh, mockPresenter.Object)

        ' Assert
        Assert.False(result)
        Assert.True(refreshCalled)
        mockPresenter.Verify(Function(p) p.PromptAsync(), Times.Once())
    End Function
End Class

Public Class FakePaymentDialog
    Inherits Form
    Implements IAPLedgerPaymentDialog

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property InvoiceDetails As String Implements IAPLedgerPaymentDialog.InvoiceDetails
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Balance As Decimal Implements IAPLedgerPaymentDialog.Balance
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property PaymentAmount As Decimal Implements IAPLedgerPaymentDialog.PaymentAmount

    Public Shadows Function ShowDialog() As DialogResult
        Return DialogResult.OK
    End Function
End Class

Public Class PresenterIntegrationTests
    <Fact>
    Public Async Function APLedgerPresenter_UsesConflictPresenter_OnConcurrencyConflict() As Task
        ' Arrange
        Dim mockApService = New Mock(Of IAccountsPayableService)()
        Dim mockVendorService = New Mock(Of IVendorService)()
        Dim mockSessionService = New Mock(Of ISessionService)()
        Dim mockServiceProvider = New Mock(Of IServiceProvider)()
        Dim mockConflictPresenter = New Mock(Of IConflictPresenter)()
        Dim mockView = New Mock(Of IAPLedgerView)()

        mockSessionService.Setup(Function(s) s.CurrentUserRole).Returns(MerchSys.SharedKernel.Enums.UserRole.Manager)

        Dim fakeDialog = New FakePaymentDialog() With { .PaymentAmount = 100 }
        mockServiceProvider.Setup(Function(sp) sp.GetService(GetType(IAPLedgerPaymentDialog))).Returns(fakeDialog)

        ' Force the service to throw conflict
        mockApService.Setup(Function(s) s.RecordPaymentAsync(It.IsAny(Of Integer)(), It.IsAny(Of Decimal)())).ThrowsAsync(New DbUpdateConcurrencyException("Conflict"))

        ' Presenter setup
        Dim presenter = New APLedgerPresenter(mockApService.Object, mockVendorService.Object, mockSessionService.Object, mockServiceProvider.Object, mockConflictPresenter.Object)
        presenter.View = mockView.Object

        ' Seed an entry so selectedId is valid
        Dim entries = New List(Of AccountsPayableDto) From {
            New AccountsPayableDto With { .Id = 1, .TotalAmount = 200, .AmountPaid = 0, .IsPaid = False }
        }
        mockApService.Setup(Function(s) s.GetAllAsync()).ReturnsAsync(entries)

        ' Simulate LoadDataAsync
        mockView.Raise(Sub(v) AddHandler v.Load, Nothing, Nothing, EventArgs.Empty)
        Await Task.Yield()

        mockView.Setup(Function(v) v.SelectedEntryId).Returns(1)

        ' Act
        mockView.Raise(Sub(v) AddHandler v.RecordPaymentClicked, Nothing, Nothing, EventArgs.Empty)
        Await Task.Delay(50) ' Ensure async execution in presenter completes

        ' Assert
        mockConflictPresenter.Verify(Function(p) p.PromptAsync(), Times.Once())
    End Function
End Class
