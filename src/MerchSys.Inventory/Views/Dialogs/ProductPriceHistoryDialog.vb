Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports MerchSys.Inventory.Presenters

Namespace Views.Dialogs

    Public Class ProductPriceHistoryDialog
        Inherits Form
        Implements IProductPriceHistoryView

        Private _histories As IReadOnlyList(Of PriceHistoryRowItem)

        Public Sub New()
            InitializeComponent()
            AddHandler dgvHistories.CellFormatting, AddressOf dgvHistories_CellFormatting
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property PriceHistories As IReadOnlyList(Of PriceHistoryRowItem) Implements IProductPriceHistoryView.PriceHistories
            Get
                Return _histories
            End Get
            Set(value As IReadOnlyList(Of PriceHistoryRowItem))
                _histories = value
                If InvokeRequired Then
                    Invoke(Sub() UpdateGrid())
                Else
                    UpdateGrid()
                End If
            End Set
        End Property

        Public Shadows Function ShowDialog(owner As IWin32Window) As DialogResult Implements IProductPriceHistoryView.ShowDialog
            Return MyBase.ShowDialog(owner)
        End Function

        Private Sub UpdateGrid()
            dgvHistories.DataSource = Nothing
            dgvHistories.DataSource = _histories
        End Sub

        Private Sub dgvHistories_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
            If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
                Dim colName = dgvHistories.Columns(e.ColumnIndex).Name
                If colName = "NewPrice" OrElse colName = "OldPrice" Then
                    Dim rowItem = TryCast(dgvHistories.Rows(e.RowIndex).DataBoundItem, PriceHistoryRowItem)
                    If rowItem IsNot Nothing AndAlso colName = "NewPrice" Then
                        If rowItem.NewPrice > rowItem.OldPrice Then
                            e.CellStyle.ForeColor = Color.Green
                        ElseIf rowItem.NewPrice < rowItem.OldPrice Then
                            e.CellStyle.ForeColor = Color.Red
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub btnClose_Click(sender As Object, e As System.EventArgs) Handles btnClose.Click
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

    End Class

End Namespace
