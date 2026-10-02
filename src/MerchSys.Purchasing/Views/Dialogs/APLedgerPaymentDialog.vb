Imports System
Imports System.Windows.Forms
Imports MerchSys.Purchasing.Views

Namespace Views.Dialogs
    Public Partial Class APLedgerPaymentDialog
        Inherits Form
        Implements IAPLedgerPaymentDialog

        Public Sub New()
            InitializeComponent()
            AddHandler btnOK.Click, AddressOf btnOK_Click
        End Sub

        Private Sub btnOK_Click(sender As Object, e As EventArgs)
            If Me.PaymentAmount > 0 AndAlso Me.PaymentAmount <= Me.Balance Then
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                MessageBox.Show("Invalid payment amount. It must be greater than zero and not exceed the balance.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property InvoiceDetails As String Implements IAPLedgerPaymentDialog.InvoiceDetails
            Get
                Return lblInvoiceDetails.Text
            End Get
            Set(value As String)
                lblInvoiceDetails.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Balance As Decimal Implements IAPLedgerPaymentDialog.Balance
            Get
                Return 0 ' Write-only for display
            End Get
            Set(value As Decimal)
                lblBalance.Text = $"Balance: ₱{value:N2}"
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property PaymentAmount As Decimal Implements IAPLedgerPaymentDialog.PaymentAmount
            Get
                Dim amt As Decimal
                If Decimal.TryParse(txtPaymentAmount.Text, amt) Then
                    Return amt
                End If
                Return 0
            End Get
            Set(value As Decimal)
                txtPaymentAmount.Text = value.ToString("0.00")
            End Set
        End Property
    End Class
End Namespace
