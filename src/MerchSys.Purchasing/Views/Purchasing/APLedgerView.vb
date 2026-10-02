Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Presenters

Namespace Views.Purchasing
    Public Partial Class APLedgerView
        Inherits UserControl
        Implements IAPLedgerView

        Public Shadows Event Load As EventHandler Implements IAPLedgerView.Load
        Public Event FilterChanged As EventHandler Implements IAPLedgerView.FilterChanged
        Public Event VendorChanged As EventHandler Implements IAPLedgerView.VendorChanged
        Public Event RecordPaymentClicked As EventHandler Implements IAPLedgerView.RecordPaymentClicked

        Public Sub New()
            InitializeComponent()
            SetupGrid()

            AddHandler btnFilterAll.CheckedChanged, AddressOf OnFilterChanged
            AddHandler btnFilterOutstanding.CheckedChanged, AddressOf OnFilterChanged
            AddHandler btnFilterOverdue.CheckedChanged, AddressOf OnFilterChanged
            AddHandler btnFilterPaid.CheckedChanged, AddressOf OnFilterChanged

            AddHandler cboVendors.SelectedIndexChanged, AddressOf OnVendorChanged
            AddHandler btnRecordPayment.Click, AddressOf OnRecordPaymentClicked
            AddHandler dgvLedger.CellFormatting, AddressOf OnCellFormatting
            dgvLedger.Columns.Add(New DataGridViewCheckBoxColumn With {
                .DataPropertyName = "IsOverdue",
                .HeaderText = "Overdue",
                .Width = 60
            })
        End Sub

        Private Sub SetupGrid()
            dgvLedger.AutoGenerateColumns = False
            dgvLedger.Columns.Clear()

            dgvLedger.Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "VendorName",
                .HeaderText = "Vendor",
                .Width = 200
            })
            dgvLedger.Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "OrderNumber",
                .HeaderText = "Invoice/PO",
                .Width = 120
            })
            dgvLedger.Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "InvoiceDate",
                .HeaderText = "Invoice Date",
                .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "d" },
                .Width = 100
            })
            dgvLedger.Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "DueDate",
                .HeaderText = "Due Date",
                .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "d" },
                .Width = 100
            })
            dgvLedger.Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "TotalAmount",
                .HeaderText = "Total",
                .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "₱0.00", .Alignment = DataGridViewContentAlignment.MiddleRight },
                .Width = 120
            })
            dgvLedger.Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "AmountPaid",
                .HeaderText = "Paid",
                .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "₱0.00", .Alignment = DataGridViewContentAlignment.MiddleRight },
                .Width = 120
            })
            dgvLedger.Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "Balance",
                .HeaderText = "Balance",
                .DefaultCellStyle = New DataGridViewCellStyle With { .Format = "₱0.00", .Alignment = DataGridViewContentAlignment.MiddleRight },
                .Width = 120
            })
            dgvLedger.Columns.Add(New DataGridViewCheckBoxColumn With {
                .DataPropertyName = "IsPaid",
                .HeaderText = "Paid",
                .Width = 60
            })
            dgvLedger.Columns.Add(New DataGridViewCheckBoxColumn With {
                .DataPropertyName = "IsOverdue",
                .HeaderText = "Overdue",
                .Width = 60
            })
        End Sub

        Private Sub OnFilterChanged(sender As Object, e As EventArgs)
            RaiseEvent FilterChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub OnVendorChanged(sender As Object, e As EventArgs)
            RaiseEvent VendorChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub OnRecordPaymentClicked(sender As Object, e As EventArgs)
            RaiseEvent RecordPaymentClicked(Me, EventArgs.Empty)
        End Sub

        Private Sub OnCellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
            If e.RowIndex >= 0 AndAlso e.RowIndex < dgvLedger.Rows.Count Then
                Dim row = DirectCast(dgvLedger.Rows(e.RowIndex).DataBoundItem, APLedgerRow)
                If row IsNot Nothing Then
                    If row.IsPaid Then
                        dgvLedger.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.LightGreen
                    ElseIf row.IsOverdue Then
                        dgvLedger.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.Orange
                    Else
                        dgvLedger.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.White
                    End If
                End If
            End If
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Vendors As IEnumerable(Of VendorSelectorItem) Implements IAPLedgerView.Vendors
            Get
                Return DirectCast(cboVendors.DataSource, IEnumerable(Of VendorSelectorItem))
            End Get
            Set(value As IEnumerable(Of VendorSelectorItem))
                cboVendors.DataSource = Nothing
                cboVendors.DataSource = value
                cboVendors.DisplayMember = "Name"
                cboVendors.ValueMember = "Id"
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedVendorId As Integer Implements IAPLedgerView.SelectedVendorId
            Get
                If cboVendors.SelectedValue IsNot Nothing Then
                    Return CInt(cboVendors.SelectedValue)
                End If
                Return 0
            End Get
            Set(value As Integer)
                cboVendors.SelectedValue = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsAllFilterActive As Boolean Implements IAPLedgerView.IsAllFilterActive
            Get
                Return btnFilterAll.Checked
            End Get
            Set(value As Boolean)
                btnFilterAll.Checked = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsOutstandingFilterActive As Boolean Implements IAPLedgerView.IsOutstandingFilterActive
            Get
                Return btnFilterOutstanding.Checked
            End Get
            Set(value As Boolean)
                btnFilterOutstanding.Checked = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsOverdueFilterActive As Boolean Implements IAPLedgerView.IsOverdueFilterActive
            Get
                Return btnFilterOverdue.Checked
            End Get
            Set(value As Boolean)
                btnFilterOverdue.Checked = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsPaidFilterActive As Boolean Implements IAPLedgerView.IsPaidFilterActive
            Get
                Return btnFilterPaid.Checked
            End Get
            Set(value As Boolean)
                btnFilterPaid.Checked = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property LedgerRows As IEnumerable(Of APLedgerRow) Implements IAPLedgerView.LedgerRows
            Get
                Return DirectCast(dgvLedger.DataSource, IEnumerable(Of APLedgerRow))
            End Get
            Set(value As IEnumerable(Of APLedgerRow))
                dgvLedger.DataSource = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property TotalOutstanding As Decimal Implements IAPLedgerView.TotalOutstanding
            Get
                Return 0 ' Write-only in UI effectively
            End Get
            Set(value As Decimal)
                lblTotalOutstanding.Text = $"Outstanding: ₱{value:N2}"
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property SelectedEntryId As Integer Implements IAPLedgerView.SelectedEntryId
            Get
                If dgvLedger.SelectedRows.Count > 0 Then
                    Dim row = DirectCast(dgvLedger.SelectedRows(0).DataBoundItem, APLedgerRow)
                    If row IsNot Nothing Then
                        Return row.Id
                    End If
                End If
                Return 0
            End Get
        End Property
    End Class
End Namespace
