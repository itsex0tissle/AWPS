namespace AWPS.UI.Shared.Models;

public sealed class WifiAvailableNetworkRecord
{
    public string SSID { get; set; } = "";
    public byte SignalBars { get; set; }
}