Imports System.Text
Imports System.Linq

Namespace Services

    Public Class WhatThisMeansService
        Implements IWhatThisMeansService

        Public Function GenerateOverviewInterpretation(overview As FinancialOverviewDto) As String Implements IWhatThisMeansService.GenerateOverviewInterpretation
            If overview Is Nothing Then Return "No data available to interpret."

            Dim sb As New StringBuilder()

            ' Interprets Today's revenue, MTD revenue, and gross margin
            sb.AppendLine($"Today's revenue is ₱{overview.TodayRevenue:N2} with a gross margin of {overview.GrossMarginToday:N1}%. " &
                          $"For the month-to-date, revenue stands at ₱{overview.MtdRevenue:N2} with a margin of {overview.GrossMarginMtd:N1}%.")

            ' Compares MTD margin with previous month trend if available
            If overview.MonthlyTrends IsNot Nothing AndAlso overview.MonthlyTrends.Count >= 2 Then
                Dim sortedTrends = overview.MonthlyTrends.OrderByDescending(Function(t) t.MonthStart).ToList()
                ' Find previous month's data if possible. MTD is current. We can just take the most recent trend month before current month.
                Dim previousTrend = sortedTrends.Skip(1).FirstOrDefault()
                If previousTrend IsNot Nothing Then
                    If overview.GrossMarginMtd > previousTrend.GrossMarginPercent Then
                        sb.AppendLine($"This month's margin is tracking better than last month's {previousTrend.GrossMarginPercent:N1}%.")
                    ElseIf overview.GrossMarginMtd < previousTrend.GrossMarginPercent Then
                        sb.AppendLine($"Note that this month's margin is currently lower than last month's {previousTrend.GrossMarginPercent:N1}%.")
                    Else
                        sb.AppendLine($"This month's margin is stable compared to last month.")
                    End If
                End If
            End If

            ' Evaluates working capital and liquidity
            sb.AppendLine($"Your current inventory value is ₱{overview.InventoryValue:N2}.")
            If overview.TotalAP > overview.TotalAR Then
                sb.AppendLine($"⚠️ Working Capital Alert: Your payables (₱{overview.TotalAP:N2}) exceed your receivables (₱{overview.TotalAR:N2}). " &
                              "Please monitor cash flow closely to ensure timely vendor payments.")
            Else
                sb.AppendLine($"Liquidity looks stable with receivables (₱{overview.TotalAR:N2}) adequately covering payables (₱{overview.TotalAP:N2}).")
            End If

            ' Highlights top-selling product
            If overview.TopSellingProducts IsNot Nothing AndAlso overview.TopSellingProducts.Any() Then
                Dim topProduct = overview.TopSellingProducts.OrderByDescending(Function(p) p.TotalRevenue).First()
                sb.AppendLine($"Your top-selling product by revenue is {topProduct.ProductName}, generating ₱{topProduct.TotalRevenue:N2} from {topProduct.QuantitySold:N1} units sold.")
            End If

            Return sb.ToString().TrimEnd()
        End Function

        Public Function GenerateIncomeStatementInterpretation(statement As IncomeStatementDto, Optional previousMargin As Decimal? = Nothing) As String Implements IWhatThisMeansService.GenerateIncomeStatementInterpretation
            If statement Is Nothing Then Return "No statement available to interpret."

            Dim sb As New StringBuilder()

            sb.AppendLine($"For this period, Net Sales were ₱{statement.NetSales:N2} and Cost of Goods Sold (COGS) were ₱{statement.CostOfGoodsSold:N2}, " &
                          $"resulting in a Gross Profit of ₱{statement.GrossProfit:N2} ({statement.GrossMarginPercent:N1}% margin).")

            sb.AppendLine($"Operating Expenses totaled ₱{statement.OperatingExpenses:N2}.")

            If statement.NetIncome > 0 Then
                sb.AppendLine($"The business operated profitably, ending with a Net Income of ₱{statement.NetIncome:N2} ({statement.NetMarginPercent:N1}% net margin).")
            ElseIf statement.NetIncome < 0 Then
                sb.AppendLine($"The business operated at a net loss of ₱{Math.Abs(statement.NetIncome):N2} for this period.")
            Else
                sb.AppendLine($"The business broke even this period.")
            End If

            If statement.OperatingExpensesByCategory IsNot Nothing AndAlso statement.OperatingExpensesByCategory.Any() Then
                Dim topExpense = statement.OperatingExpensesByCategory.OrderByDescending(Function(e) e.Value).First()
                sb.AppendLine($"The largest operating expense was {topExpense.Key} at ₱{topExpense.Value:N2}.")
            End If

            If previousMargin.HasValue Then
                If previousMargin.Value - statement.GrossMarginPercent >= 3.0D Then
                    sb.AppendLine($"⚠️ Margin Compression Warning: Your gross margin has dropped by {previousMargin.Value - statement.GrossMarginPercent:N1}% compared to the previous period.")
                End If
            End If

            Return sb.ToString().TrimEnd()
        End Function

        Public Function GenerateSalesSummaryInterpretation(summary As AccountingSalesSummaryDto) As String Implements IWhatThisMeansService.GenerateSalesSummaryInterpretation
            If summary Is Nothing Then Return "No summary available to interpret."

            Dim sb As New StringBuilder()

            sb.AppendLine($"Total Net Sales for this period reached ₱{summary.TotalNetSales:N2} across {summary.TransactionCount} transactions, " &
                          $"with an average transaction value of ₱{summary.AvgTransactionValue:N2}.")

            If summary.PaymentBreakdowns IsNot Nothing AndAlso summary.PaymentBreakdowns.Any() Then
                Dim topPayment = summary.PaymentBreakdowns.OrderByDescending(Function(p) p.GrossSales).First()
                sb.AppendLine($"The most popular payment method was {topPayment.PaymentMethodName}, accounting for {topPayment.PercentageOfTotal:N1}% of total sales.")
            End If

            If summary.CreditSalesPercentage >= 30.0D OrElse summary.HasCreditWarning Then
                sb.AppendLine($"⚠️ Credit Alert: Credit sales represent {summary.CreditSalesPercentage:N1}% of total sales (exceeds 30% risk threshold). " &
                              "Follow-up on outstanding accounts receivable is recommended to maintain healthy cash flow.")
            Else
                sb.AppendLine($"Credit sales represent {summary.CreditSalesPercentage:N1}% of total sales. Cash flow and liquidity remain stable.")
            End If

            Return sb.ToString().TrimEnd()
        End Function

        Public Function GenerateMarginAlert(currentMargin As Decimal, previousMargin As Decimal, Optional thresholdPercent As Decimal = 3.0D) As String Implements IWhatThisMeansService.GenerateMarginAlert
            If previousMargin - currentMargin >= thresholdPercent Then
                Return $"⚠️ Alert: Gross margin has compressed by {previousMargin - currentMargin:N1}%, which exceeds your tolerance threshold of {thresholdPercent:N1}%."
            ElseIf currentMargin > previousMargin Then
                Return $"Good news: Gross margin has improved by {currentMargin - previousMargin:N1}% compared to the previous period."
            Else
                Return "Gross margin is stable within the expected range."
            End If
        End Function

    End Class

End Namespace