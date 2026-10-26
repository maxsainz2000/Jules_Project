Imports System.Drawing
Imports System.Windows.Forms

Namespace Services.Theming
    Public Class ComponentStyler

        ' Button Variants
        Public Shared Sub ApplyPrimaryButton(btn As Button, tokens As ThemeTokens)
            btn.BackColor = tokens.Accent
            btn.ForeColor = Color.White
            btn.FlatStyle = FlatStyle.Flat
        End Sub

        Public Shared Sub ApplySuccessButton(btn As Button, tokens As ThemeTokens)
            btn.BackColor = tokens.Success
            btn.ForeColor = Color.White
            btn.FlatStyle = FlatStyle.Flat
        End Sub

        Public Shared Sub ApplyDangerButton(btn As Button, tokens As ThemeTokens)
            btn.BackColor = tokens.Danger
            btn.ForeColor = Color.White
            btn.FlatStyle = FlatStyle.Flat
        End Sub

        Public Shared Sub ApplyWarningButton(btn As Button, tokens As ThemeTokens)
            btn.BackColor = tokens.Warning
            btn.ForeColor = Color.White
            btn.FlatStyle = FlatStyle.Flat
        End Sub

        Public Shared Sub ApplySecondaryButton(btn As Button, tokens As ThemeTokens)
            btn.BackColor = tokens.SurfaceBackground
            btn.ForeColor = tokens.PrimaryText
            btn.FlatAppearance.BorderColor = tokens.Border
            btn.FlatAppearance.BorderSize = 1
            btn.FlatStyle = FlatStyle.Flat
        End Sub

        Public Shared Sub ApplySubtleButton(btn As Button, tokens As ThemeTokens)
            btn.BackColor = Color.Transparent
            btn.ForeColor = tokens.PrimaryText
            btn.FlatAppearance.BorderSize = 0
            btn.FlatStyle = FlatStyle.Flat
        End Sub

        ' Card Containers
        Public Shared Sub ApplyCard(pnl As Panel, tokens As ThemeTokens)
            pnl.BackColor = tokens.CardBackground
            pnl.Padding = New Padding(12)
        End Sub

        Public Shared Sub ApplyPrimaryMetricCard(pnl As Panel, tokens As ThemeTokens)
            pnl.BackColor = tokens.CardBackground
            pnl.Padding = New Padding(16)
        End Sub

        Public Shared Sub ApplySecondaryMetricCard(pnl As Panel, tokens As ThemeTokens)
            pnl.BackColor = tokens.CardBackground
            pnl.Padding = New Padding(12)
        End Sub

        Public Shared Sub ApplyTertiaryMetricCard(pnl As Panel, tokens As ThemeTokens)
            pnl.BackColor = tokens.SurfaceBackground
            pnl.Padding = New Padding(8)
        End Sub

        Public Shared Sub ApplyAlertCard(pnl As Panel, tokens As ThemeTokens, severity As String)
            ApplyCard(pnl, tokens)
            ' Adjust styling based on severity
            Select Case severity.ToLowerInvariant()
                Case "danger"
                    pnl.BackColor = Color.FromArgb(40, tokens.Danger.R, tokens.Danger.G, tokens.Danger.B)
                Case "warning"
                    pnl.BackColor = Color.FromArgb(40, tokens.Warning.R, tokens.Warning.G, tokens.Warning.B)
                Case "success"
                    pnl.BackColor = Color.FromArgb(40, tokens.Success.R, tokens.Success.G, tokens.Success.B)
                Case Else
                    pnl.BackColor = Color.FromArgb(40, tokens.Accent.R, tokens.Accent.G, tokens.Accent.B)
            End Select
        End Sub

        ' Typography Styles
        Public Shared Sub ApplyPageTitle(lbl As Label, tokens As ThemeTokens)
            If tokens.DefaultFont IsNot Nothing Then
                lbl.Font = New Font(tokens.DefaultFont.FontFamily, 16.0F, FontStyle.Bold)
            End If
            lbl.ForeColor = tokens.PrimaryText
        End Sub

        Public Shared Sub ApplySectionHeader(lbl As Label, tokens As ThemeTokens)
            If tokens.HeaderFont IsNot Nothing Then
                lbl.Font = New Font(tokens.HeaderFont.FontFamily, 12.0F, FontStyle.Bold)
            End If
            lbl.ForeColor = tokens.PrimaryText
        End Sub

        Public Shared Sub ApplySubtitle(lbl As Label, tokens As ThemeTokens)
            If tokens.DefaultFont IsNot Nothing Then
                lbl.Font = New Font(tokens.DefaultFont.FontFamily, 10.0F, FontStyle.Regular)
            End If
            lbl.ForeColor = tokens.SecondaryText
        End Sub

        Public Shared Sub ApplyCaption(lbl As Label, tokens As ThemeTokens)
            If tokens.DefaultFont IsNot Nothing Then
                lbl.Font = New Font(tokens.DefaultFont.FontFamily, 8.0F, FontStyle.Regular)
            End If
            lbl.ForeColor = tokens.MutedText
        End Sub

        Public Shared Sub ApplyMetricValue(lbl As Label, tokens As ThemeTokens, Optional large As Boolean = False)
            Dim fontSize = If(large, 24.0F, 18.0F)
            If tokens.DefaultFont IsNot Nothing Then
                lbl.Font = New Font(tokens.DefaultFont.FontFamily, fontSize, FontStyle.Bold)
            End If
            lbl.ForeColor = tokens.PrimaryText
        End Sub

        Public Shared Sub ApplyPrimaryMetricValue(lbl As Label, tokens As ThemeTokens)
            If tokens.DefaultFont IsNot Nothing Then
                lbl.Font = New Font(tokens.DefaultFont.FontFamily, 24.0F, FontStyle.Bold)
            End If
            lbl.ForeColor = tokens.PrimaryText
        End Sub

        Public Shared Sub ApplySecondaryMetricValue(lbl As Label, tokens As ThemeTokens)
            If tokens.DefaultFont IsNot Nothing Then
                lbl.Font = New Font(tokens.DefaultFont.FontFamily, 16.0F, FontStyle.Bold)
            End If
            lbl.ForeColor = tokens.PrimaryText
        End Sub

        Public Shared Sub ApplyMetricCaption(lbl As Label, tokens As ThemeTokens)
            If tokens.DefaultFont IsNot Nothing Then
                lbl.Font = New Font(tokens.DefaultFont.FontFamily, 9.0F, FontStyle.Regular)
            End If
            lbl.ForeColor = tokens.MutedText
        End Sub

        ' FilterBar Containers
        Public Shared Sub ApplyFilterBar(pnl As Panel, tokens As ThemeTokens)
            pnl.BackColor = tokens.SurfaceBackground
            pnl.Padding = New Padding(8)
        End Sub

        Public Shared Sub ApplyFilterBarItem(ctrl As Control, tokens As ThemeTokens)
            ctrl.Margin = New Padding(4, 2, 4, 2)
        End Sub

        ' DataGridView Helpers
        Public Shared Sub ApplyNumericCellFormatting(col As DataGridViewColumn)
            If col IsNot Nothing Then
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                col.DefaultCellStyle.Format = "N2"
            End If
        End Sub

        Public Shared Sub ApplySemanticStatusRow(row As DataGridViewRow, status As String, tokens As ThemeTokens)
            If row IsNot Nothing AndAlso Not String.IsNullOrEmpty(status) Then
                Dim lowerStatus = status.ToLowerInvariant()
                If lowerStatus = "completed" OrElse lowerStatus = "received" Then
                    row.DefaultCellStyle.BackColor = Color.FromArgb(40, tokens.Success.R, tokens.Success.G, tokens.Success.B)
                    row.DefaultCellStyle.ForeColor = tokens.Success
                ElseIf lowerStatus = "pending" Then
                    row.DefaultCellStyle.BackColor = Color.FromArgb(40, tokens.Warning.R, tokens.Warning.G, tokens.Warning.B)
                    row.DefaultCellStyle.ForeColor = tokens.Warning
                ElseIf lowerStatus = "overdue" OrElse lowerStatus = "cancelled" Then
                    row.DefaultCellStyle.BackColor = Color.FromArgb(40, tokens.Danger.R, tokens.Danger.G, tokens.Danger.B)
                    row.DefaultCellStyle.ForeColor = tokens.Danger
                End If
            End If
        End Sub

    End Class
End Namespace