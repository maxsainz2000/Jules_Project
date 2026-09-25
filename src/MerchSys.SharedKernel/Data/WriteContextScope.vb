Imports System.Threading
Imports MerchSys.SharedKernel.Interfaces

Namespace Data
    Public Class WriteContextScope
        Implements IWriteContextScope

        Private Class Frame
            Public Property Kind As WriteContextKind
            Public Property Previous As Frame
        End Class

        Private Shared ReadOnly _currentFrame As New AsyncLocal(Of Frame)()

        Public ReadOnly Property CurrentKind As WriteContextKind Implements IWriteContextScope.CurrentKind
            Get
                Dim frame = _currentFrame.Value
                If frame Is Nothing Then Return WriteContextKind.User
                Return frame.Kind
            End Get
        End Property

        Public Function Enter(kind As WriteContextKind) As IDisposable Implements IWriteContextScope.Enter
            Dim previousFrame = _currentFrame.Value
            _currentFrame.Value = New Frame With {
                .Kind = kind,
                .Previous = previousFrame
            }
            Return New Releaser(previousFrame)
        End Function

        Private Class Releaser
            Implements IDisposable

            Private ReadOnly _previous As Frame
            Private _disposed As Boolean = False

            Public Sub New(previous As Frame)
                _previous = previous
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                If Not _disposed Then
                    _currentFrame.Value = _previous
                    _disposed = True
                End If
            End Sub
        End Class
    End Class
End Namespace
