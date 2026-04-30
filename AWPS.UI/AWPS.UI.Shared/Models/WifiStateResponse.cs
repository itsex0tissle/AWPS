namespace AWPS.UI.Shared.Models;

public sealed class WifiStateResponse
{
    public string SSID { get; set; } = "";
    public bool Connected { get; set; }
}