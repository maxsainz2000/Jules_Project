Imports System.Windows.Forms
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting

Friend Module Program

    <STAThread()>
    Friend Sub Main(args As String())
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim host As IHost = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args).
            ConfigureServices(Sub(context, services)
                                  services.AddTransient(Of MainWindow)()
                              End Sub).
            Build()

        Dim mainWindow = host.Services.GetRequiredService(Of MainWindow)()
        Application.Run(mainWindow)
    End Sub

End Module
