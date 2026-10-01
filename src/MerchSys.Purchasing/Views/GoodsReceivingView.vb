Imports System.Collections
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel
Imports MerchSys.Purchasing.Presenters

Namespace Views
    Public Class GoodsReceivingView
        Implements IGoodsReceivingView

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnLoadPendingPOs As Func(Of Task) Implements IGoodsReceivingView.OnLoadPendingPOs
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnPOSelected As Func(Of Integer, Task) Implements IGoodsReceivingView.OnPOSelected
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnConfirmReceipt As Func(Of Task) Implements IGoodsReceivingView.OnConfirmReceipt

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property IsPOSelected As Boolean Implements IGoodsReceivingView.IsPOSelected
            Get
                Return DgvLines.Visible
            End Get
            Set(value As Boolean)
                DgvLines.Visible = value
                PanelPlaceholder.Visible = Not value
                BtnConfirmReceipt.Enabled = value
            End Set
        End Property

        Public Sub New()
            InitializeComponent()
            DgvLines.AutoGenerateColumns = False
        End Sub

        Protected Overrides Async Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            If Not DesignMode AndAlso OnLoadPendingPOs IsNot Nothing Then
                Await OnLoadPendingPOs.Invoke()
            End If
        End Sub

        Private Async Sub CboPurchaseOrders_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboPurchaseOrders.SelectedIndexChanged
            If CboPurchaseOrders.SelectedItem IsNot Nothing AndAlso OnPOSelected IsNot Nothing Then
                Dim selectedItem = DirectCast(CboPurchaseOrders.SelectedItem, POSelectorItem)
                Await OnPOSelected.Invoke(selectedItem.Id)
            End If
        End Sub

        Private Async Sub BtnConfirmReceipt_Click(sender As Object, e As EventArgs) Handles BtnConfirmReceipt.Click
            ' Force ending edit mode to commit any cell changes to the underlying data source
            DgvLines.EndEdit()
            If OnConfirmReceipt IsNot Nothing Then
                Await OnConfirmReceipt.Invoke()
            End If
        End Sub

        Private Sub DgvLines_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DgvLines.CellFormatting
            If e.RowIndex < 0 OrElse e.RowIndex >= DgvLines.Rows.Count Then Return

            Dim item As GRLineItem = TryCast(DgvLines.Rows(e.RowIndex).DataBoundItem, GRLineItem)
            If item Is Nothing Then Return

            If item.HasDiscrepancy Then
                DgvLines.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.LightGoldenrodYellow

                If DgvLines.Columns(e.ColumnIndex).Name = "ColWarning" Then
                    e.Value = "⚠"
                    e.CellStyle.ForeColor = Color.Orange
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    e.CellStyle.Font = New Font(DgvLines.Font.FontFamily, 12, FontStyle.Bold)
                End If

                If DgvLines.Columns(e.ColumnIndex).Name = "ColDiscrepancyNotes" AndAlso String.IsNullOrWhiteSpace(item.DiscrepancyNotes) Then
                     e.CellStyle.BackColor = Color.MistyRose ' Highlight empty notes when discrepancy exists
                End If
            Else
                If DgvLines.Columns(e.ColumnIndex).Name = "ColWarning" Then
                    e.Value = ""
                End If
            End If
        End Sub

        Public Sub BindPOSelector(items As IEnumerable) Implements IGoodsReceivingView.BindPOSelector
            CboPurchaseOrders.Items.Clear()
            If items IsNot Nothing Then
                For Each item In items
                    CboPurchaseOrders.Items.Add(item)
                Next
            End If
        End Sub

        Public Sub BindReceivingLines(items As IEnumerable) Implements IGoodsReceivingView.BindReceivingLines
            DgvLines.DataSource = items
            RefreshLines()
        End Sub

        Public Sub ShowError(message As String) Implements IGoodsReceivingView.ShowError
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Sub ShowMessage(message As String) Implements IGoodsReceivingView.ShowMessage
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        Public Function ConfirmAction(message As String, title As String) As Boolean Implements IGoodsReceivingView.ConfirmAction
            Return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
        End Function

        Public Sub SetStatus(status As String) Implements IGoodsReceivingView.SetStatus
            LblStatus.Text = status
        End Sub

        Public Sub RefreshLines() Implements IGoodsReceivingView.RefreshLines
            DgvLines.Refresh()
        End Sub
    End Class
End Namespace
