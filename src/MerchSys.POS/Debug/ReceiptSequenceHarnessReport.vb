#If DEBUG Then
Imports System
Imports System.Collections.Concurrent
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging
Imports MerchSys.POS.Services
Imports MerchSys.POS.Data
Imports Microsoft.EntityFrameworkCore

Namespace Debug
    Public Class ReceiptSequenceHarnessReport

        Public Shared Async Function RunAndReportAsync(totalReservations As Integer) As Task(Of String)
            Dim services As New ServiceCollection()
            services.AddLogging()

            Dim builder As New DbContextOptionsBuilder(Of POSDbContext)()
            builder.UseInMemoryDatabase("HarnessDb")
            services.AddScoped(Of POSDbContext)(Function(sp) New POSDbContext(builder.Options))

            services.AddScoped(Of IReceiptIntegrityService, ReceiptIntegrityService)()
            services.AddScoped(Of MediatR.IMediator, DummyMediator)()

            Dim serviceProvider = services.BuildServiceProvider()

            Dim reportPath = Path.Combine(Path.GetTempPath(), "ReceiptSequenceHarnessReport.md")
            Dim sb As New StringBuilder()

            sb.AppendLine("# Receipt Sequence Concurrency Harness Report")
            sb.AppendLine($"**Run Date:** {DateTime.Now}")
            sb.AppendLine($"**Total Requested:** {totalReservations}")
            sb.AppendLine()

            Dim currentYear = DateTime.UtcNow.Year
            Dim generatedNumbers As New ConcurrentBag(Of String)()
            Dim exceptions As New ConcurrentBag(Of Exception)()
            Dim startTicks = Environment.TickCount

            ' Run concurrent tasks
            Dim tasks = Enumerable.Range(0, totalReservations).Select(
                Async Function(i)
                    Using scope = serviceProvider.CreateScope()
                        Try
                            Dim receiptIntegrity = scope.ServiceProvider.GetRequiredService(Of IReceiptIntegrityService)()
                            Dim nextNumber = Await receiptIntegrity.GetNextReceiptNumberAsync(currentYear)
                            generatedNumbers.Add(nextNumber)
                        Catch ex As Exception
                            exceptions.Add(ex)
                        End Try
                    End Using
                End Function).ToArray()

            Await Task.WhenAll(tasks)
            Dim elapsedMs = Environment.TickCount - startTicks

            Dim numbers = generatedNumbers.ToList()
            Dim parsedNumbers = numbers.Select(Function(n)
                                                   Dim parts = n.Split("-"c)
                                                   Dim numVal As Integer = 0
                                                   If parts.Length = 3 AndAlso Integer.TryParse(parts(2), numVal) Then
                                                       Return numVal
                                                   End If
                                                   Return -1
                                               End Function).
                                               Where(Function(v) v >= 0).
                                               OrderBy(Function(v) v).ToList()

            Dim duplicates = numbers.Count - numbers.Distinct().Count()
            Dim gaps = 0

            If parsedNumbers.Count > 0 Then
                Dim min = parsedNumbers.First()
                Dim max = parsedNumbers.Last()
                Dim expectedCount = max - min + 1
                gaps = expectedCount - parsedNumbers.Distinct().Count()
            End If

            sb.AppendLine("## Results")
            sb.AppendLine($"- **Elapsed Time:** {elapsedMs} ms")
            sb.AppendLine($"- **Successful Reservations:** {numbers.Count}")
            sb.AppendLine($"- **Failed Requests:** {exceptions.Count}")
            sb.AppendLine($"- **Duplicates:** {duplicates}")
            sb.AppendLine($"- **Gaps Detected:** {gaps}")
            sb.AppendLine()

            If exceptions.Count > 0 Then
                sb.AppendLine("## Exceptions")
                Dim sampleExceptions = exceptions.Take(5).ToList()
                For Each ex In sampleExceptions
                    sb.AppendLine($"```")
                    sb.AppendLine(ex.Message)
                    sb.AppendLine($"```")
                Next
                If exceptions.Count > 5 Then
                    sb.AppendLine($"*(...and {exceptions.Count - 5} more exceptions)*")
                End If
            End If

            File.WriteAllText(reportPath, sb.ToString())
            Return reportPath
        End Function

        Public Shared Async Function RunAndReportAsync() As Task(Of String)
            Return Await RunAndReportAsync(800)
        End Function

        Private Class DummyMediator
            Implements MediatR.IMediator
            Public Function Send(Of TResponse)(request As MediatR.IRequest(Of TResponse), Optional cancellationToken As Threading.CancellationToken = Nothing) As Task(Of TResponse) Implements MediatR.IMediator.Send
                Return Task.FromResult(CType(Nothing, TResponse))
            End Function
            Public Function Send(Of TRequest As MediatR.IRequest)(request As TRequest, Optional cancellationToken As Threading.CancellationToken = Nothing) As Task Implements MediatR.IMediator.Send
                Return Task.CompletedTask
            End Function
            Public Function Send(request As Object, Optional cancellationToken As Threading.CancellationToken = Nothing) As Task(Of Object) Implements MediatR.IMediator.Send
                Return Task.FromResult(CType(Nothing, Object))
            End Function
            Public Async Function Publish(notification As Object, Optional cancellationToken As Threading.CancellationToken = Nothing) As Task Implements MediatR.IPublisher.Publish
                Await Task.CompletedTask
            End Function
            Public Async Function Publish(Of TNotification As MediatR.INotification)(notification As TNotification, Optional cancellationToken As Threading.CancellationToken = Nothing) As Task Implements MediatR.IPublisher.Publish
                Await Task.CompletedTask
            End Function
            Public Function CreateStream(Of TResponse)(request As MediatR.IStreamRequest(Of TResponse), Optional cancellationToken As Threading.CancellationToken = Nothing) As Collections.Generic.IAsyncEnumerable(Of TResponse) Implements MediatR.ISender.CreateStream
                Throw New NotImplementedException()
            End Function
            Public Function CreateStream(request As Object, Optional cancellationToken As Threading.CancellationToken = Nothing) As Collections.Generic.IAsyncEnumerable(Of Object) Implements MediatR.ISender.CreateStream
                Throw New NotImplementedException()
            End Function
        End Class

    End Class
End Namespace
#End If
