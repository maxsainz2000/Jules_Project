Imports System.Collections.Generic
Imports MerchSys.POS.Entities

Namespace Services.ReceiptFormatting
    Public Class ReceiptBody
        Public Property Header As New List(Of String)
        Public Property ReceiptInfo As New List(Of String)
        Public Property Items As New List(Of String)
        Public Property Totals As New List(Of String)
        Public Property Footer As New List(Of String)

        Public ReadOnly Property AllLines As IEnumerable(Of String)
            Get
                Dim lines As New List(Of String)
                lines.AddRange(Header)
                lines.AddRange(ReceiptInfo)
                lines.AddRange(Items)
                lines.AddRange(Totals)
                lines.AddRange(Footer)
                Return lines
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Join(Environment.NewLine, AllLines)
        End Function
    End Class

    Public Interface IReceiptBodyComposer
        Function Compose(receipt As OfficialReceipt, transaction As SalesTransaction, vatConfig As VatConfiguration) As ReceiptBody
    End Interface
End Namespace
