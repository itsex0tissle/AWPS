using Microsoft.AspNetCore.SignalR.Client;

namespace AWPS.UI.Shared.Services;

public sealed class TelemetryHubClientService : IAsyncDisposable
{
    private HubConnection? _connection;
    private readonly string _hubUrl;
    private string? _currentDeviceProfileId;

    public TelemetryHubClientService(string hubUrl)
    {
        ArgumentNullException.ThrowIfNull(hubUrl);
        _hubUrl = hubUrl;
    }

    public HubConnectionState ConnectionState => _connection?.State ?? HubConnectionState.Disconnected;

    public async Task ConnectAsync(Func<string, Task> onTelemetryUpdated, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(onTelemetryUpdated);

            if (_connection?.State == HubConnectionState.Connected)
            {
                return;
            }

            _connection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect(new DefaultRetryPolicy())
                .Build();

            _connection.On("TelemetryUpdated", onTelemetryUpdated);

            await _connection.StartAsync(cancellationToken);
            Console.WriteLine("SignalR connected");
        }
        catch { }
    }
    public async Task SubscribeToDeviceAsync(string deviceProfileId, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(deviceProfileId);

            if (_connection?.State != HubConnectionState.Connected)
            {
                throw new InvalidOperationException("Not connected to hub");
            }

            // Unsubscribe from previous device if different
            if (_currentDeviceProfileId is not null && _currentDeviceProfileId != deviceProfileId)
            {
                await UnsubscribeFromDeviceAsync(_currentDeviceProfileId, cancellationToken);
            }

            _currentDeviceProfileId = deviceProfileId;
            await _connection.InvokeAsync("SubscribeToDevice", deviceProfileId, cancellationToken);
            Console.WriteLine("SignalR called SubscribeToDevice");
        }
        catch { }
    }
    public async Task UnsubscribeFromDeviceAsync(string deviceProfileId, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(deviceProfileId);

            if (_connection?.State != HubConnectionState.Connected)
            {
                return;
            }

            if (_currentDeviceProfileId == deviceProfileId)
            {
                _currentDeviceProfileId = null;
            }

            await _connection.InvokeAsync("UnsubscribeFromDevice", deviceProfileId, cancellationToken);
            Console.WriteLine("SignalR called UnsubscribeFromDevice");
        }
        catch { }
    }
    public async Task DisconnectAsync()
    {
        if (_connection?.State == HubConnectionState.Connected)
        {
            await _connection.StopAsync();
        }
    }
    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }

    private sealed class DefaultRetryPolicy : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext context) =>
            context.PreviousRetryCount switch
            {
                0 => TimeSpan.FromMilliseconds(500),
                1 => TimeSpan.FromMilliseconds(1000),
                2 => TimeSpan.FromMilliseconds(5000),
                _ => null
            };
    }
}
