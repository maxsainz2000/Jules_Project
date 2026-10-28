Imports System.Windows.Forms
Imports MerchSys.POS.Presenters
Imports MerchSys.POS.Views

Namespace Views.POS
    Partial Public Class VatSettingsView
        Inherits UserControl
        Implements IVatSettingsView

        Private ReadOnly _presenter As VatSettingsPresenter

        Public Sub New()
            InitializeComponent()
        AddHandler pnlErrorState.RetryClicked, Sub(s, ev) RaiseEvent ReloadRequested(Me, EventArgs.Empty)
        End Sub

        Public Sub New(presenter As VatSettingsPresenter)
            InitializeComponent()
            _presenter = presenter
            _presenter.View = Me
        AddHandler pnlErrorState.RetryClicked, Sub(s, ev) RaiseEvent ReloadRequested(Me, EventArgs.Empty)
        End Sub

        Public Event SaveRequested As EventHandler Implements IVatSettingsView.SaveRequested
        Public Event ReloadRequested As EventHandler Implements IVatSettingsView.ReloadRequested
        Public Event VatRegistrationChanged As EventHandler Implements IVatSettingsView.VatRegistrationChanged

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsVatRegistered As Boolean Implements IVatSettingsView.IsVatRegistered
            Get
                Return chkIsVatRegistered.Checked
            End Get
            Set(value As Boolean)
                chkIsVatRegistered.Checked = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property VatRatePercent As Decimal Implements IVatSettingsView.VatRatePercent
            Get
                Return nudVatRate.Value
            End Get
            Set(value As Decimal)
                nudVatRate.Value = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property NonVatRatePercent As Decimal Implements IVatSettingsView.NonVatRatePercent
            Get
                Return nudNonVatRate.Value
            End Get
            Set(value As Decimal)
                nudNonVatRate.Value = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property BusinessName As String Implements IVatSettingsView.BusinessName
            Get
                Return txtBusinessName.Text
            End Get
            Set(value As String)
                txtBusinessName.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property BusinessAddress As String Implements IVatSettingsView.BusinessAddress
            Get
                Return txtBusinessAddress.Text
            End Get
            Set(value As String)
                txtBusinessAddress.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Tin As String Implements IVatSettingsView.Tin
            Get
                Return txtTin.Text
            End Get
            Set(value As String)
                txtTin.Text = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
                Public Property ErrorMessage As String Implements IVatSettingsView.ErrorMessage
            Get
                Return "" ' View only
            End Get
            Set(value As String)
                SafeInvoke(Sub()
                               If String.IsNullOrWhiteSpace(value) Then
                                   pnlErrorState.Visible = False
                               Else
                                   pnlErrorState.Message = value
                                   pnlErrorState.Visible = True
                                   pnlErrorState.BringToFront()
                               End If
                           End Sub)
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsSaveEnabled As Boolean Implements IVatSettingsView.IsSaveEnabled
            Get
                Return btnSave.Enabled
            End Get
            Set(value As Boolean)
                btnSave.Enabled = value
            End Set
        End Property

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            RaiseEvent SaveRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub btnReload_Click(sender As Object, e As EventArgs) Handles btnReload.Click
            RaiseEvent ReloadRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub chkIsVatRegistered_CheckedChanged(sender As Object, e As EventArgs) Handles chkIsVatRegistered.CheckedChanged
            RaiseEvent VatRegistrationChanged(Me, EventArgs.Empty)
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property IsLoading As Boolean Implements IVatSettingsView.IsLoading
            Get
                Return Not Me.Enabled
            End Get
            Set(value As Boolean)
                SafeInvoke(Sub()
                               Me.Enabled = Not value
                               If value Then pnlErrorState.Visible = False
                           End Sub)
            End Set
        End Property


        Private Sub SafeInvoke(action As Action)
            If Me.InvokeRequired Then
                Me.Invoke(action)
            Else
                action()
            End If
        End Sub

    End Class
End Namespace
