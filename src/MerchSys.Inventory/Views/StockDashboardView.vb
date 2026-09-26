Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel
Imports MerchSys.Inventory.Services

Namespace Views

    <ToolboxItem(True)>
    Public Partial Class StockDashboardView
        Inherits UserControl
        Implements IStockDashboardView

        ' UI Components
        Private _lblTotalStockValue As Label
        Private _lblLowStockCount As Label
        Private _lblExpiredBatchCount As Label
        Private _lblCriticalRiskCount As Label
        Private _lblTotalProducts As Label

        Private _txtFilter As TextBox
        Private _cboCategoryFilter As ComboBox
        Private _cboStatusFilter As ComboBox

        Private WithEvents _dgvProducts As DataGridView
        Private _dgvBatches As DataGridView

        Public Event FilterChanged As EventHandler Implements IStockDashboardView.FilterChanged
        Public Event ProductSelected As EventHandler(Of Integer) Implements IStockDashboardView.ProductSelected

        Public Sub New()
            InitializeComponent()
            SetupLayout()
        End Sub

        Private Sub InitializeComponent()
            _lblTotalStockValue = New Label()
            _lblLowStockCount = New Label()
            _lblExpiredBatchCount = New Label()
            _lblCriticalRiskCount = New Label()
            _lblTotalProducts = New Label()

            _txtFilter = New TextBox()
            _cboCategoryFilter = New ComboBox()
            _cboStatusFilter = New ComboBox()

            _dgvProducts = New DataGridView()
            _dgvBatches = New DataGridView()

            ' Basic component init to avoid null references in designer
            SuspendLayout()

            _txtFilter.Width = 200

            _cboCategoryFilter.Items.Add("All")
            _cboCategoryFilter.SelectedIndex = 0

            _cboStatusFilter.Items.Add("All")
            _cboStatusFilter.Items.Add("Low Stock")
            _cboStatusFilter.Items.Add("Critical Risk")
            _cboStatusFilter.SelectedIndex = 0

            _dgvProducts.AutoGenerateColumns = False
            _dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            _dgvProducts.ReadOnly = True
            _dgvProducts.AllowUserToAddRows = False

            ' Setup columns for _dgvProducts
            _dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "ProductId", .HeaderText = "ID", .Name = "ProductId"})
            _dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "ProductName", .HeaderText = "Name", .Name = "ProductName"})
            _dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "SKU", .HeaderText = "SKU", .Name = "SKU"})
            _dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "CurrentQuantity", .HeaderText = "Quantity", .Name = "CurrentQuantity"})
            _dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "TotalValue", .HeaderText = "Value", .Name = "TotalValue"})
            _dgvProducts.Columns.Add(New DataGridViewCheckBoxColumn() With {.DataPropertyName = "IsBelowThreshold", .HeaderText = "Low Stock?", .Name = "IsBelowThreshold"})
            _dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "RiskLevel", .HeaderText = "Risk", .Name = "RiskLevel"})

            _dgvBatches.AutoGenerateColumns = False
            _dgvBatches.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            _dgvBatches.ReadOnly = True
            _dgvBatches.AllowUserToAddRows = False

            ' Setup columns for _dgvBatches
            _dgvBatches.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "BatchId", .HeaderText = "Batch ID"})
            _dgvBatches.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "QuantityRemaining", .HeaderText = "Remaining"})
            _dgvBatches.Columns.Add(New DataGridViewTextBoxColumn() With {.DataPropertyName = "ExpiryDate", .HeaderText = "Expiry Date"})

            ' Layout logic would be added here
            ' For now just add to controls to satisfy WinForms
            Controls.Add(_lblTotalStockValue)
            Controls.Add(_lblLowStockCount)
            Controls.Add(_lblExpiredBatchCount)
            Controls.Add(_lblCriticalRiskCount)
            Controls.Add(_lblTotalProducts)
            Controls.Add(_txtFilter)
            Controls.Add(_cboCategoryFilter)
            Controls.Add(_cboStatusFilter)
            Controls.Add(_dgvProducts)
            Controls.Add(_dgvBatches)

            ' Event bindings
            AddHandler _txtFilter.TextChanged, Sub(sender, e) RaiseEvent FilterChanged(Me, EventArgs.Empty)
            AddHandler _cboCategoryFilter.SelectedIndexChanged, Sub(sender, e) RaiseEvent FilterChanged(Me, EventArgs.Empty)
            AddHandler _cboStatusFilter.SelectedIndexChanged, Sub(sender, e) RaiseEvent FilterChanged(Me, EventArgs.Empty)

            AddHandler _dgvProducts.SelectionChanged, AddressOf OnProductSelectionChanged

            ResumeLayout(False)
        End Sub

        Private Sub SetupLayout()
            ' Basic Layout
            Dim yPos = 10

            _lblTotalStockValue.Location = New Point(10, yPos)
            _lblLowStockCount.Location = New Point(150, yPos)
            _lblExpiredBatchCount.Location = New Point(300, yPos)
            _lblCriticalRiskCount.Location = New Point(450, yPos)
            _lblTotalProducts.Location = New Point(600, yPos)

            yPos += 30

            _txtFilter.Location = New Point(10, yPos)
            _cboCategoryFilter.Location = New Point(220, yPos)
            _cboStatusFilter.Location = New Point(350, yPos)

            yPos += 30

            _dgvProducts.Location = New Point(10, yPos)
            _dgvProducts.Size = New Size(700, 200)

            yPos += 210

            _dgvBatches.Location = New Point(10, yPos)
            _dgvBatches.Size = New Size(700, 150)

            Me.Size = New Size(720, yPos + 160)
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TotalStockValue As Decimal Implements IStockDashboardView.TotalStockValue
            Get
                Return 0
            End Get
            Set(value As Decimal)
                _lblTotalStockValue.Text = $"Value: {value:C}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property LowStockCount As Integer Implements IStockDashboardView.LowStockCount
            Get
                Return 0
            End Get
            Set(value As Integer)
                _lblLowStockCount.Text = $"Low Stock: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ExpiredBatchCount As Integer Implements IStockDashboardView.ExpiredBatchCount
            Get
                Return 0
            End Get
            Set(value As Integer)
                _lblExpiredBatchCount.Text = $"Expired: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property CriticalRiskCount As Integer Implements IStockDashboardView.CriticalRiskCount
            Get
                Return 0
            End Get
            Set(value As Integer)
                _lblCriticalRiskCount.Text = $"Critical: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TotalProducts As Integer Implements IStockDashboardView.TotalProducts
            Get
                Return 0
            End Get
            Set(value As Integer)
                _lblTotalProducts.Text = $"Total: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property FilterText As String Implements IStockDashboardView.FilterText
            Get
                Return _txtFilter.Text
            End Get
            Set(value As String)
                _txtFilter.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property CategoryFilter As String Implements IStockDashboardView.CategoryFilter
            Get
                Return If(_cboCategoryFilter.SelectedItem?.ToString(), "All")
            End Get
            Set(value As String)
                _cboCategoryFilter.SelectedItem = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property StatusFilter As String Implements IStockDashboardView.StatusFilter
            Get
                Return If(_cboStatusFilter.SelectedItem?.ToString(), "All")
            End Get
            Set(value As String)
                _cboStatusFilter.SelectedItem = value
            End Set
        End Property

        Public Sub SetProductRows(rows As IEnumerable(Of Presenters.ProductRowItem)) Implements IStockDashboardView.SetProductRows
            If InvokeRequired Then
                Invoke(New Action(Of IEnumerable(Of Presenters.ProductRowItem))(AddressOf SetProductRows), rows)
                Return
            End If

            _dgvProducts.DataSource = Nothing
            _dgvProducts.DataSource = rows?.ToList()
        End Sub

        Public Sub SetBatchRows(rows As IEnumerable(Of StockBatchSummaryDto)) Implements IStockDashboardView.SetBatchRows
            If InvokeRequired Then
                Invoke(New Action(Of IEnumerable(Of StockBatchSummaryDto))(AddressOf SetBatchRows), rows)
                Return
            End If

            _dgvBatches.DataSource = Nothing
            _dgvBatches.DataSource = rows?.ToList()
        End Sub

        Private Sub OnProductSelectionChanged(sender As Object, e As EventArgs)
            If _dgvProducts.SelectedRows.Count > 0 Then
                Dim row = _dgvProducts.SelectedRows(0)
                Dim item = TryCast(row.DataBoundItem, Presenters.ProductRowItem)
                If item IsNot Nothing Then
                    RaiseEvent ProductSelected(Me, item.ProductId)
                End If
            End If
        End Sub

        Private Sub _dgvProducts_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles _dgvProducts.CellFormatting
            If e.RowIndex >= 0 AndAlso _dgvProducts.Columns(e.ColumnIndex).Name = "RiskLevel" Then
                Dim rowItem = TryCast(_dgvProducts.Rows(e.RowIndex).DataBoundItem, Presenters.ProductRowItem)
                If rowItem IsNot Nothing Then
                    If rowItem.RiskLevel = StockoutRiskLevel.Critical Then
                        e.CellStyle.BackColor = Color.Red
                        e.CellStyle.ForeColor = Color.White
                    ElseIf rowItem.RiskLevel = StockoutRiskLevel.High Then
                        e.CellStyle.BackColor = Color.Orange
                    End If
                End If
            End If
        End Sub

    End Class

End Namespace
