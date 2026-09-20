Imports MediatR
Imports MerchSys.SharedKernel.Enums

Namespace Events

    ''' <summary>
    ''' Published when goods are received, containing VAT breakdowns for input tax purposes.
    ''' Publisher: Purchasing
    ''' Consumers: ACC-10, ACC-11
    ''' Payload meaning: Carries cross-module VAT data for recording input tax credits upon receipt of goods.
    ''' BIR Rationale: Required for generating VAT returns and recording input tax assets as per BIR RR No. 16-2005.
    ''' </summary>
    Public Class GoodsReceivedWithVatEvent
        Implements INotification

        ''' <summary>
        ''' Gets or sets the total vatable input amount at the header level.
        ''' </summary>
        Public Property VatableInput As Decimal

        ''' <summary>
        ''' Gets or sets the total VAT-exempt input amount at the header level.
        ''' </summary>
        Public Property VatExemptInput As Decimal

        ''' <summary>
        ''' Gets or sets the total zero-rated input amount at the header level.
        ''' </summary>
        Public Property ZeroRatedInput As Decimal

        ''' <summary>
        ''' Gets or sets the total input VAT amount at the header level.
        ''' </summary>
        Public Property InputVat As Decimal

        ''' <summary>
        ''' Gets or sets the list of received items with their respective VAT details.
        ''' </summary>
        Public Property Items As List(Of GoodsReceivedItemWithVat) = New List(Of GoodsReceivedItemWithVat)()

        ''' <summary>
        ''' Represents a single received item with VAT details.
        ''' </summary>
        Public Class GoodsReceivedItemWithVat

            ''' <summary>
            ''' Gets or sets the VAT treatment for this received item.
            ''' </summary>
            Public Property Treatment As VatTreatment

            ''' <summary>
            ''' Gets or sets the input VAT amount for this item.
            ''' </summary>
            Public Property InputVat As Decimal

        End Class

    End Class

End Namespace
