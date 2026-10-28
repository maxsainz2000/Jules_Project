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
            AddHandler pnlErrorState.RetryClicked, Sub(sender, e) RaiseEvent ReloadRequested(Me, EventArgs.Empty)
        End Sub

        Public Sub New(presenter As VatSettingsPresenter)
            InitializeComponent()
            If presenter IsNot Nothing Then
                _presenter = presenter
                _presenter.View = Me
            End If
            AddHandler pnlErrorState.RetryClicked, Sub(sender, e) RaiseEvent ReloadRequested(Me, EventArgs.Empty)
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
                Return pnlErrorState.Message
            End Get
            Set(value As String)
                If Not String.IsNullOrEmpty(value) Then
                    pnlErrorState.Message = value
                    pnlErrorState.Visible = True
                    pnlErrorState.BringToFront()
                    lblError.Visible = False
                Else
                    pnlErrorState.Visible = False
                End If
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
    End Class
End Namespace
