Imports System.Threading
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.SharedKernel.Events

Namespace Handlers
    Public Class SaleCompletedWithVatHandler
        Implements INotificationHandler(Of SaleCompletedWithVatEvent)

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(notification As SaleCompletedWithVatEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of SaleCompletedWithVatEvent).Handle
            For Each item In notification.Items
                Dim record = Await _dbContext.RevenueRecords.FirstOrDefaultAsync(
                    Function(r) r.SourceTransactionId = notification.SaleId AndAlso r.ProductId = item.ProductId, 
                    cancellationToken)

                If record IsNot Nothing Then
                    record.VatableAmount = item.VatableAmount
                    record.VatExemptAmount = item.VatExemptAmount
                    record.ZeroRatedAmount = item.ZeroRatedAmount
                    record.OutputVat = item.OutputVat
                    record.VatTreatment = item.Treatment.ToString()
                Else
                    record = New RevenueRecord With {
                        .SourceTransactionId = notification.SaleId,
                        .ProductId = item.ProductId,
                        .VatableAmount = item.VatableAmount,
                        .VatExemptAmount = item.VatExemptAmount,
                        .ZeroRatedAmount = item.ZeroRatedAmount,
                        .OutputVat = item.OutputVat,
                        .VatTreatment = item.Treatment.ToString(),
                        .RecordDate = DateTime.UtcNow,
                        .ProductName = "Unknown"
                    }
                    _dbContext.RevenueRecords.Add(record)
                End If
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace
