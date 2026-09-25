Imports System.Windows.Forms
Imports MerchSys.App.Models
Imports MerchSys.App.Views.Shell
Imports System.Collections.ObjectModel

Namespace Presenters.Shell
    Public Class ActivityRailPresenter
        Public Property View As ActivityRail
        Public Property Items As New ObservableCollection(Of RailItem)
        
        Public Sub New()
            Items.Add(New RailItem With { .ModuleId = AppModule.Purchasing, .Abbreviation = "PUR", .ToolTipText = "Purchasing" })
            Items.Add(New RailItem With { .ModuleId = AppModule.Inventory, .Abbreviation = "INV", .ToolTipText = "Inventory" })
            Items.Add(New RailItem With { .ModuleId = AppModule.POS, .Abbreviation = "POS", .ToolTipText = "Point of Sale" })
            Items.Add(New RailItem With { .ModuleId = AppModule.Accounting, .Abbreviation = "ACC", .ToolTipText = "Accounting" })
#If DEBUG Then
            Items.Add(New RailItem With { .ModuleId = AppModule.DeveloperTools, .Abbreviation = "DEV", .ToolTipText = "Developer Tools" })
#End If
        End Sub

        Public Sub SyncActiveModule(activeModule As AppModule)
            For Each item In Items
                item.IsActive = (item.ModuleId = activeModule)
            Next
        End Sub
    End Class
End Namespace
