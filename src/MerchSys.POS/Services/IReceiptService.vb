Imports System.Threading.Tasks
Imports MerchSys.POS.Entities

Namespace Services

    ''' <summary>
    ''' Interface for managing official receipts in the POS module.
    ''' </summary>
    Public Interface IReceiptService

        ''' <summary>
        ''' Generates a new BIR-compliant official receipt for a given sales transaction.
        ''' </summary>
        Function GenerateReceiptAsync(transactionId As Integer) As Task(Of OfficialReceipt)

        ''' <summary>
        ''' Retrieves an official receipt by its ID.
        ''' </summary>
        Function GetReceiptAsync(receiptId As Integer) As Task(Of OfficialReceipt)

        ''' <summary>
        ''' Retrieves the official receipt associated with a specific transaction.
        ''' </summary>
        Function GetReceiptByTransactionAsync(transactionId As Integer) As Task(Of OfficialReceipt)

        ''' <summary>
        ''' Generates the 40-character formatted thermal print string for a receipt.
        ''' </summary>
        Function PrintReceiptAsync(receiptId As Integer) As Task(Of String)

    End Interface

End Namespace
