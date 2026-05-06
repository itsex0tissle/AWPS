using Microsoft.AspNetCore.SignalR;

namespace AWPS.IoT.Server.Hubs;

/// <summary>
/// SignalR hub for broadcasting telemetry updates to connected clients.
/// Clients join groups based on device profile ID.
/// </summary>
public sealed class TelemetryHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Allows a client to subscribe to telemetry updates for a specific device profile.
    /// </summary>
    /// <param name="deviceProfileId">The device profile ID to subscribe to</param>
    public async Task SubscribeToDevice(string deviceProfileId)
    {
        ArgumentNullException.ThrowIfNull(deviceProfileId);
        Console.WriteLine($"SignalR subscribe to: {deviceProfileId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, deviceProfileId);
    }

    /// <summary>
    /// Allows a client to unsubscribe from telemetry updates for a specific device profile.
    /// </summary>
    /// <param name="deviceProfileId">The device profile ID to unsubscribe from</param>
    public async Task UnsubscribeFromDevice(string deviceProfileId)
    {
        ArgumentNullException.ThrowIfNull(deviceProfileId);
        Console.WriteLine($"SignalR unsubscribe from: {deviceProfileId}");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, deviceProfileId);
    }
}