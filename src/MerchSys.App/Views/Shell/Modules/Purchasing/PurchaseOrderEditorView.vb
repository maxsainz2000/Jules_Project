Imports System.Windows.Forms
Imports MerchSys.Purchasing.Entities

Namespace Views.Shell.Modules.Purchasing
    Public Class PurchaseOrderEditorView
        Inherits System.Windows.Forms.Form
        Implements IPurchaseOrderEditorView

        Private _presenter As PurchaseOrderEditorPresenter

        Private cboVendor As System.Windows.Forms.ComboBox
        Private lblVendor As System.Windows.Forms.Label
        Private txtOrderNumber As System.Windows.Forms.TextBox
        Private lblOrderNumber As System.Windows.Forms.Label
        Private lblStatusTitle As System.Windows.Forms.Label
        Private lblStatus As System.Windows.Forms.Label
        Private dtpExpectedDeliveryDate As System.Windows.Forms.DateTimePicker
        Private lblExpectedDeliveryDate As System.Windows.Forms.Label
        Private txtNotes As System.Windows.Forms.TextBox
        Private lblNotes As System.Windows.Forms.Label
        Private dgvLines As System.Windows.Forms.DataGridView
        Private btnAddLine As System.Windows.Forms.Button
        Private btnRemoveLine As System.Windows.Forms.Button
        Private lblTotalAmount As System.Windows.Forms.Label
        Private btnSave As System.Windows.Forms.Button
        Private btnSubmit As System.Windows.Forms.Button

        Private colProductName As System.Windows.Forms.DataGridViewTextBoxColumn
        Private colUnitCost As System.Windows.Forms.DataGridViewTextBoxColumn
        Private colLineTotal As System.Windows.Forms.DataGridViewTextBoxColumn

        Public Sub New()
            InitializeComponentLocal()
        End Sub

        Private Sub InitializeComponentLocal()
            Me.cboVendor = New System.Windows.Forms.ComboBox()
            Me.lblVendor = New System.Windows.Forms.Label()
            Me.txtOrderNumber = New System.Windows.Forms.TextBox()
            Me.lblOrderNumber = New System.Windows.Forms.Label()
            Me.lblStatusTitle = New System.Windows.Forms.Label()
            Me.lblStatus = New System.Windows.Forms.Label()
            Me.dtpExpectedDeliveryDate = New System.Windows.Forms.DateTimePicker()
            Me.lblExpectedDeliveryDate = New System.Windows.Forms.Label()
            Me.txtNotes = New System.Windows.Forms.TextBox()
            Me.lblNotes = New System.Windows.Forms.Label()
            Me.dgvLines = New System.Windows.Forms.DataGridView()
            Me.colProductName = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colUnitCost = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.colLineTotal = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.btnAddLine = New System.Windows.Forms.Button()
            Me.btnRemoveLine = New System.Windows.Forms.Button()
            Me.btnSave = New System.Windows.Forms.Button()
            Me.btnSubmit = New System.Windows.Forms.Button()
            Me.lblTotalAmount = New System.Windows.Forms.Label()
            CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()

            Me.lblVendor.AutoSize = True
            Me.lblVendor.Location = New System.Drawing.Point(12, 15)
            Me.lblVendor.Name = "lblVendor"
            Me.lblVendor.Size = New System.Drawing.Size(47, 15)
            Me.lblVendor.TabIndex = 0
            Me.lblVendor.Text = "Vendor:"

            Me.cboVendor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboVendor.FormattingEnabled = True
            Me.cboVendor.Location = New System.Drawing.Point(100, 12)
            Me.cboVendor.Name = "cboVendor"
            Me.cboVendor.Size = New System.Drawing.Size(200, 23)
            Me.cboVendor.TabIndex = 1

            Me.lblOrderNumber.AutoSize = True
            Me.lblOrderNumber.Location = New System.Drawing.Point(12, 44)
            Me.lblOrderNumber.Name = "lblOrderNumber"
            Me.lblOrderNumber.Size = New System.Drawing.Size(61, 15)
            Me.lblOrderNumber.TabIndex = 2
            Me.lblOrderNumber.Text = "Order No:"

            Me.txtOrderNumber.Location = New System.Drawing.Point(100, 41)
            Me.txtOrderNumber.Name = "txtOrderNumber"
            Me.txtOrderNumber.ReadOnly = True
            Me.txtOrderNumber.Size = New System.Drawing.Size(200, 23)
            Me.txtOrderNumber.TabIndex = 3

            Me.lblStatusTitle.AutoSize = True
            Me.lblStatusTitle.Location = New System.Drawing.Point(320, 44)
            Me.lblStatusTitle.Name = "lblStatusTitle"
            Me.lblStatusTitle.Size = New System.Drawing.Size(42, 15)
            Me.lblStatusTitle.TabIndex = 4
            Me.lblStatusTitle.Text = "Status:"

            Me.lblStatus.AutoSize = True
            Me.lblStatus.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblStatus.Location = New System.Drawing.Point(368, 44)
            Me.lblStatus.Name = "lblStatus"
            Me.lblStatus.Size = New System.Drawing.Size(35, 15)
            Me.lblStatus.TabIndex = 5
            Me.lblStatus.Text = "Draft"

            Me.lblExpectedDeliveryDate.AutoSize = True
            Me.lblExpectedDeliveryDate.Location = New System.Drawing.Point(320, 15)
            Me.lblExpectedDeliveryDate.Name = "lblExpectedDeliveryDate"
            Me.lblExpectedDeliveryDate.Size = New System.Drawing.Size(83, 15)
            Me.lblExpectedDeliveryDate.TabIndex = 6
            Me.lblExpectedDeliveryDate.Text = "Expected Del:"

            Me.dtpExpectedDeliveryDate.Checked = False
            Me.dtpExpectedDeliveryDate.Format = System.Windows.Forms.DateTimePickerFormat.Short
            Me.dtpExpectedDeliveryDate.Location = New System.Drawing.Point(409, 12)
            Me.dtpExpectedDeliveryDate.Name = "dtpExpectedDeliveryDate"
            Me.dtpExpectedDeliveryDate.ShowCheckBox = True
            Me.dtpExpectedDeliveryDate.Size = New System.Drawing.Size(120, 23)
            Me.dtpExpectedDeliveryDate.TabIndex = 7

            Me.lblNotes.AutoSize = True
            Me.lblNotes.Location = New System.Drawing.Point(12, 73)
            Me.lblNotes.Name = "lblNotes"
            Me.lblNotes.Size = New System.Drawing.Size(41, 15)
            Me.lblNotes.TabIndex = 8
            Me.lblNotes.Text = "Notes:"

            Me.txtNotes.Location = New System.Drawing.Point(100, 70)
            Me.txtNotes.Name = "txtNotes"
            Me.txtNotes.Size = New System.Drawing.Size(429, 23)
            Me.txtNotes.TabIndex = 9

            Me.dgvLines.AllowUserToAddRows = False
            Me.dgvLines.AllowUserToDeleteRows = False
            Me.dgvLines.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.dgvLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvLines.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colProductName, Me.colUnitCost, Me.colLineTotal})
            Me.dgvLines.Location = New System.Drawing.Point(12, 108)
            Me.dgvLines.Name = "dgvLines"
            Me.dgvLines.ReadOnly = True
            Me.dgvLines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvLines.Size = New System.Drawing.Size(517, 200)
            Me.dgvLines.TabIndex = 10

            Me.colProductName.DataPropertyName = "ProductName"
            Me.colProductName.HeaderText = "Product Name"
            Me.colProductName.Name = "colProductName"
            Me.colProductName.ReadOnly = True

            Me.colUnitCost.DataPropertyName = "UnitCost"
            Me.colUnitCost.HeaderText = "Unit Cost"
            Me.colUnitCost.Name = "colUnitCost"
            Me.colUnitCost.ReadOnly = True

            Me.colLineTotal.DataPropertyName = "LineTotal"
            Me.colLineTotal.HeaderText = "Line Total"
            Me.colLineTotal.Name = "colLineTotal"
            Me.colLineTotal.ReadOnly = True

            Me.btnAddLine.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.btnAddLine.Location = New System.Drawing.Point(12, 314)
            Me.btnAddLine.Name = "btnAddLine"
            Me.btnAddLine.Size = New System.Drawing.Size(75, 23)
            Me.btnAddLine.TabIndex = 11
            Me.btnAddLine.Text = "Add Line"
            Me.btnAddLine.UseVisualStyleBackColor = True

            Me.btnRemoveLine.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.btnRemoveLine.Location = New System.Drawing.Point(93, 314)
            Me.btnRemoveLine.Name = "btnRemoveLine"
            Me.btnRemoveLine.Size = New System.Drawing.Size(89, 23)
            Me.btnRemoveLine.TabIndex = 12
            Me.btnRemoveLine.Text = "Remove Line"
            Me.btnRemoveLine.UseVisualStyleBackColor = True

            Me.lblTotalAmount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblTotalAmount.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblTotalAmount.Location = New System.Drawing.Point(286, 311)
            Me.lblTotalAmount.Name = "lblTotalAmount"
            Me.lblTotalAmount.Size = New System.Drawing.Size(243, 23)
            Me.lblTotalAmount.TabIndex = 13
            Me.lblTotalAmount.Text = "Total: ₱0.00"
            Me.lblTotalAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight

            Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnSave.Location = New System.Drawing.Point(373, 350)
            Me.btnSave.Name = "btnSave"
            Me.btnSave.Size = New System.Drawing.Size(75, 23)
            Me.btnSave.TabIndex = 14
            Me.btnSave.Text = "Save Draft"
            Me.btnSave.UseVisualStyleBackColor = True

            Me.btnSubmit.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnSubmit.Location = New System.Drawing.Point(454, 350)
            Me.btnSubmit.Name = "btnSubmit"
            Me.btnSubmit.Size = New System.Drawing.Size(75, 23)
            Me.btnSubmit.TabIndex = 15
            Me.btnSubmit.Text = "Submit"
            Me.btnSubmit.UseVisualStyleBackColor = True

            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(541, 385)
            Me.Controls.Add(Me.btnSubmit)
            Me.Controls.Add(Me.btnSave)
            Me.Controls.Add(Me.lblTotalAmount)
            Me.Controls.Add(Me.btnRemoveLine)
            Me.Controls.Add(Me.btnAddLine)
            Me.Controls.Add(Me.dgvLines)
            Me.Controls.Add(Me.txtNotes)
            Me.Controls.Add(Me.lblNotes)
            Me.Controls.Add(Me.dtpExpectedDeliveryDate)
            Me.Controls.Add(Me.lblExpectedDeliveryDate)
            Me.Controls.Add(Me.lblStatus)
            Me.Controls.Add(Me.lblStatusTitle)
            Me.Controls.Add(Me.txtOrderNumber)
            Me.Controls.Add(Me.lblOrderNumber)
            Me.Controls.Add(Me.cboVendor)
            Me.Controls.Add(Me.lblVendor)
            Me.Name = "PurchaseOrderEditorView"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Purchase Order Editor"
            CType(Me.dgvLines, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

            AddHandler btnSave.Click, Sub(sender, e) RaiseEvent SaveRequested(sender, e)
            AddHandler btnSubmit.Click, Sub(sender, e) RaiseEvent SubmitRequested(sender, e)
            AddHandler btnAddLine.Click, Sub(sender, e) RaiseEvent AddLineRequested(sender, e)
            AddHandler btnRemoveLine.Click, Sub(sender, e) RaiseEvent RemoveLineRequested(sender, e)
        End Sub

        Public Event SaveRequested As EventHandler Implements IPurchaseOrderEditorView.SaveRequested
        Public Event SubmitRequested As EventHandler Implements IPurchaseOrderEditorView.SubmitRequested
        Public Event AddLineRequested As EventHandler Implements IPurchaseOrderEditorView.AddLineRequested
        Public Event RemoveLineRequested As EventHandler Implements IPurchaseOrderEditorView.RemoveLineRequested

        Public Sub AttachPresenter(presenter As PurchaseOrderEditorPresenter) Implements IPurchaseOrderEditorView.AttachPresenter
            _presenter = presenter
        End Sub

        Public Shadows Function ShowDialog() As DialogResult Implements IPurchaseOrderEditorView.ShowDialog
            Return MyBase.ShowDialog()
        End Function

        Public Sub CloseView() Implements IPurchaseOrderEditorView.CloseView
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

        Public Shadows Sub Close() Implements IPurchaseOrderEditorView.Close
            MyBase.Close()
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedVendorId As Integer? Implements IPurchaseOrderEditorView.SelectedVendorId
            Get
                If cboVendor.SelectedValue IsNot Nothing Then
                    Return CType(cboVendor.SelectedValue, Integer)
                End If
                Return Nothing
            End Get
            Set(value As Integer?)
                cboVendor.SelectedValue = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Vendors As Object Implements IPurchaseOrderEditorView.Vendors
            Get
                Return cboVendor.DataSource
            End Get
            Set(value As Object)
                cboVendor.DataSource = value
                cboVendor.DisplayMember = "Name"
                cboVendor.ValueMember = "Id"
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property OrderNumber As String Implements IPurchaseOrderEditorView.OrderNumber
            Get
                Return txtOrderNumber.Text
            End Get
            Set(value As String)
                txtOrderNumber.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Status As Integer Implements IPurchaseOrderEditorView.Status
            Get
                Return 0 ' Default Draft
            End Get
            Set(value As Integer)
                lblStatus.Text = If(value = 0, "Draft", "Submitted")
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property ExpectedDeliveryDate As Date? Implements IPurchaseOrderEditorView.ExpectedDeliveryDate
            Get
                If dtpExpectedDeliveryDate.Checked Then
                    Return dtpExpectedDeliveryDate.Value
                End If
                Return Nothing
            End Get
            Set(value As Date?)
                If value.HasValue Then
                    dtpExpectedDeliveryDate.Checked = True
                    dtpExpectedDeliveryDate.Value = value.Value
                Else
                    dtpExpectedDeliveryDate.Checked = False
                End If
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Notes As String Implements IPurchaseOrderEditorView.Notes
            Get
                Return txtNotes.Text
            End Get
            Set(value As String)
                txtNotes.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Lines As Object Implements IPurchaseOrderEditorView.Lines
            Get
                Return dgvLines.DataSource
            End Get
            Set(value As Object)
                dgvLines.DataSource = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedLineIndex As Integer? Implements IPurchaseOrderEditorView.SelectedLineIndex
            Get
                If dgvLines.SelectedRows.Count > 0 Then
                    Return dgvLines.SelectedRows(0).Index
                End If
                Return Nothing
            End Get
            Set(value As Integer?)
                If value.HasValue AndAlso value.Value >= 0 AndAlso value.Value < dgvLines.Rows.Count Then
                    dgvLines.Rows(value.Value).Selected = True
                End If
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SelectedLine As PurchaseOrderLine Implements IPurchaseOrderEditorView.SelectedLine
            Get
                If dgvLines.SelectedRows.Count > 0 Then
                    Return TryCast(dgvLines.SelectedRows(0).DataBoundItem, PurchaseOrderLine)
                End If
                Return Nothing
            End Get
            Set(value As PurchaseOrderLine)
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property TotalAmount As Decimal Implements IPurchaseOrderEditorView.TotalAmount
            Get
                Return 0D
            End Get
            Set(value As Decimal)
                lblTotalAmount.Text = String.Format("Total: ₱{0:N2}", value)
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanEdit As Boolean Implements IPurchaseOrderEditorView.CanEdit
            Get
                Return btnSave.Enabled
            End Get
            Set(value As Boolean)
                btnSave.Enabled = value
                btnSubmit.Enabled = value
                btnAddLine.Enabled = value
                btnRemoveLine.Enabled = value
                cboVendor.Enabled = value
                txtNotes.ReadOnly = Not value
                dtpExpectedDeliveryDate.Enabled = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanAddLine As Boolean Implements IPurchaseOrderEditorView.CanAddLine
            Get
                Return btnAddLine.Enabled
            End Get
            Set(value As Boolean)
                btnAddLine.Enabled = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanRemoveLine As Boolean Implements IPurchaseOrderEditorView.CanRemoveLine
            Get
                Return btnRemoveLine.Enabled
            End Get
            Set(value As Boolean)
                btnRemoveLine.Enabled = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanSave As Boolean Implements IPurchaseOrderEditorView.CanSave
            Get
                Return btnSave.Enabled
            End Get
            Set(value As Boolean)
                btnSave.Enabled = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property CanSubmit As Boolean Implements IPurchaseOrderEditorView.CanSubmit
            Get
                Return btnSubmit.Enabled
            End Get
            Set(value As Boolean)
                btnSubmit.Enabled = value
            End Set
        End Property

        Public Sub ShowError(message As String) Implements IPurchaseOrderEditorView.ShowError
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Sub ShowMessage(message As String) Implements IPurchaseOrderEditorView.ShowMessage
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub
    End Class
End Namespace
