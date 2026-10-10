Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities

Namespace Handlers
    Public Class GoodsReceivedWithVatHandler
        Implements MediatR.INotificationHandler(Of MerchSys.SharedKernel.Events.GoodsReceivedWithVatEvent)

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(notification As MerchSys.SharedKernel.Events.GoodsReceivedWithVatEvent, cancellationToken As CancellationToken) As Task Implements MediatR.INotificationHandler(Of MerchSys.SharedKernel.Events.GoodsReceivedWithVatEvent).Handle
            For Each item In notification.Items
                Dim record As New Entities.ExpenseRecord With {
                    .SourceReferenceId = notification.PurchaseOrderId,
                    .Amount = item.TotalAmount,
                    .VatableAmount = item.VatableAmount,
                    .VatExemptAmount = item.VatExemptAmount,
                    .ZeroRatedAmount = item.ZeroRatedAmount,
                    .InputVat = item.VatAmount,
                    .VatTreatment = item.VatTreatment.ToString(),
                    .ExpenseDate = notification.ReceivedAt,
                    .SourceModule = "Purchasing",
                    .Category = "Inventory Received"
                }
                _dbContext.ExpenseRecords.Add(record)
            Next

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace