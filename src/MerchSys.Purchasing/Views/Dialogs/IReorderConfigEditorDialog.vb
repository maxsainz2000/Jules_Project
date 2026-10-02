Imports MerchSys.Purchasing.Entities
Imports System.Windows.Forms

Namespace Views.Dialogs
    Public Interface IReorderConfigEditorDialog
        Sub SetConfig(config As ReorderConfig)
        Function GetConfig() As ReorderConfig
        Function ShowDialog() As DialogResult
    End Interface
End Namespace
