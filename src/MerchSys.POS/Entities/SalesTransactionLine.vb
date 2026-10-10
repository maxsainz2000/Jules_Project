Imports MerchSys.SharedKernel.Entities
Imports MerchSys.SharedKernel.Enums

Namespace Entities
    ''' <summary>
    ''' Represents a per-product line item in a sales transaction.
    ''' </summary>
    Public Partial Class SalesTransactionLine
        Inherits AuditableEntity

        ''' <summary>
        ''' Gets or sets the ID of the product sold.
        ''' </summary>
        Public Property ProductId As Integer

        ''' <summary>
        ''' Gets or sets the name of the product at the time of sale (denormalized).
        ''' </summary>
        Public Property ProductName As String

        ''' <summary>
        ''' Gets or sets the unit price of the product at the time of sale (denormalized).
        ''' </summary>
        Public Property UnitPrice As Decimal

        ''' <summary>
        ''' Gets or sets the quantity of the product sold.
        ''' </summary>
        Public Property Quantity As Integer

        ''' <summary>
        ''' Gets or sets the total line amount (UnitPrice * Quantity).
        ''' </summary>
        Public Property LineTotal As Decimal

        Public Property Treatment As VatTreatment
        Public Property VatableAmount As Decimal
        Public Property VatExemptAmount As Decimal
        Public Property ZeroRatedAmount As Decimal
        Public Property OutputVat As Decimal
    End Class
End Namespace
