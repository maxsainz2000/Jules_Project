Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports MerchSys.Inventory.Presenters
Imports MerchSys.Inventory.Views.Dialogs

Namespace Views

    Public Class ShrinkageView
        Inherits UserControl
        Implements IShrinkageView

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Presenter As Object Implements IShrinkageView.Presenter

        Public Event LoadView As EventHandler Implements IShrinkageView.LoadView
        Public Event FilterChanged As EventHandler Implements IShrinkageView.FilterChanged
        Public Event RecordShrinkageRequested As EventHandler(Of ShrinkageRecordRequest) Implements IShrinkageView.RecordShrinkageRequested

        Public Sub New()
            InitializeComponent()
            Dock = DockStyle.Fill
            cmbReason.Items.AddRange(New String() {"All", "Damage", "Theft", "Expired", "Loss"})
            cmbReason.SelectedIndex = 0
        End Sub

        Private Sub ShrinkageView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            RaiseEvent LoadView(Me, EventArgs.Empty)
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SummaryTotalItemsLost As Integer Implements IShrinkageView.SummaryTotalItemsLost
            Get
                Return 0
            End Get
            Set(value As Integer)
                lblTotalItems.Text = $"Total Items Lost: {value}"
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property SummaryTotalValueLost As Decimal Implements IShrinkageView.SummaryTotalValueLost
            Get
                Return 0D
            End Get
            Set(value As Decimal)
                lblTotalValue.Text = $"Total Value Lost: {value:C2}"
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property FilterStartDate As DateTime? Implements IShrinkageView.FilterStartDate
            Get
                If chkDateFilter.Checked Then Return dtpStart.Value
                Return Nothing
            End Get
            Set(value As DateTime?)
                If value.HasValue Then
                    dtpStart.Value = value.Value
                End If
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property FilterEndDate As DateTime? Implements IShrinkageView.FilterEndDate
            Get
                If chkDateFilter.Checked Then Return dtpEnd.Value
                Return Nothing
            End Get
            Set(value As DateTime?)
                If value.HasValue Then
                    dtpEnd.Value = value.Value
                End If
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property FilterReason As String Implements IShrinkageView.FilterReason
            Get
                If cmbReason.SelectedItem IsNot Nothing Then
                    Return cmbReason.SelectedItem.ToString()
                End If
                Return "All"
            End Get
            Set(value As String)
                cmbReason.SelectedItem = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property ShrinkageHistory As List(Of ShrinkageRowItem) Implements IShrinkageView.ShrinkageHistory
            Get
                Return CType(dgvHistory.DataSource, List(Of ShrinkageRowItem))
            End Get
            Set(value As List(Of ShrinkageRowItem))
                dgvHistory.DataSource = Nothing
                dgvHistory.DataSource = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property AvailableProducts As List(Of ShrinkageProductItem) Implements IShrinkageView.AvailableProducts

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property AvailableBatches As List(Of ShrinkageBatchItem) Implements IShrinkageView.AvailableBatches

        Private Sub btnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
            RaiseEvent FilterChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub btnRecord_Click(sender As Object, e As EventArgs) Handles btnRecord.Click
            Using dlg = New RecordShrinkageDialog()
                dlg.Initialize(AvailableProducts, AvailableBatches)
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    Dim req = New ShrinkageRecordRequest With {
                        .ProductId = dlg.SelectedProductId,
                        .BatchId = dlg.SelectedBatchId,
                        .Quantity = dlg.Quantity,
                        .Reason = dlg.Reason
                    }
                    RaiseEvent RecordShrinkageRequested(Me, req)
                End If
            End Using
        End Sub

    End Class

End Namespace
