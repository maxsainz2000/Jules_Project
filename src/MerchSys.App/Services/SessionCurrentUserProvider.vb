Imports MerchSys.SharedKernel.Interfaces

Namespace Services
    ''' <summary>
    ''' Adapts <see cref="ISessionService"/> to the minimal <see cref="ICurrentUserProvider"/>
    ''' contract the data layer uses for audit stamping (H4).
    ''' </summary>
    Public Class SessionCurrentUserProvider
        Implements ICurrentUserProvider

        Private ReadOnly _sessionService As ISessionService

        Public Sub New(sessionService As ISessionService)
            _sessionService = sessionService
        End Sub

        Public ReadOnly Property Username As String Implements ICurrentUserProvider.Username
            Get
                Return _sessionService.CurrentUsername
            End Get
        End Property
    End Class
End Namespace
