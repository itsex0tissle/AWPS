namespace AWPS.UI.Shared.Models;

public sealed class WifiAvailableNetworkResponse
{
    public string SSID { get; set; } = "";
    public byte SignalBars { get; set; }
}