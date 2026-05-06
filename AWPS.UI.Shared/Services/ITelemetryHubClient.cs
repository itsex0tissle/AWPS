namespace AWPS.UI.Shared.Services;

/// <summary>
/// Represents the contract for SignalR telemetry hub client methods.
/// </summary>
public interface ITelemetryHubClient
{
    /// <summary>
    /// Called when new telemetry data is available for a device profile.
    /// </summary>
    /// <param name="deviceProfileId">The device profile ID that has new telemetry</param>
    Task OnTelemetryUpdated(string deviceProfileId);
}
