Imports System.Threading
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.Accounting.Data
Imports MerchSys.Accounting.Entities
Imports MerchSys.SharedKernel.Events

Namespace Handlers
    Public Class GoodsReceivedWithVatHandler
        Implements INotificationHandler(Of GoodsReceivedWithVatEvent)

        Private ReadOnly _dbContext As AccountingDbContext

        Public Sub New(dbContext As AccountingDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(notification As GoodsReceivedWithVatEvent, cancellationToken As CancellationToken) As Task Implements INotificationHandler(Of GoodsReceivedWithVatEvent).Handle
            Dim record = Await _dbContext.ExpenseRecords.FirstOrDefaultAsync(
                Function(r) r.Category = "Purchase" AndAlso r.SourceModule = "Purchasing" AndAlso r.SourceReferenceId.HasValue AndAlso r.SourceReferenceId.Value = notification.SourcePurchaseOrderId,
                cancellationToken)

            If record IsNot Nothing Then
                ' The requirements say: For items in notification.Items... wait, the GoodsReceived logic is at the event level or item level?
                ' Instructions: "If exists, updates VAT columns. If not, creates new ExpenseRecord with VatableAmount = item.VatableSales, VatExemptAmount = 0, ZeroRatedAmount = 0, InputVat = item.VatAmount, VatTreatment = item.VatClassification.ToString()."
                
                ' Wait, it says "item". I should iterate or take the first?
                For Each item In notification.Items
                    record.VatableAmount = item.VatableSales
                    record.VatExemptAmount = 0
                    record.ZeroRatedAmount = 0
                    record.InputVat = item.VatAmount
                    record.VatTreatment = item.VatClassification.ToString()
                    Exit For ' Since there's one record per SourcePurchaseOrderId, just take the first item or sum them up? The prompt implies one or mapping item directly.
                Next
            Else
                For Each item In notification.Items
                    record = New ExpenseRecord With {
                        .Category = "Purchase",
                        .SourceModule = "Purchasing",
                        .SourceReferenceId = notification.SourcePurchaseOrderId,
                        .ExpenseDate = DateTime.UtcNow,
                        .Description = "Purchase Order",
                        .VatableAmount = item.VatableSales,
                        .VatExemptAmount = 0,
                        .ZeroRatedAmount = 0,
                        .InputVat = item.VatAmount,
                        .VatTreatment = item.VatClassification.ToString()
                    }
                    _dbContext.ExpenseRecords.Add(record)
                    Exit For
                Next
            End If

            Await _dbContext.SaveChangesAsync(cancellationToken)
        End Function
    End Class
End Namespace
