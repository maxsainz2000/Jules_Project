Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.SharedKernel.Interfaces

Namespace Persistence
    Public Module ConcurrencyHelper
        Public Async Function ExecuteWithConflictPromptAsync(work As Func(Of Task),
                                                             onRefresh As Func(Of Task),
                                                             conflictPresenter As IConflictPresenter) As Task(Of Boolean)
            Dim isConflict As Boolean = False
            Try
                Await work()
            Catch ex As DbUpdateConcurrencyException
                isConflict = True
            End Try

            If isConflict Then
                If conflictPresenter IsNot Nothing Then
                    Dim shouldRefresh = Await conflictPresenter.PromptAsync()
                    If shouldRefresh AndAlso onRefresh IsNot Nothing Then
                        Await onRefresh()
                    End If
                End If
                Return False
            End If

            Return True
        End Function
    End Module
End Namespace
