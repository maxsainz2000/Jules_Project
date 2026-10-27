Imports Xunit
Imports System.Windows.Forms
Imports MerchSys.App.Views
Imports MerchSys.App.Views.Accounting
Imports MerchSys.App.Views.POS

Public Class LayoutResilienceTests

    <Fact>
    Public Sub OwnerDashboardView_ShouldHaveAutoScrollEnabled()
        Dim view As New OwnerDashboardView()
        Assert.True(view.AutoScroll, "OwnerDashboardView should have AutoScroll = True")
    End Sub

    <Fact>
    Public Sub FinancialOverviewView_ShouldHaveAutoScrollEnabled()
        Dim view As New FinancialOverviewView()
        Assert.True(view.AutoScroll, "FinancialOverviewView should have AutoScroll = True")
    End Sub

    <Fact>
    Public Sub VatReturnView_ShouldHaveAutoScrollEnabled()
        Dim view As New VatReturnView()
        Assert.True(view.AutoScroll, "VatReturnView should have AutoScroll = True")
    End Sub

    <Fact>
    Public Sub VatReliefReportView_ShouldHaveAutoScrollEnabled()
        Dim view As New VatReliefReportView()
        Assert.True(view.AutoScroll, "VatReliefReportView should have AutoScroll = True")
    End Sub

    <Fact>
    Public Sub VatSettingsView_ShouldHaveAutoScrollEnabled()
        Dim view As New VatSettingsView()
        Assert.True(view.AutoScroll, "VatSettingsView should have AutoScroll = True")
    End Sub


    <Fact>
    Public Sub StockDashboardView_ShouldHaveFilterBarTags()
        Dim view As New MerchSys.Inventory.Views.StockDashboardView()
        Dim pnlFilter As FlowLayoutPanel = TryCast(view.GetType().GetField("_pnlFilterBar", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), FlowLayoutPanel)
        Assert.NotNull(pnlFilter)
        Assert.Equal("FilterBar", pnlFilter.Tag)

        Dim txtFilter As TextBox = CType(view.GetType().GetField("_txtFilter", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), TextBox)
        Assert.Equal("FilterBarItem", txtFilter.Tag)

        Dim cboCategoryFilter As ComboBox = CType(view.GetType().GetField("_cboCategoryFilter", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), ComboBox)
        Assert.Equal("FilterBarItem", cboCategoryFilter.Tag)

        Dim cboStatusFilter As ComboBox = CType(view.GetType().GetField("_cboStatusFilter", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), ComboBox)
        Assert.Equal("FilterBarItem", cboStatusFilter.Tag)
    End Sub

    <Fact>
    Public Sub PurchaseOrderListView_ShouldHaveFilterBarTags()
        Dim view As New MerchSys.Purchasing.Views.PurchaseOrderListView()
        Dim pnlFilter As FlowLayoutPanel = TryCast(view.GetType().GetProperty("pnlFilter", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), FlowLayoutPanel)
        Assert.NotNull(pnlFilter)
        Assert.Equal("FilterBar", pnlFilter.Tag)

        Dim txtSearch As TextBox = TryCast(view.GetType().GetProperty("txtSearch", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), TextBox)
        Assert.Equal("FilterBarItem", txtSearch.Tag)

        Dim cmbStatus As ComboBox = TryCast(view.GetType().GetProperty("cmbStatus", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), ComboBox)
        Assert.Equal("FilterBarItem", cmbStatus.Tag)
    End Sub

    <Fact>
    Public Sub ShrinkageView_ShouldHaveFilterBarTags()
        Dim view As New MerchSys.Inventory.Views.ShrinkageView()
        Dim pnlFilter As FlowLayoutPanel = TryCast(view.GetType().GetProperty("pnlFilter", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), FlowLayoutPanel)
        Assert.NotNull(pnlFilter)
        Assert.Equal("FilterBar", pnlFilter.Tag)

        Dim dtpStart As DateTimePicker = TryCast(view.GetType().GetProperty("dtpStart", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), DateTimePicker)
        Assert.Equal("FilterBarItem", dtpStart.Tag)
    End Sub

    <Fact>
    Public Sub ExpiryMonitorView_ShouldHaveFilterBarTags()
        Dim view As New MerchSys.Inventory.Views.ExpiryMonitorView()
        Dim pnlFilter As FlowLayoutPanel = TryCast(view.GetType().GetProperty("pnlFilter", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), FlowLayoutPanel)
        Assert.NotNull(pnlFilter)
        Assert.Equal("FilterBar", pnlFilter.Tag)

        Dim nudThreshold As NumericUpDown = TryCast(view.GetType().GetProperty("nudThreshold", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), NumericUpDown)
        Assert.Equal("FilterBarItem", nudThreshold.Tag)
    End Sub

    <Fact>
    Public Sub TamperAuditReportView_ShouldHaveFilterBarTags()
        Dim view As New MerchSys.App.Views.Accounting.TamperAuditReportView()
        Dim pnlFilter As FlowLayoutPanel = TryCast(view.GetType().GetProperty("pnlFilter", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), FlowLayoutPanel)
        Assert.NotNull(pnlFilter)
        Assert.Equal("FilterBar", pnlFilter.Tag)

        Dim dtpTo As DateTimePicker = TryCast(view.GetType().GetProperty("dtpTo", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), DateTimePicker)
        Assert.Equal("FilterBarItem", dtpTo.Tag)
    End Sub

    <Fact>
    Public Sub ProductManagementView_ShouldHaveFilterBarTags()
        Dim view As New MerchSys.Inventory.Views.ProductManagementView()
        Dim toolStripProducts As ToolStrip = TryCast(view.GetType().GetProperty("toolStripProducts", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), ToolStrip)
        Assert.NotNull(toolStripProducts)
        Assert.Equal("FilterBar", toolStripProducts.Tag)

        Dim btnAddProduct As ToolStripButton = TryCast(view.GetType().GetProperty("btnAddProduct", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(view), ToolStripButton)
        Assert.Equal("FilterBarItem", btnAddProduct.Tag)
    End Sub



    <Fact>
    Public Sub ThemeService_ShouldApplyCleanlyToAllAffectedViews()
        Dim themeService As New MerchSys.App.Services.Theming.ThemeService(Nothing) ' Passing Nothing for IServiceProvider is fine since we just test ApplyTheme UI part here directly, or we can use ComponentStyler manually

        Dim views As Control() = {
            New OwnerDashboardView(),
            New FinancialOverviewView(),
            New VatReturnView(),
            New VatReliefReportView(),
            New VatSettingsView(),
            New MerchSys.Inventory.Views.StockDashboardView(),
            New MerchSys.Purchasing.Views.PurchaseOrderListView(),
            New MerchSys.Inventory.Views.ShrinkageView(),
            New MerchSys.Inventory.Views.ExpiryMonitorView(),
            New MerchSys.App.Views.Accounting.TamperAuditReportView(),
            New MerchSys.Inventory.Views.ProductManagementView()
        }

        Dim tokens = MerchSys.App.Services.Theming.ThemeTokens.GetTokens(MerchSys.App.Services.Theming.AppTheme.Light)
        For Each view In views
            ' This should not throw exceptions
            Dim pnl = TryCast(view, Panel)
            If pnl IsNot Nothing Then
                MerchSys.App.Services.Theming.ComponentStyler.ApplyFilterBar(pnl, tokens)
            End If
            MerchSys.App.Services.Theming.ComponentStyler.ApplyFilterBarItem(view, tokens)

            ' Apply full theme to catch any missing tags
            themeService.ApplyTheme(view)
        Next
    End Sub

End Class
