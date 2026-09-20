Imports System.Windows.Forms
Imports System.ComponentModel

Namespace Views
    Public Partial Class OwnerDashboardView
        Inherits UserControl
        Implements IOwnerDashboardView

        Public Custom Event DashboardLoad As EventHandler Implements IOwnerDashboardView.Load
            AddHandler(value As EventHandler)
                AddHandler MyBase.Load, value
            End AddHandler
            RemoveHandler(value As EventHandler)
                RemoveHandler MyBase.Load, value
            End RemoveHandler
            RaiseEvent(sender As Object, e As EventArgs)
                ' The base UserControl Load event is raised automatically.
            End RaiseEvent
        End Event

        Public Sub New()
            InitializeComponent()
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Kpi1Value As String Implements IOwnerDashboardView.Kpi1Value
            Get
                Return lblKpi1Value.Text
            End Get
            Set(value As String)
                lblKpi1Value.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Kpi1Interpretation As String Implements IOwnerDashboardView.Kpi1Interpretation
            Get
                Return lblKpi1Interpretation.Text
            End Get
            Set(value As String)
                lblKpi1Interpretation.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Kpi2Value As String Implements IOwnerDashboardView.Kpi2Value
            Get
                Return lblKpi2Value.Text
            End Get
            Set(value As String)
                lblKpi2Value.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Kpi2Interpretation As String Implements IOwnerDashboardView.Kpi2Interpretation
            Get
                Return lblKpi2Interpretation.Text
            End Get
            Set(value As String)
                lblKpi2Interpretation.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Kpi3Value As String Implements IOwnerDashboardView.Kpi3Value
            Get
                Return lblKpi3Value.Text
            End Get
            Set(value As String)
                lblKpi3Value.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Kpi3Interpretation As String Implements IOwnerDashboardView.Kpi3Interpretation
            Get
                Return lblKpi3Interpretation.Text
            End Get
            Set(value As String)
                lblKpi3Interpretation.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Kpi4Value As String Implements IOwnerDashboardView.Kpi4Value
            Get
                Return lblKpi4Value.Text
            End Get
            Set(value As String)
                lblKpi4Value.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property Kpi4Interpretation As String Implements IOwnerDashboardView.Kpi4Interpretation
            Get
                Return lblKpi4Interpretation.Text
            End Get
            Set(value As String)
                lblKpi4Interpretation.Text = value
            End Set
        End Property

        Public Sub SetLoading(loading As Boolean) Implements IOwnerDashboardView.SetLoading
            pnlLoading.Visible = loading
            pnlLoading.BringToFront()
        End Sub
    End Class
End Namespace
