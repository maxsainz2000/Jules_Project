Imports System.Windows.Forms
Imports MerchSys.App.Views

Namespace Fakes
    Public Class FakeOwnerDashboardView
        Inherits UserControl
        Implements IOwnerDashboardView

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Kpi1Value As String Implements IOwnerDashboardView.Kpi1Value
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Kpi1Interpretation As String Implements IOwnerDashboardView.Kpi1Interpretation
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Kpi2Value As String Implements IOwnerDashboardView.Kpi2Value
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Kpi2Interpretation As String Implements IOwnerDashboardView.Kpi2Interpretation
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Kpi3Value As String Implements IOwnerDashboardView.Kpi3Value
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Kpi3Interpretation As String Implements IOwnerDashboardView.Kpi3Interpretation
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Kpi4Value As String Implements IOwnerDashboardView.Kpi4Value
        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property Kpi4Interpretation As String Implements IOwnerDashboardView.Kpi4Interpretation

        Public Shadows Event Load As EventHandler Implements IOwnerDashboardView.Load

        Public Sub SetLoading(loading As Boolean) Implements IOwnerDashboardView.SetLoading
        End Sub
    End Class
End Namespace
