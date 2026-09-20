Imports MediatR
Imports MerchSys.SharedKernel.Enums

Namespace Events

    ''' <summary>
    ''' Published when a sale is completed, containing VAT breakdowns at both the header and line-item level.
    ''' Publisher: POS-14
    ''' Consumers: ACC-10, ACC-11
    ''' Payload meaning: Carries cross-module VAT data for correct accounting and reporting in accordance with BIR regulations.
    ''' BIR Rationale: Required for generating VAT returns and recording output tax liabilities as per BIR RR No. 16-2005.
    ''' </summary>
    Public Class SaleCompletedWithVatEvent
        Implements INotification

        ''' <summary>
        ''' Gets or sets the total vatable sales amount at the header level.
        ''' </summary>
        Public Property VatableSales As Decimal

        ''' <summary>
        ''' Gets or sets the total VAT-exempt sales amount at the header level.
        ''' </summary>
        Public Property VatExemptSales As Decimal

        ''' <summary>
        ''' Gets or sets the total zero-rated sales amount at the header level.
        ''' </summary>
        Public Property ZeroRatedSales As Decimal

        ''' <summary>
        ''' Gets or sets the total output VAT amount at the header level.
        ''' </summary>
        Public Property OutputVat As Decimal

        ''' <summary>
        ''' Gets or sets a value indicating whether the buyer is VAT registered.
        ''' </summary>
        Public Property IsVatRegistered As Boolean

        ''' <summary>
        ''' Gets or sets the list of items with their respective VAT details.
        ''' </summary>
        Public Property Items As List(Of SaleItemWithVat) = New List(Of SaleItemWithVat)()

        ''' <summary>
        ''' Represents a single sale item with VAT details.
        ''' </summary>
        Public Class SaleItemWithVat

            ''' <summary>
            ''' Gets or sets the VAT treatment for this item.
            ''' </summary>
            Public Property Treatment As VatTreatment

            ''' <summary>
            ''' Gets or sets the vatable amount for this item.
            ''' </summary>
            Public Property VatableAmount As Decimal

            ''' <summary>
            ''' Gets or sets the VAT-exempt amount for this item.
            ''' </summary>
            Public Property VatExemptAmount As Decimal

            ''' <summary>
            ''' Gets or sets the zero-rated amount for this item.
            ''' </summary>
            Public Property ZeroRatedAmount As Decimal

            ''' <summary>
            ''' Gets or sets the output VAT amount for this item.
            ''' </summary>
            Public Property OutputVat As Decimal

        End Class

    End Class

End Namespace
