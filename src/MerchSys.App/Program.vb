Imports System.Windows.Forms
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports MerchSys.App.Configuration
Imports MerchSys.App.Presenters
Imports MerchSys.App.Services

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
                                  services.AddTransient(Of Data.DatabaseInitializer)()
                                  services.AddSingleton(Of ISessionService, LoginSessionService)()
                                  services.AddTransient(Of IAuthenticationService, AuthenticationService)()
                                  services.AddTransient(Of LoginPresenter)()
                                  services.AddTransient(Of LoginView)()
                                  services.AddTransient(Of MainWindow)()
                                  services.AddTransient(Of MainWindowPresenter)()
                              End Sub).
            Build()

        ' Initialize DB
        Dim dbInitializer = host.Services.GetRequiredService(Of Data.DatabaseInitializer)()
        dbInitializer.InitializeAsync().GetAwaiter().GetResult()

        Dim loginView = host.Services.GetRequiredService(Of LoginView)()
        Dim loginPresenter = host.Services.GetRequiredService(Of LoginPresenter)()
        loginView.SetPresenter(loginPresenter)

        Dim dialogResult = loginView.ShowDialog()

        If dialogResult = DialogResult.OK Then
            Dim mainWindow = host.Services.GetRequiredService(Of MainWindow)()
            Dim mainWindowPresenter = host.Services.GetRequiredService(Of MainWindowPresenter)()
            mainWindow.SetPresenter(mainWindowPresenter)
            Application.Run(mainWindow)
        Else
            Application.Exit()
        End If
    End Sub

End Module
