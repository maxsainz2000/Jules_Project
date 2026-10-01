Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Drawing

Namespace UI.Controls
    Public Class DataGridViewDateTimePickerColumn
        Inherits DataGridViewColumn

        Public Sub New()
            MyBase.New(New DataGridViewDateTimePickerCell())
        End Sub

        Public Overrides Property CellTemplate() As DataGridViewCell
            Get
                Return MyBase.CellTemplate
            End Get
            Set(ByVal value As DataGridViewCell)
                If value IsNot Nothing AndAlso Not value.GetType().IsAssignableFrom(GetType(DataGridViewDateTimePickerCell)) Then
                    Throw New InvalidCastException("Must be a DataGridViewDateTimePickerCell")
                End If
                MyBase.CellTemplate = value
            End Set
        End Property
    End Class

    Public Class DataGridViewDateTimePickerCell
        Inherits DataGridViewTextBoxCell

        Public Sub New()
            MyBase.New()
            Me.Style.Format = "d"
        End Sub

        Public Overrides Sub InitializeEditingControl(ByVal rowIndex As Integer, ByVal initialFormattedValue As Object, ByVal dataGridViewCellStyle As DataGridViewCellStyle)
            MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)
            Dim ctl As DataGridViewDateTimePickerEditingControl = CType(DataGridView.EditingControl, DataGridViewDateTimePickerEditingControl)
            Try
                If Me.Value IsNot Nothing AndAlso Me.Value IsNot DBNull.Value Then
                    ctl.Value = CType(Me.Value, DateTime)
                Else
                    ctl.Value = DateTime.Now
                End If
            Catch ex As Exception
                ctl.Value = DateTime.Now
            End Try
        End Sub

        Public Overrides ReadOnly Property EditType() As Type
            Get
                Return GetType(DataGridViewDateTimePickerEditingControl)
            End Get
        End Property

        Public Overrides ReadOnly Property ValueType() As Type
            Get
                Return GetType(DateTime)
            End Get
        End Property

        Public Overrides ReadOnly Property DefaultNewRowValue() As Object
            Get
                Return DBNull.Value
            End Get
        End Property
    End Class

    Public Class DataGridViewDateTimePickerEditingControl
        Inherits DateTimePicker
        Implements IDataGridViewEditingControl

        Private dataGridView As DataGridView
        Private valueChanged As Boolean = False
        Private rowIndex As Integer

        Public Sub New()
            Me.Format = DateTimePickerFormat.Short
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property EditingControlFormattedValue() As Object Implements IDataGridViewEditingControl.EditingControlFormattedValue
            Get
                Return Me.Value.ToShortDateString()
            End Get
            Set(ByVal value As Object)
                If TypeOf value Is String Then
                    Try
                        Me.Value = DateTime.Parse(CStr(value))
                    Catch ex As Exception
                        Me.Value = DateTime.Now
                    End Try
                End If
            End Set
        End Property

        Public Function GetEditingControlFormattedValue(ByVal context As DataGridViewDataErrorContexts) As Object Implements IDataGridViewEditingControl.GetEditingControlFormattedValue
            Return EditingControlFormattedValue
        End Function

        Public Sub ApplyCellStyleToEditingControl(ByVal dataGridViewCellStyle As DataGridViewCellStyle) Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl
            Me.Font = dataGridViewCellStyle.Font
            Me.CalendarForeColor = dataGridViewCellStyle.ForeColor
            Me.CalendarMonthBackground = dataGridViewCellStyle.BackColor
        End Sub

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property EditingControlRowIndex() As Integer Implements IDataGridViewEditingControl.EditingControlRowIndex
            Get
                Return rowIndex
            End Get
            Set(ByVal value As Integer)
                rowIndex = value
            End Set
        End Property

        Public Function EditingControlWantsInputKey(ByVal keyData As Keys, ByVal dataGridViewWantsInputKey As Boolean) As Boolean Implements IDataGridViewEditingControl.EditingControlWantsInputKey
            Select Case keyData And Keys.KeyCode
                Case Keys.Left, Keys.Up, Keys.Down, Keys.Right, Keys.Home, Keys.End, Keys.PageDown, Keys.PageUp
                    Return True
                Case Else
                    Return Not dataGridViewWantsInputKey
            End Select
        End Function

        Public Sub PrepareEditingControlForEdit(ByVal selectAll As Boolean) Implements IDataGridViewEditingControl.PrepareEditingControlForEdit
            ' No preparation needed
        End Sub

        Public ReadOnly Property RepositionEditingControlOnValueChange() As Boolean Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange
            Get
                Return False
            End Get
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Property EditingControlDataGridView() As DataGridView Implements IDataGridViewEditingControl.EditingControlDataGridView
            Get
                Return dataGridView
            End Get
            Set(ByVal value As DataGridView)
                dataGridView = value
            End Set
        End Property

        <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
        Public Shadows Property EditingControlValueChanged() As Boolean Implements IDataGridViewEditingControl.EditingControlValueChanged
            Get
                Return valueChanged
            End Get
            Set(ByVal value As Boolean)
                valueChanged = value
            End Set
        End Property

        Public ReadOnly Property EditingPanelCursor() As Cursor Implements IDataGridViewEditingControl.EditingPanelCursor
            Get
                Return MyBase.Cursor
            End Get
        End Property

        Protected Overrides Sub OnValueChanged(ByVal eventargs As EventArgs)
            valueChanged = True
            Me.EditingControlDataGridView.NotifyCurrentCellDirty(True)
            MyBase.OnValueChanged(eventargs)
        End Sub
    End Class
End Namespace
