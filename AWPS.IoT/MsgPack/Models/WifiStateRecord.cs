namespace AWPS.IoT.MsgPack.Models
{
    public sealed class WifiStateRecord
    {
        public string SSID { get; set; } = "";
        public bool Connected { get; set; }
    }
}