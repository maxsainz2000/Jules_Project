Imports System.Threading
Imports MerchSys.SharedKernel.Interfaces

Namespace Data

    Public Class WriteContextScope
        Implements IWriteContextScope

        Private Class ScopeFrame
            Public Property Kind As WriteContextKind
            Public Property Parent As ScopeFrame
        End Class

        Private ReadOnly _currentScope As New AsyncLocal(Of ScopeFrame)

        Public ReadOnly Property Current As WriteContextKind Implements IWriteContextScope.Current
            Get
                Dim frame = _currentScope.Value
                If frame IsNot Nothing Then
                    Return frame.Kind
                End If
                Return WriteContextKind.User
            End Get
        End Property

        Public Function Enter(kind As WriteContextKind) As IDisposable Implements IWriteContextScope.Enter
            Dim currentFrame = _currentScope.Value
            Dim newFrame As New ScopeFrame With {
                .Kind = kind,
                .Parent = currentFrame
            }
            _currentScope.Value = newFrame
            Return New ScopeReleaser(Me, currentFrame)
        End Function

        Private Class ScopeReleaser
            Implements IDisposable

            Private ReadOnly _scope As WriteContextScope
            Private ReadOnly _parentToRestore As ScopeFrame
            Private _disposed As Boolean

            Public Sub New(scope As WriteContextScope, parentToRestore As ScopeFrame)
                _scope = scope
                _parentToRestore = parentToRestore
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                If Not _disposed Then
                    _scope._currentScope.Value = _parentToRestore
                    _disposed = True
                End If
            End Sub
        End Class

    End Class

End Namespace
