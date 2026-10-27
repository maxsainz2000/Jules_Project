Imports System.Windows.Forms
Imports MerchSys.Purchasing.Dtos
Imports MerchSys.Purchasing.Presenters

Namespace Views.Purchasing
    Partial Public Class PurchasingDashboardView
        Implements IPurchasingDashboardView

        Private ReadOnly _presenter As PurchasingDashboardPresenter

        Public Sub New()
            InitializeComponent()
            SetupGrids()
        End Sub

        Public Sub New(presenter As PurchasingDashboardPresenter)
            Me.New()
            _presenter = presenter
            If _presenter IsNot Nothing Then
                _presenter.View = Me
            End If
        End Sub

        Private Sub SetupGrids()
            dgvPendingDeliveries.AutoGenerateColumns = False
            dgvPendingDeliveries.Columns.Clear()
            dgvPendingDeliveries.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "OrderNumber", .HeaderText = "Order #", .Width = 80})
            dgvPendingDeliveries.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "VendorName", .HeaderText = "Vendor", .Width = 150})
            dgvPendingDeliveries.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "OrderDate", .HeaderText = "Order Date", .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "d"}, .Width = 90})
            dgvPendingDeliveries.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "ExpectedDeliveryDate", .HeaderText = "Expected", .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "d"}, .Width = 90})
            dgvPendingDeliveries.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Status", .HeaderText = "Status", .Width = 90})
            dgvPendingDeliveries.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "TotalAmount", .HeaderText = "Amount", .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "C2"}, .Width = 90})

            dgvTopVendors.AutoGenerateColumns = False
            dgvTopVendors.Columns.Clear()
            dgvTopVendors.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "VendorName", .HeaderText = "Vendor Name", .Width = 150})
            dgvTopVendors.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "TotalSpend", .HeaderText = "Total Spend", .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "C2"}, .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill})
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            RaiseEvent ViewLoaded(Me, EventArgs.Empty)
        End Sub

        Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
            RaiseEvent RefreshRequested(Me, EventArgs.Empty)
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OutstandingAp As Decimal Implements IPurchasingDashboardView.OutstandingAp
            Get
                Dim val As Decimal
                If Decimal.TryParse(lblOutstandingApValue.Text, Globalization.NumberStyles.Currency, Nothing, val) Then
                    Return val
                End If
                Return 0
            End Get
            Set(value As Decimal)
                lblOutstandingApValue.Text = value.ToString("C2")
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OverdueApCount As Integer Implements IPurchasingDashboardView.OverdueApCount
            Get
                Dim val As Integer
                Integer.TryParse(lblOverdueApValue.Text, val)
                Return val
            End Get
            Set(value As Integer)
                lblOverdueApValue.Text = value.ToString()
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property PendingDeliveriesCount As Integer Implements IPurchasingDashboardView.PendingDeliveriesCount
            Get
                Dim val As Integer
                Integer.TryParse(lblPendingDeliveriesValue.Text, val)
                Return val
            End Get
            Set(value As Integer)
                lblPendingDeliveriesValue.Text = value.ToString()
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property ReorderSuggestionsCount As Integer Implements IPurchasingDashboardView.ReorderSuggestionsCount
            Get
                Dim val As Integer
                Integer.TryParse(lblReorderSuggestionsValue.Text, val)
                Return val
            End Get
            Set(value As Integer)
                lblReorderSuggestionsValue.Text = value.ToString()
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property ActiveVendorsCount As Integer Implements IPurchasingDashboardView.ActiveVendorsCount
            Get
                Dim val As Integer
                Integer.TryParse(lblActiveVendorsValue.Text, val)
                Return val
            End Get
            Set(value As Integer)
                lblActiveVendorsValue.Text = value.ToString()
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property AverageLeadTimeDays As Double Implements IPurchasingDashboardView.AverageLeadTimeDays
            Get
                Dim val As Double
                Double.TryParse(lblAvgLeadTimeValue.Text, val)
                Return val
            End Get
            Set(value As Double)
                lblAvgLeadTimeValue.Text = value.ToString("F1") & " days"
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsLoading As Boolean Implements IPurchasingDashboardView.IsLoading
            Get
                Return Not btnRefresh.Enabled
            End Get
            Set(value As Boolean)
                btnRefresh.Enabled = Not value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property ErrorMessage As String Implements IPurchasingDashboardView.ErrorMessage
            Get
                Return lblErrorMessage.Text
            End Get
            Set(value As String)
                lblErrorMessage.Text = value
                lblErrorMessage.Visible = Not String.IsNullOrEmpty(value)
            End Set
        End Property

        Public Sub SetPendingDeliveries(rows As IEnumerable(Of PendingDeliveryRowDto)) Implements IPurchasingDashboardView.SetPendingDeliveries
            dgvPendingDeliveries.DataSource = rows?.ToList()
        End Sub

        Public Sub SetTopVendorsBySpend(rows As IEnumerable(Of TopVendorSpendDto)) Implements IPurchasingDashboardView.SetTopVendorsBySpend
            dgvTopVendors.DataSource = rows?.ToList()
        End Sub

        Public Event ViewLoaded As EventHandler Implements IPurchasingDashboardView.ViewLoaded

        Public Event RefreshRequested As EventHandler Implements IPurchasingDashboardView.RefreshRequested
        Public Event NavigateRequested As EventHandler(Of String) Implements IPurchasingDashboardView.NavigateRequested
    End Class
End Namespace