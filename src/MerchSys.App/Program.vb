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
                                  services.AddSingleton(Of LoginSessionService)()
                                  services.AddSingleton(Of ISessionService)(Function(sp) sp.GetRequiredService(Of LoginSessionService)())
                                  services.AddTransient(Of IAuthenticationService, AuthenticationService)()

                                  ' Use Scoped so that within a scope, ILoginView and LoginPresenter share the same view instance
                                  services.AddScoped(Of ILoginView, LoginView)()
                                  services.AddScoped(Of LoginPresenter)()

                                  services.AddScoped(Of MainWindow)()
                                  services.AddScoped(Of MainWindowPresenter)()
                              End Sub).
            Build()

        Dim config = host.Services.GetRequiredService(Of IConfiguration)()
        Dim connString = config.GetSection("Sync")("MariaDbConnection")
        If Not String.IsNullOrWhiteSpace(connString) Then
            DatabaseInitializer.Initialize(connString)
        End If

        ' Create a scope for the login flow
        Dim loggedIn As Boolean = False
        Using loginScope = host.Services.CreateScope()
            Dim loginView = loginScope.ServiceProvider.GetRequiredService(Of ILoginView)()
            Dim loginPresenter = loginScope.ServiceProvider.GetRequiredService(Of LoginPresenter)()

            If DirectCast(loginView, Form).ShowDialog() = DialogResult.OK Then
                loggedIn = True
            End If
        End Using

        If loggedIn Then
            ' Create a scope for the main window flow
            Using mainScope = host.Services.CreateScope()
                Dim mainWindow = mainScope.ServiceProvider.GetRequiredService(Of MainWindow)()
                Dim mainPresenter = mainScope.ServiceProvider.GetRequiredService(Of MainWindowPresenter)()
                Application.Run(mainWindow)
            End Using
        Else
            Application.Exit()
        End If
    End Sub

End Module
