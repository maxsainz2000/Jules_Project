Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Handlers
    Public Class SaleCompletedWithVatHandler
        Implements MediatR.INotificationHandler(Of MerchSys.SharedKernel.Events.SaleCompletedWithVatEvent)

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(notification As MerchSys.SharedKernel.Events.SaleCompletedWithVatEvent, cancellationToken As CancellationToken) As Task Implements MediatR.INotificationHandler(Of MerchSys.SharedKernel.Events.SaleCompletedWithVatEvent).Handle
            For Each item In notification.Items
                Dim existing = Await _dbContext.RevenueRecords.FirstOrDefaultAsync(Function(r) r.SourceTransactionId = notification.SaleId AndAlso r.ProductId = item.ProductId, cancellationToken)

                If existing Is Nothing Then
                    Dim record As New Entities.RevenueRecord With {
                        .SourceTransactionId = notification.SaleId,
                        .ProductId = item.ProductId,
                        .TotalRevenue = item.TotalAmount,
                        .VatableAmount = item.VatableAmount,
                        .VatExemptAmount = item.VatExemptAmount,
                        .ZeroRatedAmount = item.ZeroRatedAmount,
                        .OutputVat = item.VatAmount,
                        .VatTreatment = item.Treatment.ToString(),
                        .RecordDate = notification.CompletedAt
                    }
                    _dbContext.RevenueRecords.Add(record)
                Else
                    existing.TotalRevenue = item.TotalAmount
                    existing.VatableAmount = item.VatableAmount
                    existing.VatExemptAmount = item.VatExemptAmount
                    existing.ZeroRatedAmount = item.ZeroRatedAmount
                    existing.OutputVat = item.VatAmount
                    existing.VatTreatment = item.Treatment.ToString()
                End If
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace