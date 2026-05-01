namespace AWPS.IoT.MsgPack.Models
{
    public sealed class WifiConnectionResultRecord
    {
        public bool Connected { get; set; }
        public string Message { get; set; } = "";
    }
}