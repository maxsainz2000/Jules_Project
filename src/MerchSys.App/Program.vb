Imports MerchSys.Inventory.Startup
Imports System.Windows.Forms
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging
Imports MerchSys.App.Configuration
Imports MerchSys.App.Services
Imports MerchSys.App.Views
Imports MerchSys.App.Presenters
Imports MerchSys.App.Views.Shell
Imports MerchSys.App.Data
Imports MerchSys.SharedKernel.Interfaces
Imports MerchSys.App.Startup
Imports MerchSys.App.Views.Shell.Modules
Imports MerchSys.Purchasing.Startup
Imports MerchSys.POS.Extensions
Imports MerchSys.Accounting.Extensions

Friend Module Program

    Private _idleMonitor As IIdleMonitor
    Private _serviceProvider As IServiceProvider

    <STAThread()>
    Friend Sub Main(args As String())
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        ' Resolve and validate the production connection string BEFORE the host is built.
        ' ConfigureServices runs inside Build(), and AddModuleDbContexts rejects an empty
        ' connection string — so validating only afterwards would surface a raw
        ' ArgumentException instead of the operator dialog (C4/M15).
        Dim bootstrapConfig = New ConfigurationBuilder().
            SetBasePath(AppContext.BaseDirectory).
            AddJsonFile("appsettings.json", optional:=True, reloadOnChange:=False).
            AddEnvironmentVariables().
            AddCommandLine(args).
            AddProductionOverlay().
            Build()

        Dim connString = ConnectionStringLoader.GetMariaDbConnectionString(bootstrapConfig, Nothing)
        Dim configError = StartupConfigValidator.ValidateConnectionString(connString)
        If configError IsNot Nothing Then
            MessageBox.Show(configError, "Production Configuration Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Environment.Exit(1)
        End If

        Dim host As IHost = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args).
            ConfigureAppConfiguration(Sub(context, builder)
                                          builder.AddProductionOverlay()
                                      End Sub).
            ConfigureServices(Sub(context, services)
                                  Dim config = context.Configuration
                                  services.Configure(Of IdleMonitorOptions)(config.GetSection("Session"))

#If DEBUG Then
                                  If Environment.GetEnvironmentVariable("VISTA_DISABLE_IDLE_TIMEOUT") = "1" Then
                                      services.AddSingleton(Of IIdleMonitor, NoOpIdleMonitor)()
                                  Else
                                      services.AddSingleton(Of IIdleMonitor, WinFormsIdleMonitor)()
                                  End If
#Else
                                  services.AddSingleton(Of IIdleMonitor, WinFormsIdleMonitor)()
#End If

                                  ' Use Scoped so they share the same instance in the warning dialog scope
                                  services.AddScoped(Of ISessionTimeoutWarningView, SessionTimeoutWarningView)()
                                  services.AddScoped(Of SessionTimeoutWarningPresenter)()

                                  services.AddSingleton(Of LoginSessionService)()
                                  services.AddSingleton(Of ISessionService)(Function(sp) sp.GetRequiredService(Of LoginSessionService)())
                                  ' Supplies the audit identity to the data layer (H4).
                                  services.AddSingleton(Of ICurrentUserProvider, SessionCurrentUserProvider)()

                                  services.AddSingleton(Of IWriteContextScope, MerchSys.SharedKernel.Data.WriteContextScope)()
                                  services.AddScoped(Of MerchSys.SharedKernel.Data.RoleGuardInterceptor)()

                                  services.AddTransient(Of IAuthenticationService, AuthenticationService)()

                                  ' Use Scoped so that within a scope, ILoginView and LoginPresenter share the same view instance
                                  services.AddScoped(Of ILoginView, LoginView)()
                                  services.AddScoped(Of LoginPresenter)()
                                  
                                  ' Shell infrastructure — scoped per login session so a re-login
                                  ' after logout or an idle timeout gets a fresh, undisposed shell.
                                  services.AddScoped(Of MainWindow)()
                                  services.AddScoped(Of IMainWindowView)(Function(sp) sp.GetRequiredService(Of MainWindow)())
                                  services.AddScoped(Of MainWindowPresenter)()

                                  services.AddScoped(Of IOwnerDashboardView, OwnerDashboardView)()
                                  services.AddScoped(Of OwnerDashboardPresenter)()

                                  services.AddConnectionHealthMonitor()

                                  ' Add database contexts — one resolved, pooling-enabled connection
                                  ' string is used for EF contexts AND the raw-ADO services, so the
                                  ' whole application always targets the same database.
                                  ' One resolved value is shared by the EF contexts and the
                                  ' raw-ADO services, so they always target the same database.
                                  services.AddModuleDbContexts(connString)

                                  ' Add MediatR
                                  services.AddMediatRServices()

                                  ' Add Purchasing
                                  services.AddPurchasingServices()
                                  services.AddInventoryServices()
                                  services.AddPosModule()
                                  services.AddAccountingServices()

                                  ' Add Low Stock Notifier
                                  services.AddSingleton(Of ILowStockNotifier, WinFormsLowStockNotifier)()

                                  ' Register UI Panels
                                  services.AddTransient(Of PurchasingPanel)()
                                  services.AddTransient(Of InventoryPanel)()
                                  services.AddTransient(Of PosPanel)()
                                  services.AddTransient(Of AccountingPanel)()
                                  services.AddTransient(Of DeveloperToolsPanel)()

                                  services.AddSingleton(Of MerchSys.App.Services.Theming.IThemeService, MerchSys.App.Services.Theming.ThemeService)()

                                  services.AddSingleton(Of MerchSys.SharedKernel.Interfaces.INotificationService, MerchSys.App.Services.DefaultNotificationService)()
                                  services.AddTransient(Of MerchSys.App.Views.POS.VatSettingsView)()
                                  services.AddTransient(Of MerchSys.POS.Views.IVatSettingsView)(Function(sp) sp.GetRequiredService(Of MerchSys.App.Views.POS.VatSettingsView)())
                                  services.AddTransient(Of MerchSys.App.Views.Accounting.FinancialOverviewView)()
                                  services.AddTransient(Of MerchSys.Accounting.Views.IFinancialOverviewView)(Function(sp) sp.GetRequiredService(Of MerchSys.App.Views.Accounting.FinancialOverviewView)())
                                  services.AddTransient(Of MerchSys.App.Views.Accounting.IncomeStatementView)()
                                  services.AddTransient(Of MerchSys.Accounting.Views.IIncomeStatementView)(Function(sp) sp.GetRequiredService(Of MerchSys.App.Views.Accounting.IncomeStatementView)())
                                  services.AddTransient(Of MerchSys.App.Views.Accounting.SalesSummaryView)()
                                  services.AddTransient(Of MerchSys.Accounting.Views.ISalesSummaryView)(Function(sp) sp.GetRequiredService(Of MerchSys.App.Views.Accounting.SalesSummaryView)())
                                  services.AddTransient(Of MerchSys.App.Views.Accounting.VatReturnView)()
                                  services.AddTransient(Of MerchSys.Accounting.Views.IVatReturnView)(Function(sp) sp.GetRequiredService(Of MerchSys.App.Views.Accounting.VatReturnView)())
                                  services.AddTransient(Of MerchSys.App.Views.Accounting.VatReliefReportView)()
                                  services.AddTransient(Of MerchSys.Accounting.Views.IVatReliefReportView)(Function(sp) sp.GetRequiredService(Of MerchSys.App.Views.Accounting.VatReliefReportView)())
                                  services.AddTransient(Of MerchSys.App.Views.Accounting.TamperAuditReportView)()
                                  services.AddTransient(Of MerchSys.Accounting.Views.ITamperAuditReportView)(Function(sp) sp.GetRequiredService(Of MerchSys.App.Views.Accounting.TamperAuditReportView)())

                              End Sub).
            Build()

        _serviceProvider = host.Services

        Dim loggerFactory = host.Services.GetRequiredService(Of Microsoft.Extensions.Logging.ILoggerFactory)()
        Dim startupLogger = loggerFactory.CreateLogger(Of MariaDbSchemaInitializer)()

        Try
            ' Single schema-bootstrap entry point: central versioned SQL + EF-model module schemas.
            DatabaseBootstrapper.Initialize(connString, _serviceProvider, startupLogger)
        Catch ex As Exception
            MessageBox.Show("Failed to initialize central database schema or module databases:" & Environment.NewLine & ex.Message, "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Environment.Exit(1)
        End Try

        Dim healthMonitor = _serviceProvider.GetRequiredService(Of IConnectionHealthMonitor)()
        ConnectionHealthMonitorLocator.Current = healthMonitor
        healthMonitor.Start()

        _idleMonitor = host.Services.GetRequiredService(Of IIdleMonitor)()
        AddHandler _idleMonitor.IdleWarning, AddressOf HandleIdleWarning
        AddHandler _idleMonitor.SessionExpired, AddressOf HandleSessionExpired

        Dim themeService = _serviceProvider.GetRequiredService(Of MerchSys.App.Services.Theming.IThemeService)()

        RunApplicationFlow()

        ' Dispose the long-lived monitors so their timers are released at shutdown (L6).
        If _idleMonitor IsNot Nothing Then
            _idleMonitor.Stop()
            Dim disposableIdle = TryCast(_idleMonitor, IDisposable)
            If disposableIdle IsNot Nothing Then disposableIdle.Dispose()
        End If

        If ConnectionHealthMonitorLocator.Current IsNot Nothing Then
            ConnectionHealthMonitorLocator.Current.Stop()
            Dim disposableMonitor = TryCast(ConnectionHealthMonitorLocator.Current, IDisposable)
            If disposableMonitor IsNot Nothing Then disposableMonitor.Dispose()
        End If
    End Sub

    Private _mainWindowForm As Form

    Private Sub RunApplicationFlow()
        Dim continueRunning As Boolean = True

        While continueRunning
            ' Create a scope for the login flow
            Dim loggedIn As Boolean = False
            Using loginScope = _serviceProvider.CreateScope()
                Dim loginView = loginScope.ServiceProvider.GetRequiredService(Of ILoginView)()
                Dim loginPresenter = loginScope.ServiceProvider.GetRequiredService(Of LoginPresenter)()

                If DirectCast(loginView, Form).ShowDialog() = DialogResult.OK Then
                    loggedIn = True
                End If
            End Using

            If Not loggedIn Then
                continueRunning = False
                Continue While
            End If

            _idleMonitor.Start()

            ' A fresh shell is created for every login session. Previously MainWindow and
            ' its presenter were singletons, so re-login after an idle timeout reused a
            ' window that Close() had already disposed (H7).
            Dim sessionScope = _serviceProvider.CreateScope()
            Try
                Dim mainWindow = sessionScope.ServiceProvider.GetRequiredService(Of MainWindow)()
                _mainWindowForm = mainWindow
                sessionScope.ServiceProvider.GetRequiredService(Of MainWindowPresenter)()

                Application.Run(mainWindow)
            Finally
                _mainWindowForm = Nothing
                _idleMonitor.Stop()
                sessionScope.Dispose()
            End Try

            ' Logout or idle timeout clears the session -> loop back to the login screen.
            ' The user closing the window while still signed in exits the application.
            Dim sessionService = _serviceProvider.GetRequiredService(Of LoginSessionService)()
            continueRunning = Not sessionService.IsAuthenticated
        End While
    End Sub

    Private _warningScope As IServiceScope
    Private _warningDialog As ISessionTimeoutWarningView
    Private _warningPresenter As SessionTimeoutWarningPresenter

    Private Sub HandleIdleWarning(sender As Object, e As IdleMonitorWarningEventArgs)
        If _warningScope Is Nothing Then
            _warningScope = _serviceProvider.CreateScope()
            _warningDialog = _warningScope.ServiceProvider.GetRequiredService(Of ISessionTimeoutWarningView)()
            _warningPresenter = _warningScope.ServiceProvider.GetRequiredService(Of SessionTimeoutWarningPresenter)()

            AddHandler _warningPresenter.StaySignedInRequested, AddressOf OnStaySignedIn
            AddHandler _warningPresenter.SignOutRequested, AddressOf OnSignOut

            ' Delegate to presenter
            _warningPresenter.Tick(e.RemainingSeconds)
            _warningPresenter.Show(_mainWindowForm)
        Else
            _warningPresenter.Tick(e.RemainingSeconds)
        End If
    End Sub

    Private Sub OnStaySignedIn(sender As Object, e As EventArgs)
        _idleMonitor.Reset()
        CleanupWarningDialog()
    End Sub

    Private Sub OnSignOut(sender As Object, e As EventArgs)
        CleanupWarningDialog()
        ForceLogout()
    End Sub

    Private Sub HandleSessionExpired(sender As Object, e As EventArgs)
        CleanupWarningDialog()
        ForceLogout()
    End Sub

    Private Sub CleanupWarningDialog()
        If _warningScope IsNot Nothing Then
            If _warningPresenter IsNot Nothing Then
                RemoveHandler _warningPresenter.StaySignedInRequested, AddressOf OnStaySignedIn
                RemoveHandler _warningPresenter.SignOutRequested, AddressOf OnSignOut
            End If

            If _warningDialog IsNot Nothing Then
                Dim frm = TryCast(_warningDialog, Form)
                If frm IsNot Nothing AndAlso Not frm.IsDisposed AndAlso frm.Visible Then
                    frm.Hide()
                End If
            End If
            _warningScope.Dispose()
            _warningScope = Nothing
            _warningDialog = Nothing
            _warningPresenter = Nothing
        End If
    End Sub

    Private Sub ForceLogout()
        Dim sessionService = _serviceProvider.GetRequiredService(Of LoginSessionService)()
        sessionService.ClearUser()

        If _mainWindowForm IsNot Nothing Then
            ' Safely close the main window on the UI thread
            If _mainWindowForm.InvokeRequired Then
                _mainWindowForm.Invoke(Sub() _mainWindowForm.Close())
            Else
                _mainWindowForm.Close()
            End If
        End If
    End Sub

End Module
