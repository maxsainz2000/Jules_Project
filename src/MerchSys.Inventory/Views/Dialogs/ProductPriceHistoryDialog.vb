Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel
Imports MerchSys.Inventory.Presenters

Namespace Views.Dialogs

    Public Partial Class ProductPriceHistoryDialog
        Inherits Form
        Implements IProductPriceHistoryView

        Public Event LoadView As EventHandler Implements IProductPriceHistoryView.LoadView

        Private _history As IReadOnlyList(Of PriceHistoryRowItem) = New List(Of PriceHistoryRowItem)()

        Public Sub New()
            InitializeComponent()
            AddHandler Me.Load, Sub(sender, e) RaiseEvent LoadView(Me, EventArgs.Empty)
            AddHandler dgvHistory.CellFormatting, AddressOf dgvHistory_CellFormatting
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ProductId As Integer Implements IProductPriceHistoryView.ProductId

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property History As IReadOnlyList(Of PriceHistoryRowItem) Implements IProductPriceHistoryView.History
            Get
                Return _history
            End Get
            Set(value As IReadOnlyList(Of PriceHistoryRowItem))
                _history = value
                If InvokeRequired Then
                    Invoke(Sub() UpdateGrid())
                Else
                    UpdateGrid()
                End If
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Presenter As ProductPriceHistoryPresenter Implements IProductPriceHistoryView.Presenter

        Private Sub UpdateGrid()
            dgvHistory.DataSource = Nothing
            dgvHistory.DataSource = _history
        End Sub

        Private Sub dgvHistory_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
            If dgvHistory.Columns(e.ColumnIndex).Name = "PriceDelta" AndAlso e.Value IsNot Nothing Then
                Dim delta As Decimal
                If Decimal.TryParse(e.Value.ToString(), delta) Then
                    If delta > 0 Then
                        e.CellStyle.ForeColor = Color.Green
                    ElseIf delta < 0 Then
                        e.CellStyle.ForeColor = Color.Red
                    End If
                    e.Value = delta.ToString("+0.00;-0.00;0.00")
                    e.FormattingApplied = True
                End If
            End If
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

        Public Shadows Function ShowDialog(owner As IWin32Window) As DialogResult Implements IProductPriceHistoryView.ShowDialog
            Return MyBase.ShowDialog(owner)
        End Function
    End Class

End Namespace
