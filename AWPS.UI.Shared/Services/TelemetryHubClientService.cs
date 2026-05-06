using Microsoft.AspNetCore.SignalR.Client;

namespace AWPS.UI.Shared.Services;

/// <summary>
/// Service for managing SignalR connections to the telemetry hub on the server.
/// Handles connection lifecycle and subscriptions to device profile updates.
/// </summary>
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

    /// <summary>
    /// Gets the current connection state.
    /// </summary>
    public HubConnectionState ConnectionState => _connection?.State ?? HubConnectionState.Disconnected;

    /// <summary>
    /// Connects to the telemetry hub and sets up event handlers.
    /// </summary>
    /// <param name="onTelemetryUpdated">Callback invoked when telemetry is updated</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task ConnectAsync(Func<string, Task> onTelemetryUpdated, CancellationToken cancellationToken = default)
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

        _connection.On<string>("TelemetryUpdated", onTelemetryUpdated);

        await _connection.StartAsync(cancellationToken);
    }

    /// <summary>
    /// Subscribes to telemetry updates for a specific device profile.
    /// </summary>
    /// <param name="deviceProfileId">The device profile ID to subscribe to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task SubscribeToDeviceAsync(string deviceProfileId, CancellationToken cancellationToken = default)
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
    }

    /// <summary>
    /// Unsubscribes from telemetry updates for a specific device profile.
    /// </summary>
    /// <param name="deviceProfileId">The device profile ID to unsubscribe from</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task UnsubscribeFromDeviceAsync(string deviceProfileId, CancellationToken cancellationToken = default)
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
    }

    /// <summary>
    /// Disconnects from the telemetry hub.
    /// </summary>
    public async Task DisconnectAsync()
    {
        if (_connection?.State == HubConnectionState.Connected)
        {
            await _connection.StopAsync();
        }
    }

    /// <summary>
    /// Disposes the hub connection.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }

    /// <summary>
    /// Custom retry policy for automatic reconnection.
    /// </summary>
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
