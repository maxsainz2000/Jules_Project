Imports System.Threading
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.Accounting.Enums
Imports MerchSys.Accounting.Exceptions

Namespace Services
    Public Class VatReportingService
        Implements IVatReportingService

        Private ReadOnly _dbContext As AccountingDbContext
        Private ReadOnly _mediator As IMediator
        Private ReadOnly _logger As ILogger(Of VatReportingService)

        Public Sub New(dbContext As AccountingDbContext, mediator As IMediator, logger As ILogger(Of VatReportingService))
            _dbContext = dbContext
            _mediator = mediator
            _logger = logger
        End Sub

        Public Async Function GenerateVatReturnAsync(year As Integer, period As Integer, formType As VatReturnFormType, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatReturn) Implements IVatReportingService.GenerateVatReturnAsync
            Dim existing = Await _dbContext.VatReturns.Include(Function(v) v.Lines).FirstOrDefaultAsync(
                Function(v) v.Year = year AndAlso v.Period = period AndAlso v.FormType = formType,
                cancellationToken)

            If existing IsNot Nothing AndAlso existing.FilingStatus = VatFilingStatus.Filed Then
                Throw New VatReturnLockedException("VAT Return is filed and locked.", existing.Id, year, period, formType)
            End If

            Dim vatReturn As VatReturn
            If existing IsNot Nothing Then
                vatReturn = existing
                vatReturn.Lines.Clear()
            Else
                vatReturn = New VatReturn With {
                    .Year = year,
                    .Period = period,
                    .FormType = formType,
                    .PeriodType = If(formType = VatReturnFormType.Form2550M, VatReturnPeriodType.Monthly, VatReturnPeriodType.Quarterly),
                    .FilingStatus = VatFilingStatus.Generated
                }
                _dbContext.VatReturns.Add(vatReturn)
            End If

            Dim periodStart As DateTime
            Dim periodEnd As DateTime
            
            If vatReturn.PeriodType = VatReturnPeriodType.Monthly Then
                periodStart = New DateTime(year, period, 1, 0, 0, 0, DateTimeKind.Utc)
                periodEnd = periodStart.AddMonths(1).AddSeconds(-1)
            Else
                periodStart = New DateTime(year, (period - 1) * 3 + 1, 1, 0, 0, 0, DateTimeKind.Utc)
                periodEnd = periodStart.AddMonths(3).AddSeconds(-1)
            End If
            
            vatReturn.PeriodStart = periodStart
            vatReturn.PeriodEnd = periodEnd

            Dim revenues = Await _dbContext.RevenueRecords.Where(Function(r) r.RecordDate >= periodStart AndAlso r.RecordDate <= periodEnd).ToListAsync(cancellationToken)
            Dim expenses = Await _dbContext.ExpenseRecords.Where(Function(e) e.ExpenseDate >= periodStart AndAlso e.ExpenseDate <= periodEnd).ToListAsync(cancellationToken)

            vatReturn.TotalSales = revenues.Sum(Function(r) r.VatableAmount + r.VatExemptAmount + r.ZeroRatedAmount)
            vatReturn.OutputVatTotal = revenues.Sum(Function(r) r.OutputVat)
            
            vatReturn.TotalPurchases = expenses.Sum(Function(e) e.VatableAmount + e.VatExemptAmount + e.ZeroRatedAmount)
            vatReturn.InputVatTotal = expenses.Sum(Function(e) e.InputVat)
            
            vatReturn.NetVatPayable = vatReturn.OutputVatTotal - vatReturn.InputVatTotal

            vatReturn.Lines.Add(New VatReturnLine With {
                .LineNumber = 1,
                .LineCode = "SALES",
                .Description = "Total Sales",
                .Amount = vatReturn.TotalSales,
                .TaxRate = 0,
                .TaxAmount = vatReturn.OutputVatTotal
            })

            vatReturn.Lines.Add(New VatReturnLine With {
                .LineNumber = 2,
                .LineCode = "PURCH",
                .Description = "Total Purchases",
                .Amount = vatReturn.TotalPurchases,
                .TaxRate = 0,
                .TaxAmount = vatReturn.InputVatTotal
            })

            Await _dbContext.SaveChangesAsync(cancellationToken)
            Return vatReturn
        End Function

        Public Async Function GetVatReturnAsync(returnId As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatReturn) Implements IVatReportingService.GetVatReturnAsync
            Return Await _dbContext.VatReturns.Include(Function(v) v.Lines).FirstOrDefaultAsync(Function(v) v.Id = returnId, cancellationToken)
        End Function

        Public Async Function ListVatReturnsAsync(Optional year As Integer? = Nothing, Optional cancellationToken As CancellationToken = Nothing) As Task(Of List(Of VatReturn)) Implements IVatReportingService.ListVatReturnsAsync
            Dim query = _dbContext.VatReturns.AsQueryable()
            If year.HasValue Then
                query = query.Where(Function(v) v.Year = year.Value)
            End If
            Return Await query.OrderByDescending(Function(v) v.Year).ThenByDescending(Function(v) v.Period).ToListAsync(cancellationToken)
        End Function

        Public Async Function FileVatReturnAsync(returnId As Integer, filedBy As String, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatReturn) Implements IVatReportingService.FileVatReturnAsync
            Dim vatReturn = Await _dbContext.VatReturns.FindAsync(New Object() {returnId}, cancellationToken)
            If vatReturn IsNot Nothing Then
                vatReturn.FilingStatus = VatFilingStatus.Filed
                vatReturn.FiledAt = DateTime.UtcNow
                vatReturn.FiledBy = filedBy
                Await _dbContext.SaveChangesAsync(cancellationToken)
            End If
            Return vatReturn
        End Function

        Public Async Function AmendVatReturnAsync(returnId As Integer, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatReturn) Implements IVatReportingService.AmendVatReturnAsync
            Dim vatReturn = Await _dbContext.VatReturns.FindAsync(New Object() {returnId}, cancellationToken)
            If vatReturn IsNot Nothing Then
                vatReturn.FilingStatus = VatFilingStatus.Amended
                Await _dbContext.SaveChangesAsync(cancellationToken)
            End If
            Return vatReturn
        End Function
    End Class
End Namespace
