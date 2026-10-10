Imports System.Threading
Imports System.Threading.Tasks
Imports MediatR
Imports Microsoft.EntityFrameworkCore
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities
Imports MerchSys.SharedKernel.Events

Namespace Services
    Public Class VatConfigurationUpdateRequest
        Public Property IsVatRegistered As Boolean
        Public Property VatRate As Decimal
        Public Property NonVatPercentageTaxRate As Decimal
        Public Property BusinessName As String
        Public Property Address As String
        Public Property Tin As String
    End Class

    Public Class VatConfigurationUpdateResult
        Public Property Success As Boolean
        Public Property ErrorMessage As String
    End Class

    Public Interface IVatConfigurationWriter
        Function UpdateConfigurationAsync(request As VatConfigurationUpdateRequest, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatConfigurationUpdateResult)
    End Interface

    Public Class VatConfigurationWriter
        Implements IVatConfigurationWriter

        Private ReadOnly _dbContext As POSDbContext
        Private ReadOnly _configLoader As VatConfigurationLoader
        Private ReadOnly _mediator As IMediator

        Public Sub New(dbContext As POSDbContext, configLoader As VatConfigurationLoader, mediator As IMediator)
            _dbContext = dbContext
            _configLoader = configLoader
            _mediator = mediator
        End Sub

        Public Async Function UpdateConfigurationAsync(request As VatConfigurationUpdateRequest, Optional cancellationToken As CancellationToken = Nothing) As Task(Of VatConfigurationUpdateResult) Implements IVatConfigurationWriter.UpdateConfigurationAsync
            Dim currentConfig = Await _dbContext.VatConfigurations.FirstOrDefaultAsync(Function(c) c.Id = 1, cancellationToken)

            If currentConfig Is Nothing Then
                currentConfig = New VatConfiguration() With {
                    .Id = 1
                }
                _dbContext.VatConfigurations.Add(currentConfig)
            End If

            Dim previousIsVatRegistered As Boolean = currentConfig.IsVatRegistered

            currentConfig.IsVatRegistered = request.IsVatRegistered
            currentConfig.VatRate = request.VatRate
            currentConfig.NonVatPercentageTaxRate = request.NonVatPercentageTaxRate
            currentConfig.BusinessName = request.BusinessName
            currentConfig.Address = request.Address
            currentConfig.Tin = request.Tin
            currentConfig.EffectiveFrom = DateTime.UtcNow

            Await _dbContext.SaveChangesAsync(cancellationToken)

            _configLoader.Invalidate()

            Await _mediator.Publish(New VatConfigurationChangedEvent(
                currentConfig.IsVatRegistered,
                previousIsVatRegistered,
                currentConfig.VatRate), cancellationToken)

            Return New VatConfigurationUpdateResult With { .Success = True }
        End Function
    End Class
End Namespace
