Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel
Imports MerchSys.Inventory.Presenters

Namespace Views

    Public Partial Class ExpiryMonitorView
        Inherits UserControl
        Implements IExpiryMonitorView

        Public Sub New()
            InitializeComponent()
            InitializeGrids()
        End Sub

        Private Sub InitializeGrids()
            dgvNearExpiry.AutoGenerateColumns = False
            dgvNearExpiry.AllowUserToAddRows = False
            dgvNearExpiry.ReadOnly = True

            dgvNearExpiry.Columns.Add(New DataGridViewTextBoxColumn() With {
                .DataPropertyName = "BatchId",
                .HeaderText = "Batch ID",
                .Width = 80
            })
            dgvNearExpiry.Columns.Add(New DataGridViewTextBoxColumn() With {
                .DataPropertyName = "ProductId",
                .HeaderText = "Product ID",
                .Width = 80
            })
            dgvNearExpiry.Columns.Add(New DataGridViewTextBoxColumn() With {
                .DataPropertyName = "ExpiryDate",
                .HeaderText = "Expiry Date",
                .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "d"},
                .Width = 100
            })
            dgvNearExpiry.Columns.Add(New DataGridViewTextBoxColumn() With {
                .DataPropertyName = "QuantityRemaining",
                .HeaderText = "Qty Remaining",
                .Width = 120
            })

            dgvExpired.AutoGenerateColumns = False
            dgvExpired.AllowUserToAddRows = False
            dgvExpired.ReadOnly = True

            dgvExpired.Columns.Add(New DataGridViewTextBoxColumn() With {
                .DataPropertyName = "BatchId",
                .HeaderText = "Batch ID",
                .Width = 80
            })
            dgvExpired.Columns.Add(New DataGridViewTextBoxColumn() With {
                .DataPropertyName = "ProductId",
                .HeaderText = "Product ID",
                .Width = 80
            })
            dgvExpired.Columns.Add(New DataGridViewTextBoxColumn() With {
                .DataPropertyName = "ExpiryDate",
                .HeaderText = "Expiry Date",
                .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "d"},
                .Width = 100
            })
            dgvExpired.Columns.Add(New DataGridViewTextBoxColumn() With {
                .DataPropertyName = "QuantityRemaining",
                .HeaderText = "Qty Remaining",
                .Width = 120
            })

            Dim btnCol As New DataGridViewButtonColumn()
            btnCol.HeaderText = "Action"
            btnCol.Text = "Write Off"
            btnCol.UseColumnTextForButtonValue = True
            btnCol.Name = "WriteOffCol"
            btnCol.Width = 100
            dgvExpired.Columns.Add(btnCol)
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property NearExpiryCount As Integer Implements IExpiryMonitorView.NearExpiryCount
            Get
                Dim count As Integer
                If Integer.TryParse(lblNearExpiry.Text.Replace("Near Expiry Count: ", ""), count) Then
                    Return count
                End If
                Return 0
            End Get
            Set(value As Integer)
                lblNearExpiry.Text = $"Near Expiry Count: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ExpiredCount As Integer Implements IExpiryMonitorView.ExpiredCount
            Get
                Dim count As Integer
                If Integer.TryParse(lblExpired.Text.Replace("Expired Count: ", ""), count) Then
                    Return count
                End If
                Return 0
            End Get
            Set(value As Integer)
                lblExpired.Text = $"Expired Count: {value}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property TotalValueAtRisk As Decimal Implements IExpiryMonitorView.TotalValueAtRisk
            Get
                Dim val As Decimal
                If Decimal.TryParse(lblTotalValueAtRisk.Text.Replace("Total Value At Risk: $", ""), val) Then
                    Return val
                End If
                Return 0D
            End Get
            Set(value As Decimal)
                lblTotalValueAtRisk.Text = $"Total Value At Risk: ${value:N2}"
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ThresholdDays As Integer Implements IExpiryMonitorView.ThresholdDays
            Get
                Return CInt(numThreshold.Value)
            End Get
            Set(value As Integer)
                numThreshold.Value = value
            End Set
        End Property

        Public Event RefreshRequested As EventHandler Implements IExpiryMonitorView.RefreshRequested
        Public Event ThresholdChanged As EventHandler Implements IExpiryMonitorView.ThresholdChanged
        Public Event WriteOffRequested As EventHandler(Of ExpiryActionArgs) Implements IExpiryMonitorView.WriteOffRequested

        Public Sub DisplayNearExpiry(batches As Object) Implements IExpiryMonitorView.DisplayNearExpiry
            dgvNearExpiry.DataSource = batches
        End Sub

        Public Sub DisplayExpired(batches As Object) Implements IExpiryMonitorView.DisplayExpired
            dgvExpired.DataSource = batches
        End Sub

        Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
            RaiseEvent RefreshRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub numThreshold_ValueChanged(sender As Object, e As EventArgs) Handles numThreshold.ValueChanged
            RaiseEvent ThresholdChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub dgvExpired_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvExpired.CellContentClick
            If e.RowIndex >= 0 AndAlso e.ColumnIndex = dgvExpired.Columns("WriteOffCol").Index Then
                Dim row = CType(dgvExpired.Rows(e.RowIndex).DataBoundItem, ExpiryRowItem)

                Dim result = MessageBox.Show($"Are you sure you want to write off {row.QuantityRemaining} units of Product {row.ProductId} from Batch {row.BatchId}?", "Confirm Write-Off", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                If result = DialogResult.Yes Then
                    RaiseEvent WriteOffRequested(Me, New ExpiryActionArgs With {
                        .BatchId = row.BatchId,
                        .ProductId = row.ProductId,
                        .Quantity = row.QuantityRemaining
                    })
                End If
            End If
        End Sub

        Private Sub dgvNearExpiry_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvNearExpiry.CellFormatting
            If e.RowIndex >= 0 AndAlso dgvNearExpiry.Rows(e.RowIndex).DataBoundItem IsNot Nothing Then
                Dim item = CType(dgvNearExpiry.Rows(e.RowIndex).DataBoundItem, ExpiryRowItem)
                If item.UrgencyLevel = 2 Then ' <= 7 days
                    e.CellStyle.BackColor = Color.LightCoral
                    e.CellStyle.ForeColor = Color.White
                End If
            End If
        End Sub
    End Class

End Namespace
