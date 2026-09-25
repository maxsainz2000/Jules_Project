Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Namespace Models
    Public Enum AppModule
        Purchasing
        Inventory
        POS
        Accounting
        DeveloperTools
    End Enum

    Public Class RailItem
        Implements INotifyPropertyChanged

        Public Property ModuleId As AppModule
        Public Property Abbreviation As String
        Public Property ToolTipText As String

        Private _isActive As Boolean
        Public Property IsActive As Boolean
            Get
                Return _isActive
            End Get
            Set(value As Boolean)
                If _isActive <> value Then
                    _isActive = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class
End Namespace
