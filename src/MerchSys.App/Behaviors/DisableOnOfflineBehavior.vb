Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Runtime.CompilerServices
Imports MerchSys.App.Services

Namespace Behaviors
    <ProvideProperty("IsDisabledWhenOffline", GetType(Control))>
    Public Class DisableOnOfflineBehavior
        Inherits Component
        Implements IExtenderProvider

        Private ReadOnly _controls As New ConditionalWeakTable(Of Control, BooleanWrapper)()

        Private _monitorSubscribed As Boolean = False

        Public Sub New()
            If Not DesignMode Then
                SubscribeToMonitor()
            End If
        End Sub

        Public Sub New(container As IContainer)
            Me.New()
            container.Add(Me)
        End Sub

        Public Function CanExtend(extendee As Object) As Boolean Implements IExtenderProvider.CanExtend
            Return TypeOf extendee Is Control
        End Function

        <Category("Behavior")>
        <Description("Determines whether the control should be disabled when the database connection is offline.")>
        Public Function GetIsDisabledWhenOffline(control As Control) As Boolean
            Dim wrapper As BooleanWrapper = Nothing
            If _controls.TryGetValue(control, wrapper) Then
                Return wrapper.Value
            End If
            Return False
        End Function

        Public Sub SetIsDisabledWhenOffline(control As Control, value As Boolean)
            Dim wrapper As BooleanWrapper = Nothing
            If _controls.TryGetValue(control, wrapper) Then
                wrapper.Value = value
            Else
                _controls.Add(control, New BooleanWrapper(value))
            End If

            If Not DesignMode Then
                If Not _monitorSubscribed Then
                    SubscribeToMonitor()
                End If
                If value Then
                    Dim monitor = ConnectionHealthMonitorLocator.Current
                    Dim state = If(monitor IsNot Nothing, monitor.CurrentState, ConnectionState.Online)
                    UpdateControlState(control, state = ConnectionState.Online)
                End If
            End If
        End Sub

        Private Sub SubscribeToMonitor()
            Dim monitor = ConnectionHealthMonitorLocator.Current
            If monitor IsNot Nothing AndAlso Not _monitorSubscribed Then
                AddHandler monitor.StateChanged, AddressOf OnStateChanged
                _monitorSubscribed = True
            End If
        End Sub

        Private Sub OnStateChanged(sender As Object, e As ConnectionStateChangedEventArgs)
            Dim isOnline = e.State = ConnectionState.Online
            ' Update all tracked controls
            For Each kvp In _controls
                Dim control = kvp.Key
                Dim wrapper = kvp.Value
                If wrapper.Value Then
                    UpdateControlState(control, isOnline)
                End If
            Next
        End Sub

        Private Sub UpdateControlState(control As Control, isOnline As Boolean)
            If control.IsDisposed OrElse control.Disposing Then Return

            If control.InvokeRequired Then
                Try
                    control.Invoke(Sub() UpdateControlState(control, isOnline))
                Catch ex As ObjectDisposedException
                    ' Ignore if control is disposed while invoking
                End Try
            Else
                control.Enabled = isOnline
            End If
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                Dim monitor = ConnectionHealthMonitorLocator.Current
                If monitor IsNot Nothing AndAlso _monitorSubscribed Then
                    RemoveHandler monitor.StateChanged, AddressOf OnStateChanged
                    _monitorSubscribed = False
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Class BooleanWrapper
            Public Property Value As Boolean
            Public Sub New(val As Boolean)
                Value = val
            End Sub
        End Class
    End Class
End Namespace
