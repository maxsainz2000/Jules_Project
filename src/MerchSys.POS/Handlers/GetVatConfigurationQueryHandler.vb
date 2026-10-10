Imports System.Threading
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Data

Namespace Handlers
    Public Class GetVatConfigurationQueryHandler
        Implements MediatR.IRequestHandler(Of MerchSys.SharedKernel.Queries.GetVatConfigurationQuery, MerchSys.SharedKernel.Queries.GetVatConfigurationResult)

        Private ReadOnly _dbContext As POSDbContext

        Public Sub New(dbContext As POSDbContext)
            _dbContext = dbContext
        End Sub

        Public Async Function Handle(request As MerchSys.SharedKernel.Queries.GetVatConfigurationQuery, cancellationToken As CancellationToken) As Task(Of MerchSys.SharedKernel.Queries.GetVatConfigurationResult) Implements MediatR.IRequestHandler(Of MerchSys.SharedKernel.Queries.GetVatConfigurationQuery, MerchSys.SharedKernel.Queries.GetVatConfigurationResult).Handle
            Dim config = Await _dbContext.VatConfigurations.AsNoTracking().FirstOrDefaultAsync(cancellationToken)

            If config Is Nothing Then
                Return New MerchSys.SharedKernel.Queries.GetVatConfigurationResult With {
                    .IsVatRegistered = True,
                    .VatRate = 0.12D,
                    .NonVatPercentageTaxRate = 0.01D,
                    .BusinessName = "Default Business",
                    .BusinessAddress = "Default Address",
                    .BusinessTIN = "000-000-000-000"
                }
            End If

            Return New MerchSys.SharedKernel.Queries.GetVatConfigurationResult With {
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