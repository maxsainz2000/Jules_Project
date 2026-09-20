Namespace Enums

    ''' <summary>
    ''' Represents the VAT treatment of a transaction or item in accordance with BIR RR No. 16-2005.
    ''' </summary>
    Public Enum VatTreatment

        ''' <summary>
        ''' Subject to the standard VAT rate.
        ''' </summary>
        Vatable = 0

        ''' <summary>
        ''' Exempt from VAT.
        ''' </summary>
        Exempt = 1

        ''' <summary>
        ''' Subject to 0% VAT rate.
        ''' </summary>
        ZeroRated = 2

    End Enum

End Namespace
