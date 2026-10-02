Imports MerchSys.Purchasing.Entities
Imports System.Windows.Forms

Namespace Views.Dialogs
    Public Class ReorderConfigEditorDialog
        Inherits Form
        Implements IReorderConfigEditorDialog

        Private _config As ReorderConfig

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Sub SetConfig(config As ReorderConfig) Implements IReorderConfigEditorDialog.SetConfig
            _config = config
            numReorderThreshold.Value = config.ReorderThreshold
            numSafetyStock.Value = config.SafetyStock
            numLeadTimeDays.Value = config.LeadTimeDays
            numSeasonalMultiplier.Value = config.SeasonalMultiplier
        End Sub

        Public Function GetConfig() As ReorderConfig Implements IReorderConfigEditorDialog.GetConfig
            _config.ReorderThreshold = CInt(numReorderThreshold.Value)
            _config.SafetyStock = CInt(numSafetyStock.Value)
            _config.LeadTimeDays = CInt(numLeadTimeDays.Value)
            _config.SeasonalMultiplier = numSeasonalMultiplier.Value
            Return _config
        End Function

        Public Shadows Function ShowDialog() As DialogResult Implements IReorderConfigEditorDialog.ShowDialog
            Return MyBase.ShowDialog()
        End Function

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            DialogResult = DialogResult.OK
            Close()
        End Sub

        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            DialogResult = DialogResult.Cancel
            Close()
        End Sub
    End Class
End Namespace
