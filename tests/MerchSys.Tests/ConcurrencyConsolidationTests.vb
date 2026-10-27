Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Moq
Imports Xunit
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Persistence

Public Class ConcurrencyConsolidationTests

    <Fact>
    Public Async Function ExecuteWithConflictPromptAsync_WhenWorkSucceeds_ReturnsTrue_And_DoesNotCallPromptOrRefresh() As Task
        Dim mockPresenter = New Mock(Of IConflictPresenter)()
        Dim workCalled = False
        Dim refreshCalled = False

        Dim work As Func(Of Task) = Function()
                                        workCalled = True
                                        Return Task.CompletedTask
                                    End Function

        Dim onRefresh As Func(Of Task) = Function()
                                             refreshCalled = True
                                             Return Task.CompletedTask
                                         End Function

        Dim result = Await ConcurrencyHelper.ExecuteWithConflictPromptAsync(work, onRefresh, mockPresenter.Object)

        Assert.True(result)
        Assert.True(workCalled)
        Assert.False(refreshCalled)
        mockPresenter.Verify(Function(p) p.PromptAsync(), Times.Never())
    End Function

    <Fact>
    Public Async Function ExecuteWithConflictPromptAsync_WhenConcurrencyExceptionAndPromptReturnsTrue_ReturnsFalse_And_CallsRefresh() As Task
        Dim mockPresenter = New Mock(Of IConflictPresenter)()
        mockPresenter.Setup(Function(p) p.PromptAsync()).ReturnsAsync(True)

        Dim workCalled = False
        Dim refreshCalled = False

        Dim work As Func(Of Task) = Function()
                                        workCalled = True
                                        Throw New DbUpdateConcurrencyException("Conflict!")
                                    End Function

        Dim onRefresh As Func(Of Task) = Function()
                                             refreshCalled = True
                                             Return Task.CompletedTask
                                         End Function

        Dim result = Await ConcurrencyHelper.ExecuteWithConflictPromptAsync(work, onRefresh, mockPresenter.Object)

        Assert.False(result)
        Assert.True(workCalled)
        Assert.True(refreshCalled)
        mockPresenter.Verify(Function(p) p.PromptAsync(), Times.Once())
    End Function

    <Fact>
    Public Async Function ExecuteWithConflictPromptAsync_WhenConcurrencyExceptionAndPromptReturnsFalse_ReturnsFalse_And_DoesNotCallRefresh() As Task
        Dim mockPresenter = New Mock(Of IConflictPresenter)()
        mockPresenter.Setup(Function(p) p.PromptAsync()).ReturnsAsync(False)

        Dim workCalled = False
        Dim refreshCalled = False

        Dim work As Func(Of Task) = Function()
                                        workCalled = True
                                        Throw New DbUpdateConcurrencyException("Conflict!")
                                    End Function

        Dim onRefresh As Func(Of Task) = Function()
                                             refreshCalled = True
                                             Return Task.CompletedTask
                                         End Function

        Dim result = Await ConcurrencyHelper.ExecuteWithConflictPromptAsync(work, onRefresh, mockPresenter.Object)

        Assert.False(result)
        Assert.True(workCalled)
        Assert.False(refreshCalled)
        mockPresenter.Verify(Function(p) p.PromptAsync(), Times.Once())
    End Function

    <Fact>
    Public Async Function ExecuteWithConflictPromptAsync_WhenNonConcurrencyException_Throws() As Task
        Dim mockPresenter = New Mock(Of IConflictPresenter)()

        Dim work As Func(Of Task) = Function()
                                        Throw New InvalidOperationException("Other error")
                                    End Function

        Dim onRefresh As Func(Of Task) = Function() Task.CompletedTask

        Await Assert.ThrowsAsync(Of InvalidOperationException)(Function() ConcurrencyHelper.ExecuteWithConflictPromptAsync(work, onRefresh, mockPresenter.Object))

        mockPresenter.Verify(Function(p) p.PromptAsync(), Times.Never())
    End Function

End Class
