Imports System.Threading
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Data
Imports MerchSys.SharedKernel.Queries

Namespace Handlers
    Public Class GetVatConfigurationQueryHandler
        Implements IRequestHandler(Of GetVatConfigurationQuery, GetVatConfigurationResult)

        Private ReadOnly _dbContext As POSDbContext

        Public Sub New(dbContext As POSDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(request As GetVatConfigurationQuery, cancellationToken As CancellationToken) As Task(Of GetVatConfigurationResult) Implements IRequestHandler(Of GetVatConfigurationQuery, GetVatConfigurationResult).Handle
            Dim config = Await _dbContext.VatConfigurations.AsNoTracking().FirstOrDefaultAsync(cancellationToken)
            
            If config Is Nothing Then
                Return New GetVatConfigurationResult With {
                    .IsVatRegistered = True,
                    .VatRate = 0.12D,
                    .NonVatPercentageTaxRate = 0.03D,
                    .BusinessName = "MerchSys Default",
                    .BusinessAddress = "Unknown",
                    .BusinessTIN = "000-000-000-000"
                }
            End If

            Return New GetVatConfigurationResult With {
                .IsVatRegistered = config.IsVatRegistered,
                .VatRate = config.VatRate,
                .NonVatPercentageTaxRate = config.NonVatPercentageTaxRate,
                .BusinessName = config.BusinessName,
                .BusinessAddress = config.Address,
                .BusinessTIN = config.Tin
            }
        End Function
    End Class
End Namespace
