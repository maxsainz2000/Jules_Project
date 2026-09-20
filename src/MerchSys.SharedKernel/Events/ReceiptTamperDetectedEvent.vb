Imports System
Imports MediatR

Namespace Events

    ''' <summary>
    ''' Published when a discrepancy is detected between the expected and actual hash of a receipt, indicating possible tampering.
    ''' Publisher: POS-13
    ''' Consumers: Accounting audit log handler
    ''' Payload meaning: Contains details of the potentially tampered receipt and the hashes involved.
    ''' </summary>
    Public Class ReceiptTamperDetectedEvent
        Implements INotification

        ''' <summary>
        ''' Gets or sets the unique identifier of the receipt.
        ''' </summary>
        Public Property ReceiptId As Guid

        ''' <summary>
        ''' Gets or sets the human-readable receipt number.
        ''' </summary>
        Public Property ReceiptNumber As String

        ''' <summary>
        ''' Gets or sets the expected hash value for the receipt.
        ''' </summary>
        Public Property ExpectedHash As String

        ''' <summary>
        ''' Gets or sets the actual calculated hash value for the receipt.
        ''' </summary>
        Public Property ActualHash As String

        ''' <summary>
        ''' Gets or sets the date and time when the tampering was detected.
        ''' </summary>
        Public Property DetectedAt As DateTime

        ''' <summary>
        ''' Gets or sets the identifier or name of the entity that detected the tampering.
        ''' </summary>
        Public Property DetectedBy As String

    End Class

End Namespace
