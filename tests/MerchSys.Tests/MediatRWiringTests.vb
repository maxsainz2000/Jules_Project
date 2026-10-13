Imports Xunit
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports MediatR
Imports Microsoft.Extensions.DependencyInjection
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.SharedKernel.Queries
Imports MerchSys.Purchasing.Data
Imports MerchSys.Inventory.Data
Imports MerchSys.POS.Data
Imports MerchSys.Accounting.Data

''' <summary>
''' Guards the "message with no handler" bug class.
'''
''' Two of the Critical findings were exactly this: <c>GetInventoryValuationQuery</c> was sent by
''' FinancialOverviewService with no handler, so Task.WhenAll faulted and the Financial Overview
''' screen threw; and <c>StockReturnedEvent</c> was published by SalesReturnService with no handler,
''' so sales returns silently never restocked.
'''
''' Both are invisible at compile time — MediatR resolves handlers at runtime — and both were only
''' found by reading every message type by hand. This test does that mechanically: every request and
''' every notification must have a handler registered.
'''
''' It asserts on DI *registrations*, not resolution: building the handlers would require the whole
''' application container, which is not what is under test.
'''
''' Known gaps are listed explicitly below rather than tolerated silently, and the list is checked
''' for staleness — a gap that gets fixed must be removed, so the list cannot rot.
'''
''' DB-free — it only builds a service collection and inspects it.
''' </summary>
Public Class MediatRWiringTests

    Private Shared ReadOnly HandlerAssemblies As System.Reflection.Assembly() = {
        GetType(IEventBus).Assembly,
        GetType(PurchasingDbContext).Assembly,
        GetType(InventoryDbContext).Assembly,
        GetType(POSDbContext).Assembly,
        GetType(AccountingDbContext).Assembly
    }

    ''' <summary>
    ''' Requests with no handler, each of which needs a decision rather than just a class.
    ''' </summary>
    Private Shared ReadOnly KnownRequestGaps As New Dictionary(Of String, String) From {
        {"MerchSys.SharedKernel.Queries.GetProductCatalogQuery",
         "SalesCartPresenter.OnSearchProduct (line 49) sends this, and its ContinueWith only reads " &
         "t.Result when Not t.IsFaulted — so the fault is swallowed and POS product search silently " &
         "returns nothing. Either give it a handler, or have the presenter send " &
         "GetProductsForCatalogQuery, which does have one."}
    }

    ''' <summary>
    ''' Notifications with no handler, each of which needs a decision rather than just a class.
    ''' </summary>
    Private Shared ReadOnly KnownNotificationGaps As New Dictionary(Of String, String) From {
        {"MerchSys.SharedKernel.Events.ReceiptTamperDetectedEvent",
         "ReceiptIntegrityService publishes this when a receipt hash fails to verify (line 91) and " &
         "nothing acts on it, so NIRC s.235 tamper evidence is detected but unactioned. Needs a " &
         "decision on the response: block, alert, or record."},
        {"MerchSys.SharedKernel.Events.VatConfigurationChangedEvent",
         "Published by IVatConfigurationWriter when the VAT rate changes, but nothing invalidates the " &
         "per-process VatConfigurationLoader cache, so the other client laptops keep the stale rate " &
         "until restart."}
    }

    Private Shared Function BuildServices() As ServiceCollection
        Dim services As New ServiceCollection()
        services.AddLogging()
        services.AddMediatR(Sub(cfg) cfg.RegisterServicesFromAssemblies(HandlerAssemblies))
        Return services
    End Function

    Private Shared Function ConcreteTypes() As IEnumerable(Of Type)
        Return HandlerAssemblies.
            SelectMany(Function(a) a.GetTypes()).
            Where(Function(t) t.IsClass AndAlso Not t.IsAbstract AndAlso Not t.IsGenericTypeDefinition)
    End Function

    <Fact>
    Public Sub EveryRequestHasAHandler()
        Dim services = BuildServices()
        Dim missing As New List(Of String)

        For Each requestType In ConcreteTypes().
            Where(Function(t) ImplementsOpenGeneric(t, GetType(IRequest(Of ))))

            Dim handlerType = GetType(IRequestHandler(Of ,)).
                MakeGenericType(requestType, RequestResponseType(requestType))

            If Not services.Any(Function(d) d.ServiceType = handlerType) Then
                missing.Add(requestType.FullName)
            End If
        Next

        AssertNoUnexpectedGaps(missing, KnownRequestGaps,
            "requests have no IRequestHandler registered, so mediator.Send throws at runtime")
    End Sub

    <Fact>
    Public Sub EveryNotificationHasAtLeastOneHandler()
        Dim services = BuildServices()
        Dim missing As New List(Of String)

        For Each notificationType In ConcreteTypes().
            Where(Function(t) GetType(INotification).IsAssignableFrom(t))

            Dim handlerType = GetType(INotificationHandler(Of )).MakeGenericType(notificationType)

            If Not services.Any(Function(d) d.ServiceType = handlerType) Then
                missing.Add(notificationType.FullName)
            End If
        Next

        AssertNoUnexpectedGaps(missing, KnownNotificationGaps,
            "notifications have no INotificationHandler registered, so publishing them is a silent no-op")
    End Sub

    Private Shared Sub AssertNoUnexpectedGaps(missing As List(Of String), known As Dictionary(Of String, String), description As String)
        Dim unexpected = missing.Where(Function(name) Not known.ContainsKey(name)).ToList()
        Assert.True(unexpected.Count = 0,
            "These " & description & ":" &
            Environment.NewLine & String.Join(Environment.NewLine, unexpected))

        ' A gap that has been closed must come off the list, or the list quietly becomes fiction.
        Dim stale = known.Keys.Where(Function(name) Not missing.Contains(name)).ToList()
        Assert.True(stale.Count = 0,
            "These KnownGaps entries now HAVE handlers — remove them from the list:" &
            Environment.NewLine & String.Join(Environment.NewLine, stale))
    End Sub

    Private Shared Function ImplementsOpenGeneric(candidate As Type, openGeneric As Type) As Boolean
        Return candidate.GetInterfaces().Any(
            Function(i) i.IsGenericType AndAlso i.GetGenericTypeDefinition() = openGeneric)
    End Function

    Private Shared Function RequestResponseType(requestType As Type) As Type
        Return requestType.GetInterfaces().
            First(Function(i) i.IsGenericType AndAlso i.GetGenericTypeDefinition() = GetType(IRequest(Of ))).
            GetGenericArguments()(0)
    End Function

End Class
