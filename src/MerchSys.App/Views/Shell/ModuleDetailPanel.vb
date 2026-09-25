Imports System.Windows.Forms
Imports MerchSys.App.Presenters
Imports MerchSys.App.Presenters.Shell
Imports MerchSys.App.Models

Namespace Views.Shell
    Public Class ModuleDetailPanel
        Inherits UserControl

        Private _presenter As MainWindowPresenter

        Public Sub New(connectionStatusPresenter As ConnectionStatusPresenter)
            InitializeComponent()
            Dock = DockStyle.Left
            Width = 220
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48)
            
            ConnectionStatusSlot.Controls.Add(connectionStatusPresenter.View)
            connectionStatusPresenter.View.Dock = DockStyle.Fill
            
            ' Add layout fix here
            pnlItems.SendToBack()
        End Sub

        Public Sub SetPresenter(presenter As MainWindowPresenter)
            _presenter = presenter
            AddHandler _presenter.ActiveModuleChanged, AddressOf OnActiveModuleChanged
            ' Initialize first view
            UpdateVisiblePanel(_presenter.ActiveModule)
        End Sub

        Private Sub OnActiveModuleChanged(sender As Object, moduleId As AppModule)
            UpdateVisiblePanel(moduleId)
        End Sub

        Public Sub UpdateVisiblePanel(moduleId As AppModule)
            lblModuleName.Text = _presenter.ActiveModuleName
            
            ' Dispose old controls to prevent memory leaks
            For Each ctrl As Control In pnlItems.Controls
                ctrl.Dispose()
            Next
            pnlItems.Controls.Clear()
            
            Dim newPanel As UserControl = Nothing
            Select Case moduleId
                Case AppModule.Purchasing
                    newPanel = New Modules.PurchasingPanel()
                Case AppModule.Inventory
                    newPanel = New Modules.InventoryPanel()
                Case AppModule.POS
                    newPanel = New Modules.PosPanel()
                Case AppModule.Accounting
                    newPanel = New Modules.AccountingPanel()
                Case AppModule.DeveloperTools
                    newPanel = New Modules.DeveloperToolsPanel()
            End Select
            
            If newPanel IsNot Nothing Then
                newPanel.Dock = DockStyle.Fill
                pnlItems.Controls.Add(newPanel)
            End If
        End Sub
    End Class
End Namespace
