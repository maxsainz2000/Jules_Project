Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports MerchSys.Purchasing.Dtos
Imports MerchSys.Purchasing.Entities

Namespace Services

    Public Interface IGoodsReceivingService
        Function ReceiveGoodsAsync(dto As ReceiveGoodsDto) As Task(Of GoodsReceipt)
        Function GetReceiptByIdAsync(receiptId As Integer) As Task(Of GoodsReceipt)
        Function GetReceiptsForPOAsync(purchaseOrderId As Integer) As Task(Of List(Of GoodsReceipt))
    End Interface

End Namespace
