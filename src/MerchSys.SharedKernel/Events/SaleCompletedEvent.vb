Imports MediatR

Namespace Events

    Public Class SaleCompletedEvent
        Implements INotification

        Public Property SaleId As Integer
        Public Property SaleDate As DateTime
        Public Property PaymentMethod As MerchSys.SharedKernel.Enums.PaymentMethod
        Public Property Items As List(Of SaleCompletedItem) = New List(Of SaleCompletedItem)()

            Public Class SaleCompletedItem
                Public Property ProductId As Integer
                Public Property QuantitySold As Integer

                ''' <summary>
                ''' Price per unit at the time of sale. Accounting needs it to record revenue —
                ''' without it <c>RevenueRecord.TotalRevenue</c> was never populated, so every
                ''' revenue and gross-profit figure in FinancialOverviewService and
                ''' IncomeStatementPresenter reported zero.
                ''' </summary>
                Public Property UnitPrice As Decimal
            End Class
    End Class

End Namespace
