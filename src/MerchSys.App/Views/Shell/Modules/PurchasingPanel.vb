Imports System.Windows.Forms
Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.Purchasing.Views
Imports MerchSys.Purchasing.Presenters

Namespace Views.Shell.Modules
    Public Class PurchasingPanel
        Inherits UserControl

        Public Sub New(serviceProvider As IServiceProvider)
            InitializeComponent()
            Dock = DockStyle.Fill
            BackColor = System.Drawing.Color.Transparent
            
            Dim presenter = DirectCast(serviceProvider.GetService(GetType(PurchaseOrderListPresenter)), PurchaseOrderListPresenter)
            Dim poListView = DirectCast(presenter.View, PurchaseOrderListView)
            poListView.Dock = DockStyle.Fill
            Me.Controls.Add(poListView)
        End Sub
    End Class
End Namespace
