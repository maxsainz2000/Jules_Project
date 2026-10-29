Imports System
Imports System.Windows.Forms
Imports System.Reflection
Imports Xunit
Imports MerchSys.App.Views
Imports MerchSys.App.Views.Dialogs

Public Class KeyboardAndFocusTests

    <Fact>
    Public Sub ConcurrencyConflictPrompt_HasAcceptAndCancelButtonsAndMnemonics()
        Using dialog As New ConcurrencyConflictPrompt("Test")
            Assert.NotNull(dialog.AcceptButton)
            Assert.NotNull(dialog.CancelButton)

            Dim btnRefresh As Button = TryCast(dialog.Controls.Find("btnRefresh", True)(0), Button)
            Dim btnCancel As Button = TryCast(dialog.Controls.Find("btnCancel", True)(0), Button)

            Assert.Contains("&", btnRefresh.Text)
            Assert.Contains("&", btnCancel.Text)
        End Using
    End Sub

    <Fact>
    Public Sub SessionTimeoutWarningView_HasAcceptAndCancelButtonsAndMnemonics()
        Using dialog As New SessionTimeoutWarningView()
            Assert.NotNull(dialog.AcceptButton)
            Assert.NotNull(dialog.CancelButton)

            Dim btnStaySignedIn As Button = TryCast(dialog.Controls.Find("btnStaySignedIn", True)(0), Button)
            Dim btnSignOut As Button = TryCast(dialog.Controls.Find("btnSignOut", True)(0), Button)

            Assert.Contains("&", btnStaySignedIn.Text)
            Assert.Contains("&", btnSignOut.Text)

            Assert.True(btnStaySignedIn.TabIndex < btnSignOut.TabIndex)
        End Using
    End Sub

    <Fact>
    Public Sub LoginView_HasExpectedTabIndicesAndMnemonics()
        Using view As New LoginView()
            Dim btnLogin As Button = TryCast(view.Controls.Find("btnLogin", True)(0), Button)
            Dim btnChangePassword As Button = TryCast(view.Controls.Find("btnChangePassword", True)(0), Button)
            Dim txtUsername As TextBox = TryCast(view.Controls.Find("txtUsername", True)(0), TextBox)
            Dim txtPassword As TextBox = TryCast(view.Controls.Find("txtPassword", True)(0), TextBox)

            Assert.Contains("&", btnLogin.Text)
            Assert.Contains("&", btnChangePassword.Text)

            Assert.True(txtUsername.TabIndex < txtPassword.TabIndex)
            Assert.True(txtPassword.TabIndex < btnLogin.TabIndex)
        End Using
    End Sub

    <Fact>
    Public Sub Dialogs_HaveConfiguredAcceptAndCancelButtons()
        ' CategoryEditDialog
        Dim categoryDialog As New MerchSys.Inventory.Views.Dialogs.CategoryEditDialog(Nothing)
        Assert.NotNull(categoryDialog.AcceptButton)
        Assert.NotNull(categoryDialog.CancelButton)
        categoryDialog.Dispose()

        ' ProductEditDialog
        Dim productDialog As New MerchSys.Inventory.Views.Dialogs.ProductEditDialog(Nothing, Nothing)
        Assert.NotNull(productDialog.AcceptButton)
        Assert.NotNull(productDialog.CancelButton)
        productDialog.Dispose()

        ' RecordShrinkageDialog
        Dim shrinkageDialog As New MerchSys.Inventory.Views.Dialogs.RecordShrinkageDialog()
        Assert.NotNull(shrinkageDialog.AcceptButton)
        Assert.NotNull(shrinkageDialog.CancelButton)
        shrinkageDialog.Dispose()

        ' APLedgerPaymentDialog
        Dim apDialog As New MerchSys.Purchasing.Views.Dialogs.APLedgerPaymentDialog()
        Assert.NotNull(apDialog.AcceptButton)
        Assert.NotNull(apDialog.CancelButton)
        apDialog.Dispose()

        ' PurchaseOrderEditorDialog
        Dim poDialog As New MerchSys.Purchasing.Views.Dialogs.PurchaseOrderEditorDialog()
        Assert.NotNull(poDialog.AcceptButton)
        Assert.NotNull(poDialog.CancelButton)
        poDialog.Dispose()
    End Sub

End Class
