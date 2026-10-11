Imports System.Threading.Tasks
Imports MediatR
Imports MerchSys.SharedKernel.Interfaces

Namespace Startup

    ''' <summary>
    ''' Default <see cref="IEventBus"/> implementation. Delegates to MediatR so that
    ''' <c>PublishAsync</c> reaches every registered <see cref="INotification"/> handler
    ''' (in-process, single client-server deployment). Registered by
    ''' <see cref="MediatRConfig.AddMediatRServices"/>.
    ''' </summary>
    Public NotInheritable Class MediatREventBus
        Implements IEventBus

        Private ReadOnly _mediator As IMediator

        Public Sub New(mediator As IMediator)
            _mediator = mediator
        End Sub

        Public Function PublishAsync(Of TEvent As INotification)([event] As TEvent) As Task Implements IEventBus.PublishAsync
            Return _mediator.Publish([event])
        End Function

    End Class

End Namespace
