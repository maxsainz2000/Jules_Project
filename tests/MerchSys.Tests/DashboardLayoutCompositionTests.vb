Imports Xunit
Imports MerchSys.App.Views
Imports MerchSys.App.Views.Accounting
Imports MerchSys.Inventory.Views
Imports MerchSys.Purchasing.Views.Purchasing
Imports MerchSys.App.Services.Theming
Imports System.Windows.Forms
Imports System.Reflection

Namespace Tests
    Public Class DashboardLayoutCompositionTests

        Private Function GetControl(view As Object, name As String) As Control
            Dim propInfo = view.GetType().GetProperty(name, BindingFlags.NonPublic Or BindingFlags.Instance)
            If propInfo IsNot Nothing Then
                Return CType(propInfo.GetValue(view), Control)
            End If
            Dim fieldInfo = view.GetType().GetField(name, BindingFlags.NonPublic Or BindingFlags.Instance)
            If fieldInfo IsNot Nothing Then
                Return CType(fieldInfo.GetValue(view), Control)
            End If

            Dim fieldInfo2 = view.GetType().GetField("_" & name, BindingFlags.NonPublic Or BindingFlags.Instance)
            If fieldInfo2 IsNot Nothing Then
                Return CType(fieldInfo2.GetValue(view), Control)
            End If

            Return Nothing
        End Function

        <Fact>
        Public Sub OwnerDashboardView_HasCorrectTags()
            Using view As New OwnerDashboardView()
                Assert.Equal("PrimaryMetricCard", GetControl(view, "pnlKpi1")?.Tag)
                Assert.Equal("PrimaryMetricValue", GetControl(view, "lblKpi1Value")?.Tag)
                Assert.Equal("SectionHeader", GetControl(view, "lblKpi1Title")?.Tag)
                Assert.Equal("MetricCaption", GetControl(view, "lblKpi1Interpretation")?.Tag)

                Assert.Equal("SecondaryMetricCard", GetControl(view, "pnlKpi2")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblKpi2Value")?.Tag)
                Assert.Equal("MetricCaption", GetControl(view, "lblKpi2Interpretation")?.Tag)

                Assert.Equal("SecondaryMetricCard", GetControl(view, "pnlKpi3")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblKpi3Value")?.Tag)
                Assert.Equal("MetricCaption", GetControl(view, "lblKpi3Interpretation")?.Tag)

                Assert.Equal("SecondaryMetricCard", GetControl(view, "pnlKpi4")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblKpi4Value")?.Tag)
                Assert.Equal("MetricCaption", GetControl(view, "lblKpi4Interpretation")?.Tag)
            End Using
        End Sub

        <Fact>
        Public Sub FinancialOverviewView_HasCorrectTags()
            Using view As New FinancialOverviewView()
                Assert.Equal("PrimaryMetricValue", GetControl(view, "lblMtdRevenue")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblTodayRevenue")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblGrossMarginToday")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblTotalAR")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblTotalAP")?.Tag)
                Assert.Equal("MetricCaption", GetControl(view, "lblYtdRevenue")?.Tag)
                Assert.Equal("MetricCaption", GetControl(view, "lblInventoryValue")?.Tag)
                Assert.Equal("TertiaryMetricCard", GetControl(view, "pnlVatTile")?.Tag)
            End Using
        End Sub

        <Fact>
        Public Sub SalesSummaryView_HasCorrectTags()
            Using view As New SalesSummaryView(Nothing)
                Assert.Equal("PrimaryMetricCard", GetControl(view, "pnlNetSales")?.Tag)
                Assert.Equal("PrimaryMetricValue", GetControl(view, "lblNetSalesValue")?.Tag)
                Assert.Equal("MetricCaption", GetControl(view, "lblNetSalesTitle")?.Tag)

                Assert.Equal("SecondaryMetricCard", GetControl(view, "pnlTransactions")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblTransactionsValue")?.Tag)
                Assert.Equal("MetricCaption", GetControl(view, "lblTransactionsTitle")?.Tag)

                Assert.Equal("SecondaryMetricCard", GetControl(view, "pnlAvgTransaction")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblAvgTransactionValue")?.Tag)
                Assert.Equal("MetricCaption", GetControl(view, "lblAvgTransactionTitle")?.Tag)

                Assert.Equal("SecondaryMetricCard", GetControl(view, "pnlReturns")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblReturnsValue")?.Tag)
                Assert.Equal("MetricCaption", GetControl(view, "lblReturnsTitle")?.Tag)
            End Using
        End Sub

        <Fact>
        Public Sub StockDashboardView_HasCorrectTags()
            Using view As New StockDashboardView()
                Assert.Equal("PrimaryMetricValue", GetControl(view, "lblCriticalRiskCount")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblTotalStockValue")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblLowStockCount")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblExpiredBatchCount")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblTotalProducts")?.Tag)
            End Using
        End Sub

        <Fact>
        Public Sub ExpiryMonitorView_HasCorrectTags()
            Using view As New ExpiryMonitorView()
                Assert.Equal("PrimaryMetricValue", GetControl(view, "lblTotalValueAtRisk")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblNearExpiry")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblExpired")?.Tag)
            End Using
        End Sub

        <Fact>
        Public Sub ShrinkageView_HasCorrectTags()
            Using view As New ShrinkageView()
                Assert.Equal("PrimaryMetricValue", GetControl(view, "lblTotalValue")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblTotalItems")?.Tag)
                Assert.Equal("PrimaryMetricCard", GetControl(view, "pnlSummary")?.Tag)
            End Using
        End Sub

        <Fact>
        Public Sub VatReturnView_HasCorrectTags()
            Using view As New VatReturnView(Nothing)
                Assert.Equal("PrimaryMetricValue", GetControl(view, "lblNetVatPayable")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblTotalSales")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblTotalPurchases")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblOutputVat")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblInputVat")?.Tag)
                Assert.Equal("PrimaryMetricCard", GetControl(view, "pnlSummary")?.Tag)
            End Using
        End Sub

        <Fact>
        Public Sub VatReliefReportView_HasCorrectTags()
            Using view As New VatReliefReportView(Nothing)
                Assert.Equal("PrimaryMetricCard", GetControl(view, "pnlNetVatBanner")?.Tag)
                Assert.Equal("PrimaryMetricValue", GetControl(view, "lblNetVatPayable")?.Tag)
                Assert.Equal("SecondaryMetricCard", GetControl(view, "pnlSalesCard")?.Tag)
                Assert.Equal("SecondaryMetricCard", GetControl(view, "pnlPurchasesCard")?.Tag)
            End Using
        End Sub

        <Fact>
        Public Sub IncomeStatementView_HasCorrectTags()
            Using view As New IncomeStatementView(Nothing)
                Assert.Equal("PrimaryMetricValue", GetControl(view, "lblNetIncome")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblNetSales")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblGrossProfit")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblCostOfGoodsSold")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblOperatingExpenses")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblGrossMarginPercent")?.Tag)
                Assert.Equal("SecondaryMetricValue", GetControl(view, "lblNetMarginPercent")?.Tag)
            End Using
        End Sub

        <Fact>
        Public Sub APLedgerView_HasCorrectTags()
            Using view As New APLedgerView()
                Assert.Equal("PrimaryMetricValue", GetControl(view, "lblTotalOutstanding")?.Tag)
            End Using
        End Sub

        <Fact>
        Public Sub ApplyTheme_DoesNotThrowExceptions()
            Dim themeService As New ThemeService()
            themeService.SetTheme(AppTheme.Dark)

            Using view1 As New OwnerDashboardView()
                Dim ex1 = Record.Exception(Sub() themeService.ApplyTheme(view1))
                Assert.Null(ex1)
            End Using
            Using view2 As New FinancialOverviewView()
                Dim ex2 = Record.Exception(Sub() themeService.ApplyTheme(view2))
                Assert.Null(ex2)
            End Using
            Using view3 As New SalesSummaryView(Nothing)
                Dim ex3 = Record.Exception(Sub() themeService.ApplyTheme(view3))
                Assert.Null(ex3)
            End Using
            Using view4 As New StockDashboardView()
                Dim ex4 = Record.Exception(Sub() themeService.ApplyTheme(view4))
                Assert.Null(ex4)
            End Using
            Using view5 As New ExpiryMonitorView()
                Dim ex5 = Record.Exception(Sub() themeService.ApplyTheme(view5))
                Assert.Null(ex5)
            End Using
            Using view6 As New ShrinkageView()
                Dim ex6 = Record.Exception(Sub() themeService.ApplyTheme(view6))
                Assert.Null(ex6)
            End Using
            Using view7 As New VatReturnView(Nothing)
                Dim ex7 = Record.Exception(Sub() themeService.ApplyTheme(view7))
                Assert.Null(ex7)
            End Using
            Using view8 As New VatReliefReportView(Nothing)
                Dim ex8 = Record.Exception(Sub() themeService.ApplyTheme(view8))
                Assert.Null(ex8)
            End Using
            Using view9 As New IncomeStatementView(Nothing)
                Dim ex9 = Record.Exception(Sub() themeService.ApplyTheme(view9))
                Assert.Null(ex9)
            End Using
            Using view10 As New APLedgerView()
                Dim ex10 = Record.Exception(Sub() themeService.ApplyTheme(view10))
                Assert.Null(ex10)
            End Using
        End Sub

    End Class
End Namespace
