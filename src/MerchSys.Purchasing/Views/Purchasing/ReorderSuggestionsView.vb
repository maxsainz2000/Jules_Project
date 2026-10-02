Imports MerchSys.Purchasing.Entities
Imports System.Windows.Forms
Imports System.ComponentModel

Namespace Views.Purchasing
    Public Class ReorderSuggestionsView
        Inherits UserControl
        Implements IReorderSuggestionsView

        Public Event LoadSuggestions As EventHandler Implements IReorderSuggestionsView.LoadSuggestions
        Public Event GenerateSuggestionsClicked As EventHandler Implements IReorderSuggestionsView.GenerateSuggestionsClicked
        Public Event AcceptSuggestionClicked As Action(Of Integer, Integer) Implements IReorderSuggestionsView.AcceptSuggestionClicked
        Public Event DismissSuggestionClicked As Action(Of Integer, String) Implements IReorderSuggestionsView.DismissSuggestionClicked

        Public Event LoadConfigurations As EventHandler Implements IReorderSuggestionsView.LoadConfigurations
        Public Event EditConfigurationClicked As Action(Of ReorderConfig) Implements IReorderSuggestionsView.EditConfigurationClicked

        Private _allSuggestions As List(Of ReorderSuggestion)
        Private _allConfigs As List(Of ReorderConfig)

        Public Sub New()
            InitializeComponent()
            SetupGrids()
            cmbStatusFilter.SelectedIndex = 0 ' Default to "Pending"
        End Sub

        Private Sub SetupGrids()
            dgvSuggestions.AutoGenerateColumns = False
            dgvSuggestions.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Id", .Name = "Id", .Visible = False})
            dgvSuggestions.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "ProductId", .HeaderText = "Product ID", .ReadOnly = True})
            dgvSuggestions.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "SuggestedQuantity", .HeaderText = "Suggested Qty", .Name = "SuggestedQuantity", .ReadOnly = False})
            dgvSuggestions.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Status", .HeaderText = "Status", .ReadOnly = True})
            dgvSuggestions.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "Reason", .HeaderText = "Reason", .ReadOnly = True})
            dgvSuggestions.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "SeasonalIndicator", .HeaderText = "Seasonal", .ReadOnly = True})

            Dim acceptCol As New DataGridViewButtonColumn With {.HeaderText = "Accept", .Text = "Accept", .UseColumnTextForButtonValue = True, .Name = "AcceptCol"}
            dgvSuggestions.Columns.Add(acceptCol)

            Dim dismissCol As New DataGridViewButtonColumn With {.HeaderText = "Dismiss", .Text = "Dismiss", .UseColumnTextForButtonValue = True, .Name = "DismissCol"}
            dgvSuggestions.Columns.Add(dismissCol)

            dgvConfigs.AutoGenerateColumns = False
            dgvConfigs.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "ProductId", .HeaderText = "Product ID", .ReadOnly = True})
            dgvConfigs.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "ReorderThreshold", .HeaderText = "Threshold", .ReadOnly = True})
            dgvConfigs.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "SafetyStock", .HeaderText = "Safety Stock", .ReadOnly = True})
            dgvConfigs.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "LeadTimeDays", .HeaderText = "Lead Time", .ReadOnly = True})
            dgvConfigs.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = "SeasonalMultiplier", .HeaderText = "Seasonal Multiplier", .ReadOnly = True})

            Dim editCol As New DataGridViewButtonColumn With {.HeaderText = "Edit", .Text = "Edit", .UseColumnTextForButtonValue = True, .Name = "EditCol"}
            dgvConfigs.Columns.Add(editCol)
        End Sub

        Private Sub ReorderSuggestionsView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            If Not DesignMode Then
                RaiseEvent LoadSuggestions(Me, EventArgs.Empty)
                RaiseEvent LoadConfigurations(Me, EventArgs.Empty)
            End If
        End Sub

        Public Sub BindSuggestions(suggestions As List(Of ReorderSuggestion)) Implements IReorderSuggestionsView.BindSuggestions
            _allSuggestions = suggestions
            FilterSuggestions()
        End Sub

        Public Sub BindConfigurations(configs As List(Of ReorderConfig)) Implements IReorderSuggestionsView.BindConfigurations
            _allConfigs = configs
            dgvConfigs.DataSource = configs

            ' Update seasonal indicator in suggestions if possible
            If _allSuggestions IsNot Nothing Then
                FilterSuggestions()
            End If
        End Sub

        Private Sub FilterSuggestions()
            If _allSuggestions Is Nothing Then Return

            Dim selectedStatus = cmbStatusFilter.SelectedItem?.ToString()
            Dim filtered = _allSuggestions

            If selectedStatus <> "All" Then
                filtered = _allSuggestions.Where(Function(s) s.Status = selectedStatus).ToList()
            End If

            ' Create anonymous type to include seasonal indicator for grid display
            Dim displayList = filtered.Select(Function(s)
                                                  Dim isSeasonal = False
                                                  If _allConfigs IsNot Nothing Then
                                                      Dim config = _allConfigs.FirstOrDefault(Function(c) c.ProductId = s.ProductId)
                                                      If config IsNot Nothing AndAlso config.SeasonalMultiplier > 1D Then
                                                          isSeasonal = True
                                                      End If
                                                  End If
                                                  Return New With {
                                                      .Id = s.Id,
                                                      .ProductId = s.ProductId,
                                                      .SuggestedQuantity = s.SuggestedQuantity,
                                                      .Status = s.Status,
                                                      .Reason = s.Reason,
                                                      .SeasonalIndicator = If(isSeasonal, "*", "")
                                                  }
                                              End Function).ToList()

            dgvSuggestions.DataSource = displayList

            ' Hide accept/dismiss buttons if not pending
            Dim acceptCol = dgvSuggestions.Columns("AcceptCol")
            Dim dismissCol = dgvSuggestions.Columns("DismissCol")
            If acceptCol IsNot Nothing AndAlso dismissCol IsNot Nothing Then
                Dim isPending = (selectedStatus = "Pending")
                acceptCol.Visible = isPending
                dismissCol.Visible = isPending
                dgvSuggestions.Columns("SuggestedQuantity").ReadOnly = Not isPending
            End If
        End Sub

        Private Sub cmbStatusFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbStatusFilter.SelectedIndexChanged
            FilterSuggestions()
        End Sub

        Private Sub btnGenerate_Click(sender As Object, e As EventArgs) Handles btnGenerate.Click
            RaiseEvent GenerateSuggestionsClicked(Me, EventArgs.Empty)
        End Sub

        Private Sub dgvSuggestions_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSuggestions.CellContentClick
            If e.RowIndex >= 0 Then
                If dgvSuggestions.Columns(e.ColumnIndex).Name = "AcceptCol" Then
                    Dim suggestionId = CInt(dgvSuggestions.Rows(e.RowIndex).Cells("Id").Value)
                    Dim qty = CInt(dgvSuggestions.Rows(e.RowIndex).Cells("SuggestedQuantity").Value)
                    RaiseEvent AcceptSuggestionClicked(suggestionId, qty)
                ElseIf dgvSuggestions.Columns(e.ColumnIndex).Name = "DismissCol" Then
                    Dim suggestionId = CInt(dgvSuggestions.Rows(e.RowIndex).Cells("Id").Value)
                    Dim reason = InputBox("Enter reason for dismissal:", "Dismiss Suggestion")
                    If Not String.IsNullOrWhiteSpace(reason) Then
                        RaiseEvent DismissSuggestionClicked(suggestionId, reason)
                    End If
                End If
            End If
        End Sub

        Private Sub dgvConfigs_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvConfigs.CellContentClick
            If e.RowIndex >= 0 AndAlso dgvConfigs.Columns(e.ColumnIndex).Name = "EditCol" Then
                Dim config = CType(dgvConfigs.Rows(e.RowIndex).DataBoundItem, ReorderConfig)
                RaiseEvent EditConfigurationClicked(config)
            End If
        End Sub

        Public Sub ShowMessage(message As String) Implements IReorderSuggestionsView.ShowMessage
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        Public Sub ShowError(message As String) Implements IReorderSuggestionsView.ShowError
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub
    End Class
End Namespace
