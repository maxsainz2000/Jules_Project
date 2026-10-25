#If DEBUG Then
Imports System.Text
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Debug
    Public Class VatLedgerSchemaHarnessResult
        Public Property CheckName As String
        Public Property Passed As Boolean
        Public Property Details As String
    End Class

    Public Class VatLedgerSchemaReport
        Public Property Timestamp As DateTime
        Public Property Results As New List(Of VatLedgerSchemaHarnessResult)
        Public Property AllPassed As Boolean

        Public Function ToMarkdown() As String
            Dim sb As New StringBuilder()
            sb.AppendLine($"# VAT Ledger Schema Verification Report")
            sb.AppendLine($"**Generated:** {Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}")
            sb.AppendLine($"**Overall Status:** {If(AllPassed, "PASS", "FAIL")}")
            sb.AppendLine()
            sb.AppendLine("| Check | Status | Details |")
            sb.AppendLine("|-------|--------|---------|")
            For Each result In Results
                Dim statusStr = If(result.Passed, "✅ Pass", "❌ Fail")
                Dim safeDetails = result.Details.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ")
                sb.AppendLine($"| {result.CheckName} | {statusStr} | {safeDetails} |")
            Next
            Return sb.ToString()
        End Function
    End Class

    Public Class VatLedgerSchemaHarness
        Public Async Function ExecuteAsync(context As AccountingDbContext) As Task(Of VatLedgerSchemaReport)
            Dim report As New VatLedgerSchemaReport() With {.Timestamp = DateTime.UtcNow}
            Dim model = context.Model

            ' Check 1: Model Registration & Table Mapping
            Dim check1Passed As Boolean = True
            Dim check1Details As New StringBuilder()

            Dim vatReturnType = model.FindEntityType(GetType(VatReturn))
            If vatReturnType Is Nothing Then
                check1Passed = False
                check1Details.Append("VatReturn entity missing. ")
            ElseIf vatReturnType.GetTableName() <> "Acc_VatReturns" Then
                check1Passed = False
                check1Details.Append($"VatReturn maps to {vatReturnType.GetTableName()} instead of Acc_VatReturns. ")
            End If

            Dim vatReturnLineType = model.FindEntityType(GetType(VatReturnLine))
            If vatReturnLineType Is Nothing Then
                check1Passed = False
                check1Details.Append("VatReturnLine entity missing. ")
            ElseIf vatReturnLineType.GetTableName() <> "Acc_VatReturnLines" Then
                check1Passed = False
                check1Details.Append($"VatReturnLine maps to {vatReturnLineType.GetTableName()} instead of Acc_VatReturnLines. ")
            End If

            Dim revenueRecordType = model.FindEntityType(GetType(RevenueRecord))
            If revenueRecordType Is Nothing Then
                check1Passed = False
                check1Details.Append("RevenueRecord entity missing. ")
            Else
                Dim vatProps = {"VatableAmount", "VatExemptAmount", "ZeroRatedAmount", "OutputVat", "InputVat"}
                For Each p In vatProps
                    If revenueRecordType.FindProperty(p) Is Nothing Then
                        check1Passed = False
                        check1Details.Append($"RevenueRecord missing {p}. ")
                    End If
                Next
            End If

            Dim expenseRecordType = model.FindEntityType(GetType(ExpenseRecord))
            If expenseRecordType Is Nothing Then
                check1Passed = False
                check1Details.Append("ExpenseRecord entity missing. ")
            Else
                Dim vatProps = {"VatableAmount", "VatExemptAmount", "ZeroRatedAmount", "OutputVat", "InputVat"}
                For Each p In vatProps
                    If expenseRecordType.FindProperty(p) Is Nothing Then
                        check1Passed = False
                        check1Details.Append($"ExpenseRecord missing {p}. ")
                    End If
                Next
            End If

            If check1Passed Then check1Details.Append("All required entities and properties mapped correctly.")

            report.Results.Add(New VatLedgerSchemaHarnessResult With {
                .CheckName = "Model Registration & Table Mapping",
                .Passed = check1Passed,
                .Details = check1Details.ToString().Trim()
            })

            ' Check 2: Unique Period/Form Constraint
            Dim check2Passed As Boolean = False
            Dim check2Details As String = "VatReturn unique index on Year, Period, PeriodType, FormType missing."
            If vatReturnType IsNot Nothing Then
                Dim indexes = vatReturnType.GetIndexes()
                For Each idx In indexes
                    If idx.IsUnique AndAlso idx.Properties.Count = 4 Then
                        Dim pNames = idx.Properties.Select(Function(p) p.Name).ToList()
                        If pNames.Contains("Year") AndAlso pNames.Contains("Period") AndAlso pNames.Contains("PeriodType") AndAlso pNames.Contains("FormType") Then
                            check2Passed = True
                            check2Details = "Unique constraint found."
                            Exit For
                        End If
                    End If
                Next
            End If

            report.Results.Add(New VatLedgerSchemaHarnessResult With {
                .CheckName = "Unique Period/Form Constraint",
                .Passed = check2Passed,
                .Details = check2Details
            })

            ' Check 3: Cascade Deletion
            Dim check3Passed As Boolean = False
            Dim check3Details As String = "VatReturnLine -> VatReturn foreign key cascade delete missing."
            If vatReturnLineType IsNot Nothing Then
                Dim fks = vatReturnLineType.GetForeignKeys()
                For Each fk In fks
                    If fk.PrincipalEntityType.ClrType Is GetType(VatReturn) Then
                        If fk.DeleteBehavior = DeleteBehavior.Cascade Then
                            check3Passed = True
                            check3Details = "Cascade delete configured."
                        Else
                            check3Details = $"Foreign key configured with {fk.DeleteBehavior} instead of Cascade."
                        End If
                        Exit For
                    End If
                Next
            End If

            report.Results.Add(New VatLedgerSchemaHarnessResult With {
                .CheckName = "Cascade Deletion",
                .Passed = check3Passed,
                .Details = check3Details
            })

            ' Check 4: Precision & RowVersion Token
            Dim check4Passed As Boolean = True
            Dim check4Details As New StringBuilder()

            If vatReturnType IsNot Nothing Then
                Dim precisionProps = {"TotalSales", "TotalPurchases", "OutputVatTotal", "InputVatTotal", "NetVatPayable"}
                For Each pName In precisionProps
                    Dim prop = vatReturnType.FindProperty(pName)
                    If prop Is Nothing Then
                        check4Passed = False
                        check4Details.Append($"Missing {pName}. ")
                    Else
                        Dim precision = prop.GetPrecision()
                        Dim scale = prop.GetScale()
                        If precision Is Nothing OrElse scale Is Nothing OrElse precision.Value <> 18 OrElse scale.Value <> 2 Then
                            check4Passed = False
                            check4Details.Append($"{pName} precision is ({precision},{scale}) instead of (18,2). ")
                        End If
                    End If
                Next

                Dim rvProp = vatReturnType.FindProperty("RowVersion")
                If rvProp Is Nothing Then
                    check4Passed = False
                    check4Details.Append("RowVersion missing. ")
                ElseIf Not rvProp.IsConcurrencyToken Then
                    check4Passed = False
                    check4Details.Append("RowVersion is not a concurrency token. ")
                End If
            Else
                check4Passed = False
                check4Details.Append("VatReturn entity missing.")
            End If

            If check4Passed Then check4Details.Append("Precision and RowVersion configured correctly.")

            report.Results.Add(New VatLedgerSchemaHarnessResult With {
                .CheckName = "Precision & RowVersion Token",
                .Passed = check4Passed,
                .Details = check4Details.ToString().Trim()
            })

            report.AllPassed = report.Results.All(Function(r) r.Passed)

            Return Await Task.FromResult(report)
        End Function
    End Class
End Namespace
#End If