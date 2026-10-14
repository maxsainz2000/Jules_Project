Imports MerchSys.SharedKernel.Entities

Namespace Entities
    Public Class FinancialPeriod
        Inherits AuditableEntity
        Implements MerchSys.SharedKernel.Interfaces.IAuditedEntity

        Public Property PeriodType As String
        Public Property StartDate As DateTime
        Public Property EndDate As DateTime
        Public Property PeriodLabel As String
        Public Property TotalRevenue As Decimal
        Public Property TotalCogs As Decimal
        Public Property GrossProfit As Decimal
        Public Property GrossMarginPercent As Decimal
        Public Property OperatingExpenses As Decimal
        Public Property NetIncome As Decimal
        Public Property RowVersion As DateTime
    End Class
End Namespace
