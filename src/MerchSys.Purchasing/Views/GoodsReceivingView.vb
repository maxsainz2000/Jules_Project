Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.Drawing
Imports MerchSys.Purchasing.Presenters

Namespace Views

    Public Partial Class GoodsReceivingView
        Inherits UserControl
        Implements IGoodsReceivingView

        Public Event PurchaseOrderSelected As EventHandler(Of Integer?) Implements IGoodsReceivingView.PurchaseOrderSelected
        Public Event ConfirmReceiptRequested As EventHandler Implements IGoodsReceivingView.ConfirmReceiptRequested

        Private _presenter As GoodsReceivingPresenter

        Public Sub New(presenter As GoodsReceivingPresenter)
            _presenter = presenter
            _presenter.View = Me
            InitializeComponent()
            SetupGrid()
        End Sub

        Protected Overrides Async Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            If Not DesignMode AndAlso _presenter IsNot Nothing Then
                Await _presenter.InitializeAsync()
            End If
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedPurchaseOrderId As Integer? Implements IGoodsReceivingView.SelectedPurchaseOrderId
            Get
                If cmbPurchaseOrders.SelectedItem IsNot Nothing Then
                    Return DirectCast(cmbPurchaseOrders.SelectedItem, POSelectorItem).Id
                End If
                Return Nothing
            End Get
            Set(value As Integer?)
                If value.HasValue Then
                    For Each item As POSelectorItem In cmbPurchaseOrders.Items
                        If item.Id = value.Value Then
                            cmbPurchaseOrders.SelectedItem = item
                            Exit For
                        End If
                    Next
                Else
                    cmbPurchaseOrders.SelectedItem = Nothing
                End If
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanConfirmReceipt As Boolean Implements IGoodsReceivingView.CanConfirmReceipt
            Get
                Return btnConfirmReceipt.Enabled
            End Get
            Set(value As Boolean)
                btnConfirmReceipt.Enabled = value
                pnlPlaceholder.Visible = Not value
                dgvLines.Visible = value
            End Set
        End Property

        Public Sub SetPurchaseOrders(pos As List(Of POSelectorItem)) Implements IGoodsReceivingView.SetPurchaseOrders
            cmbPurchaseOrders.DataSource = Nothing
            cmbPurchaseOrders.DataSource = pos
            cmbPurchaseOrders.DisplayMember = "DisplayText"
            cmbPurchaseOrders.ValueMember = "Id"
            cmbPurchaseOrders.SelectedIndex = -1
        End Sub

        Public Sub SetLineItems(lines As List(Of GRLineItem)) Implements IGoodsReceivingView.SetLineItems
            bindingSourceLines.DataSource = Nothing
            If lines IsNot Nothing AndAlso lines.Count > 0 Then
                bindingSourceLines.DataSource = lines
            End If
            dgvLines.DataSource = bindingSourceLines
            dgvLines.Refresh()
            UpdateRowStyling()
        End Sub

        Public Sub ShowMessage(message As String, title As String) Implements IGoodsReceivingView.ShowMessage
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        Public Sub ShowError(message As String) Implements IGoodsReceivingView.ShowError
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Private Sub SetupGrid()
            dgvLines.AutoGenerateColumns = False
            dgvLines.Columns.Clear()

            Dim colIndicator = New DataGridViewTextBoxColumn() With {
                .Name = "Indicator",
                .HeaderText = "!",
                .Width = 30,
                .ReadOnly = True,
                .DefaultCellStyle = New DataGridViewCellStyle() With {.Alignment = DataGridViewContentAlignment.MiddleCenter, .ForeColor = Color.Red, .Font = New Font(dgvLines.Font, FontStyle.Bold)}
            }

            Dim colProduct = New DataGridViewTextBoxColumn() With {
                .Name = "ProductName",
                .DataPropertyName = "ProductName",
                .HeaderText = "Product",
                .ReadOnly = True,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            }

            Dim colQtyOrdered = New DataGridViewTextBoxColumn() With {
                .Name = "QuantityOrdered",
                .DataPropertyName = "QuantityOrdered",
                .HeaderText = "Qty Ordered",
                .ReadOnly = True,
                .Width = 80
            }

            Dim colQtyReceived = New DataGridViewTextBoxColumn() With {
                .Name = "QuantityReceived",
                .DataPropertyName = "QuantityReceived",
                .HeaderText = "Qty Received",
                .Width = 80,
                .DefaultCellStyle = New DataGridViewCellStyle() With {.BackColor = Color.LightYellow}
            }

            Dim colUnitCost = New DataGridViewTextBoxColumn() With {
                .Name = "UnitCost",
                .DataPropertyName = "UnitCost",
                .HeaderText = "Unit Cost",
                .Width = 80,
                .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2", .BackColor = Color.LightYellow}
            }

            Dim colExpiry = New MerchSys.Purchasing.UI.Controls.DataGridViewDateTimePickerColumn() With {
                .Name = "ExpiryDate",
                .DataPropertyName = "ExpiryDate",
                .HeaderText = "Expiry Date",
                .Width = 100,
                .DefaultCellStyle = New DataGridViewCellStyle() With {.BackColor = Color.LightYellow}
            }

            Dim colNotes = New DataGridViewTextBoxColumn() With {
                .Name = "DiscrepancyNotes",
                .DataPropertyName = "DiscrepancyNotes",
                .HeaderText = "Discrepancy Notes",
                .Width = 150,
                .DefaultCellStyle = New DataGridViewCellStyle() With {.BackColor = Color.LightYellow}
            }

            Dim colVatClass = New DataGridViewComboBoxColumn() With {
                .Name = "VatClassification",
                .DataPropertyName = "VatClassification",
                .HeaderText = "VAT Class",
                .Width = 100,
                .DefaultCellStyle = New DataGridViewCellStyle() With {.BackColor = Color.LightYellow}
            }
            If _presenter IsNot Nothing Then
                colVatClass.DataSource = _presenter.VatTreatmentValues
            End If

            Dim colVatAmt = New DataGridViewTextBoxColumn() With {
                .Name = "VatAmount",
                .DataPropertyName = "VatAmount",
                .HeaderText = "VAT Amount",
                .Width = 80,
                .ReadOnly = True,
                .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2", .BackColor = Color.WhiteSmoke}
            }

            dgvLines.Columns.AddRange(New DataGridViewColumn() {colIndicator, colProduct, colQtyOrdered, colQtyReceived, colUnitCost, colExpiry, colNotes, colVatClass, colVatAmt})
        End Sub

        Private Sub cmbPurchaseOrders_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbPurchaseOrders.SelectedIndexChanged
            RaiseEvent PurchaseOrderSelected(Me, SelectedPurchaseOrderId)
        End Sub

        Private Sub btnConfirmReceipt_Click(sender As Object, e As EventArgs) Handles btnConfirmReceipt.Click
            dgvLines.EndEdit()
            RaiseEvent ConfirmReceiptRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub dgvLines_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvLines.CellValueChanged
            If e.RowIndex >= 0 Then
                UpdateRowStyling()
            End If
        End Sub

        Private Sub dgvLines_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgvLines.CurrentCellDirtyStateChanged
            If dgvLines.IsCurrentCellDirty AndAlso TypeOf dgvLines.CurrentCell Is MerchSys.Purchasing.UI.Controls.DataGridViewDateTimePickerCell Then
                dgvLines.CommitEdit(DataGridViewDataErrorContexts.Commit)
            End If
        End Sub

        Private Sub UpdateRowStyling()
            For Each row As DataGridViewRow In dgvLines.Rows
                If row.DataBoundItem IsNot Nothing Then
                    Dim item = DirectCast(row.DataBoundItem, GRLineItem)
                    If item.HasDiscrepancy Then
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 240, 200) ' Amber
                        row.Cells("Indicator").Value = "⚠"

                        If String.IsNullOrWhiteSpace(item.DiscrepancyNotes) Then
                            row.Cells("DiscrepancyNotes").Style.BackColor = Color.LightCoral
                        Else
                            row.Cells("DiscrepancyNotes").Style.BackColor = Color.LightYellow
                        End If
                    Else
                        row.DefaultCellStyle.BackColor = Color.White
                        row.Cells("Indicator").Value = ""
                        row.Cells("DiscrepancyNotes").Style.BackColor = Color.LightYellow
                    End If
                End If
            Next
        End Sub

        Private Sub dgvLines_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles dgvLines.DataBindingComplete
            UpdateRowStyling()
        End Sub
    End Class

End Namespace
