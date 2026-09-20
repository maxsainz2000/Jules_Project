Imports System.Threading.Tasks
Imports MediatR

Namespace Interfaces

    Public Interface IEventBus
        Function PublishAsync(Of TEvent As INotification)([event] As TEvent) As Task
    End Interface

End Namespace
