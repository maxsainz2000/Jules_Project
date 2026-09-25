Imports System.Windows.Forms
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Configuration
Imports MerchSys.App.Configuration
Imports MerchSys.App.Services
Imports MerchSys.App.Views
Imports MerchSys.App.Presenters
Imports MerchSys.App.Data
Imports MerchSys.SharedKernel.Interfaces

Friend Module Program

    Private _idleMonitor As IIdleMonitor
    Private _serviceProvider As IServiceProvider

    <STAThread()>
    Friend Sub Main(args As String())
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

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

                                  services.AddSingleton(Of IWriteContextScope, MerchSys.SharedKernel.Data.WriteContextScope)()
                                  services.AddScoped(Of MerchSys.SharedKernel.Data.RoleGuardInterceptor)()

                                  services.AddTransient(Of IAuthenticationService, AuthenticationService)()

                                  ' Use Scoped so that within a scope, ILoginView and LoginPresenter share the same view instance
                                  services.AddScoped(Of ILoginView, LoginView)()
                                  services.AddScoped(Of LoginPresenter)()

                                  services.AddScoped(Of MainWindow)()
                                  services.AddScoped(Of MainWindowPresenter)()

                                  services.AddScoped(Of IOwnerDashboardView, OwnerDashboardView)()
                                  services.AddScoped(Of OwnerDashboardPresenter)()
                              End Sub).
            Build()

        _serviceProvider = host.Services

        Dim configService = host.Services.GetRequiredService(Of IConfiguration)()
        Dim connString = configService.GetSection("Sync")("MariaDbConnection")
        If Not String.IsNullOrWhiteSpace(connString) Then
            Try
                Dim logger = host.Services.GetService(Of Microsoft.Extensions.Logging.ILogger(Of MariaDbSchemaInitializer))()
                MariaDbSchemaInitializer.Initialize(connString, logger)
            Catch ex As Exception
                MessageBox.Show("Failed to initialize central database schema:" & Environment.NewLine & ex.Message, "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Environment.Exit(1)
            End Try
        End If

        _idleMonitor = host.Services.GetRequiredService(Of IIdleMonitor)()
        AddHandler _idleMonitor.IdleWarning, AddressOf HandleIdleWarning
        AddHandler _idleMonitor.SessionExpired, AddressOf HandleSessionExpired

        RunApplicationFlow()

        If _idleMonitor IsNot Nothing Then
            _idleMonitor.Stop()
        End If
    End Sub

    Private _forceLogoutRequested As Boolean = False
    Private _mainWindowForm As Form

    Private Sub RunApplicationFlow()
        Dim continueRunning As Boolean = True

        While continueRunning
            _forceLogoutRequested = False

            ' Create a scope for the login flow
            Dim loggedIn As Boolean = False
            Using loginScope = _serviceProvider.CreateScope()
                Dim loginView = loginScope.ServiceProvider.GetRequiredService(Of ILoginView)()
                Dim loginPresenter = loginScope.ServiceProvider.GetRequiredService(Of LoginPresenter)()

                If DirectCast(loginView, Form).ShowDialog() = DialogResult.OK Then
                    loggedIn = True
                End If
            End Using

            If loggedIn Then
                _idleMonitor.Start()

                ' Create a scope for the main window flow
                Using mainScope = _serviceProvider.CreateScope()
                    Dim mainWindow = mainScope.ServiceProvider.GetRequiredService(Of MainWindow)()
                    _mainWindowForm = mainWindow
                    Dim mainPresenter = mainScope.ServiceProvider.GetRequiredService(Of MainWindowPresenter)()
                    Application.Run(mainWindow)
                End Using

                _idleMonitor.Stop()
                _mainWindowForm = Nothing

                If _forceLogoutRequested Then
                    continueRunning = True
                Else
                    Dim sessionService = _serviceProvider.GetRequiredService(Of LoginSessionService)()
                    If sessionService.IsAuthenticated Then
                        ' Main window closed but user wasn't logged out explicitly via flow, just exit
                        continueRunning = False
                    Else
                        ' Logged out properly
                        continueRunning = True
                    End If
                End If
            Else
                continueRunning = False
            End If
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

        _forceLogoutRequested = True
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
