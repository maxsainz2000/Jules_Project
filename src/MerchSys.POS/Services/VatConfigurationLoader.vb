Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.POS.Data
Imports MerchSys.POS.Entities

Namespace Services
    Public Class VatConfigurationLoader
        Private ReadOnly _scopeFactory As IServiceScopeFactory
        Private ReadOnly _semaphore As New SemaphoreSlim(1, 1)
        Private _cachedConfig As VatConfiguration

        Public Sub New(scopeFactory As IServiceScopeFactory)
            _scopeFactory = scopeFactory
        End Sub

        Public Async Function GetConfigurationAsync() As Task(Of VatConfiguration)
            If _cachedConfig IsNot Nothing Then
                Return _cachedConfig
            End If

            Await _semaphore.WaitAsync()
            Try
                If _cachedConfig IsNot Nothing Then
                    Return _cachedConfig
                End If

                Using scope = _scopeFactory.CreateScope()
                    Dim dbContext = scope.ServiceProvider.GetRequiredService(Of POSDbContext)()
                    Dim config = Await dbContext.VatConfigurations.FirstOrDefaultAsync(Function(c) c.Id = 1)

                    If config Is Nothing Then
                        config = New VatConfiguration() With {
                            .Id = 1,
                            .IsVatRegistered = False,
                            .VatRate = 0.12D,
                            .NonVatPercentageTaxRate = 0.03D,
                            .EffectiveFrom = DateTime.UtcNow,
                            .BusinessName = "Default Business",
                            .Tin = "000-000-000-000",
                            .Address = "Default Address"
                        }
                        dbContext.VatConfigurations.Add(config)
                        Await dbContext.SaveChangesAsync()
                    End If

                    _cachedConfig = config
                End Using
            Finally
                _semaphore.Release()
            End Try

            Return _cachedConfig
        End Function

        Public Sub Invalidate()
            _cachedConfig = Nothing
        End Sub
    End Class
End Namespace
