Imports System.Threading.Tasks

Namespace Services

    Public Class PaymentResultDto
        Public Property Success As Boolean
        Public Property TransactionId As Integer
        Public Property ChangeAmount As Decimal
        Public Property ReceiptNumber As String
        Public Property ErrorMessage As String
    End Class

    Public Interface IPaymentService
        Function ProcessPaymentAsync(transactionId As Integer, amountTendered As Decimal, method As String, referenceNumber As String) As Task(Of PaymentResultDto)
    End Interface

End Namespace
