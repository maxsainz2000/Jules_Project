Imports System.Windows.Forms
Imports System.Collections
Imports System.Threading.Tasks
Imports System.ComponentModel
Imports MerchSys.Purchasing.Presenters

Namespace Views
    Public Class VendorDirectoryView
        Implements IVendorDirectoryView

        Private ReadOnly _presenter As VendorListPresenter

        ' Required for Designer support
        Public Sub New()
            InitializeComponent()
        End Sub

        Public Sub New(presenter As VendorListPresenter)
            InitializeComponent()
            _presenter = presenter
            _presenter.View = Me
            WireEvents()
        End Sub

        Private Sub WireEvents()
            AddHandler btnAdd.Click, Async Sub(sender, e)
                                         If OnAdd IsNot Nothing Then Await OnAdd.Invoke()
                                     End Sub
            AddHandler btnEdit.Click, Async Sub(sender, e)
                                          If OnEdit IsNot Nothing Then Await OnEdit.Invoke()
                                      End Sub
            AddHandler btnSave.Click, Async Sub(sender, e)
                                          If OnSave IsNot Nothing Then Await OnSave.Invoke()
                                      End Sub
            AddHandler btnDelete.Click, Async Sub(sender, e)
                                            If OnDelete IsNot Nothing Then Await OnDelete.Invoke()
                                        End Sub
            AddHandler btnRefresh.Click, Async Sub(sender, e)
                                             If OnSearch IsNot Nothing Then Await OnSearch.Invoke(String.Empty)
                                         End Sub
            AddHandler txtSearch.KeyUp, Async Sub(sender, e)
                                            If e.KeyCode = Keys.Enter Then
                                                If OnSearch IsNot Nothing Then Await OnSearch.Invoke(txtSearch.Text)
                                            ElseIf e.KeyCode = Keys.Escape Then
                                                txtSearch.Text = String.Empty
                                                If OnSearch IsNot Nothing Then Await OnSearch.Invoke(String.Empty)
                                            End If
                                        End Sub
            AddHandler btnCancel.Click, Sub(sender, e) IsEditorOpen = False
            AddHandler VendorGrid.SelectionChanged, Async Sub(sender, e)
                                                        If OnSelectionChanged IsNot Nothing Then Await OnSelectionChanged.Invoke(SelectedVendorId)
                                                    End Sub
            AddHandler VendorGrid.DoubleClick, Async Sub(sender, e)
                                                   If OnEdit IsNot Nothing Then Await OnEdit.Invoke()
                                               End Sub
        End Sub

        Protected Overrides Async Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            If Not DesignMode AndAlso OnSearch IsNot Nothing Then
                Await OnSearch.Invoke(String.Empty)
            End If
        End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property IsEditorOpen As Boolean Implements IVendorDirectoryView.IsEditorOpen
            Get
                Return PanelEditor.Visible
            End Get
            Set(value As Boolean)
                PanelEditor.Visible = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedVendorId As Integer? Implements IVendorDirectoryView.SelectedVendorId
            Get
                If VendorGrid.SelectedRows.Count > 0 Then
                    Dim row = VendorGrid.SelectedRows(0)
                    If row.Cells("Id") IsNot Nothing AndAlso row.Cells("Id").Value IsNot Nothing Then
                        Return Convert.ToInt32(row.Cells("Id").Value)
                    End If
                End If
                Return Nothing
            End Get
            Set(value As Integer?)
                If Not value.HasValue Then
                    VendorGrid.ClearSelection()
                    Return
                End If

                For Each row As DataGridViewRow In VendorGrid.Rows
                    If row.Cells("Id") IsNot Nothing AndAlso row.Cells("Id").Value IsNot Nothing AndAlso Convert.ToInt32(row.Cells("Id").Value) = value.Value Then
                        row.Selected = True
                        Return
                    End If
                Next
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorName As String Implements IVendorDirectoryView.EditorName
            Get
                Return txtName.Text
            End Get
            Set(value As String)
                txtName.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorPhone As String Implements IVendorDirectoryView.EditorPhone
            Get
                Return txtPhone.Text
            End Get
            Set(value As String)
                txtPhone.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property EditorLeadTimeDays As Integer Implements IVendorDirectoryView.EditorLeadTimeDays
            Get
                Return Convert.ToInt32(numLeadTime.Value)
            End Get
            Set(value As Integer)
                numLeadTime.Value = value
            End Set
        End Property

        Public Sub BindVendors(items As IEnumerable) Implements IVendorDirectoryView.BindVendors
            VendorGrid.DataSource = items
        End Sub

        Public Sub BindRecentPOs(items As IEnumerable) Implements IVendorDirectoryView.BindRecentPOs
            POGrid.DataSource = items
            If items IsNot Nothing Then
                Dim count As Integer = 0
                For Each item In items
                    count += 1
                Next
                LabelStats.Text = $"Purchase History: {count} Recent Orders"
            Else
                LabelStats.Text = "Purchase History"
            End If
        End Sub

        Public Sub ShowError(message As String) Implements IVendorDirectoryView.ShowError
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Function ConfirmAction(message As String, title As String) As Boolean Implements IVendorDirectoryView.ConfirmAction
            Return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
        End Function

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnAdd As Func(Of Task) Implements IVendorDirectoryView.OnAdd

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnEdit As Func(Of Task) Implements IVendorDirectoryView.OnEdit

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnSave As Func(Of Task) Implements IVendorDirectoryView.OnSave

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnDelete As Func(Of Task) Implements IVendorDirectoryView.OnDelete

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnSearch As Func(Of String, Task) Implements IVendorDirectoryView.OnSearch

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property OnSelectionChanged As Func(Of Integer?, Task) Implements IVendorDirectoryView.OnSelectionChanged

    End Class
End Namespace
