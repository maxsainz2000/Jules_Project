Imports System
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging
Imports MySqlConnector

Namespace Services
    Public Class ConnectionHealthMonitor
        Implements IConnectionHealthMonitor
        Implements IDisposable

        Private ReadOnly _logger As ILogger(Of ConnectionHealthMonitor)
        Private ReadOnly _configuration As IConfiguration
        Private ReadOnly _timer As Timer

        Private _currentState As ConnectionState = ConnectionState.Offline
        Private _retryCount As Integer = 0
        Private _isChecking As Boolean = False

        Private ReadOnly _healthCheckIntervalSeconds As Integer
        Private ReadOnly _retryBackoffSeconds As Integer()
        Private ReadOnly _maxRetries As Integer
        Private ReadOnly _connectionString As String

        Public Event StateChanged As EventHandler(Of ConnectionStateChangedEventArgs) Implements IConnectionHealthMonitor.StateChanged

        Public ReadOnly Property CurrentState As ConnectionState Implements IConnectionHealthMonitor.CurrentState
            Get
                Return _currentState
            End Get
        End Property

        Public Sub New(logger As ILogger(Of ConnectionHealthMonitor), configuration As IConfiguration)
            _logger = logger
            _configuration = configuration

            ' Load configuration with defaults
            _healthCheckIntervalSeconds = _configuration.GetValue(Of Integer)("Connection:HealthCheckIntervalSeconds", 15)

            Dim backoffConfig = _configuration.GetSection("Connection:RetryBackoffSeconds").Get(Of Integer())()
            If backoffConfig IsNot Nothing AndAlso backoffConfig.Length > 0 Then
                _retryBackoffSeconds = backoffConfig
            Else
                _retryBackoffSeconds = New Integer() {2, 4, 8, 16, 32}
            End If

            _maxRetries = _configuration.GetValue(Of Integer)("Connection:MaxRetries", 5)

            _connectionString = MerchSys.App.Configuration.ConnectionStringLoader.GetMariaDbConnectionString(_configuration, _logger)

            _timer = New Timer()
            _timer.Interval = _healthCheckIntervalSeconds * 1000
            AddHandler _timer.Tick, AddressOf OnTimerTick
        End Sub

        Public Sub Start() Implements IConnectionHealthMonitor.Start
            _logger.LogInformation("Starting ConnectionHealthMonitor.")
            ChangeState(ConnectionState.Reconnecting)
            _timer.Start()

            ' Run first check immediately (but don't block Start method)
            Task.Run(Function() PerformCheckAsync())
        End Sub

        Public Sub [Stop]() Implements IConnectionHealthMonitor.Stop
            _logger.LogInformation("Stopping ConnectionHealthMonitor.")
            _timer.Stop()
            ChangeState(ConnectionState.Offline)
        End Sub

        Public Async Function RetryNowAsync() As Task Implements IConnectionHealthMonitor.RetryNowAsync
            _logger.LogInformation("Manual retry requested.")
            If Not _timer.Enabled Then
                _timer.Start()
            End If

            _retryCount = 0
            ChangeState(ConnectionState.Reconnecting)
            Await PerformCheckAsync()
        End Function

        Private Async Sub OnTimerTick(sender As Object, e As EventArgs)
            Await PerformCheckAsync()
        End Sub

        Private Async Function PerformCheckAsync() As Task
            If _isChecking Then Return
            _isChecking = True

            Try
                Dim isConnected = Await CheckConnectionAsync()

                If isConnected Then
                    If _currentState <> ConnectionState.Online Then
                        _logger.LogInformation("Connection is online.")
                        _retryCount = 0

                        ' Reset timer interval to standard check interval
                        _timer.Interval = _healthCheckIntervalSeconds * 1000
                        ChangeState(ConnectionState.Online)
                    End If
                Else
                    HandleOfflineState()
                End If
            Catch ex As Exception
                _logger.LogError(ex, "Error during connection health check.")
                HandleOfflineState()
            Finally
                _isChecking = False
            End Try
        End Function

        Private Async Function CheckConnectionAsync() As Task(Of Boolean)
            If String.IsNullOrWhiteSpace(_connectionString) Then Return False

            Try
                Using conn = New MySqlConnection(_connectionString)
                    Await conn.OpenAsync()
                    Using cmd = conn.CreateCommand()
                        cmd.CommandText = "SELECT 1"
                        Await cmd.ExecuteScalarAsync()
                        Return True
                    End Using
                End Using
            Catch
                Return False
            End Try
        End Function

        Private Sub HandleOfflineState()
            If _currentState = ConnectionState.Online OrElse _currentState = ConnectionState.Reconnecting Then
                _retryCount += 1

                If _retryCount > _maxRetries Then
                    _logger.LogWarning($"Connection max retries ({_maxRetries}) reached. Transitioning to Offline.")
                    _timer.Interval = _healthCheckIntervalSeconds * 1000 ' Go back to slow polling
                    ChangeState(ConnectionState.Offline)
                Else
                    Dim backoffIndex = Math.Min(_retryCount - 1, _retryBackoffSeconds.Length - 1)
                    Dim nextInterval = _retryBackoffSeconds(backoffIndex)

                    _logger.LogWarning($"Connection check failed (Retry {_retryCount}/{_maxRetries}). Next check in {nextInterval}s.")
                    _timer.Interval = nextInterval * 1000
                    ChangeState(ConnectionState.Reconnecting)
                End If
            End If
        End Sub

        Private Sub ChangeState(newState As ConnectionState)
            If _currentState <> newState Then
                _currentState = newState
                ' Events raised by Windows.Forms.Timer tick are already on the UI thread,
                ' but if we called this from Task.Run, we need to ensure subscribers handle UI updates safely
                ' or dispatch it. Since we will dispatch in the Presenter or Extender if needed, we just raise.
                RaiseEvent StateChanged(Me, New ConnectionStateChangedEventArgs(_currentState))
            End If
        End Sub

        Private disposedValue As Boolean
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    [Stop]()
                    If _timer IsNot Nothing Then
                        _timer.Dispose()
                    End If
                End If
                disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace