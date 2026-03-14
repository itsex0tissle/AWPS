using AWPS.IoT.BinaryRecords;

namespace AWPS.IoT.WebControllers.Records
{
    public sealed class PostWifiRequestRecord : BinaryRecord
    {
        #region Instance
        public string SSID { get; set; } = "";
        public string Password { get; set; } = "";
        #endregion

        #region BinaryRecord
        protected override int ChildByteLength
        {
            get => SSID.Length + Password.Length + 8;
        }
        public override BinaryRecordType Type
        {
            get => BinaryRecordType.PostWifiRequest;
        }
        public override byte Version
        {
            get => 1;
        }

        protected override void SerializeChild(byte[] buffer, int offset)
        {
            BinaryRecord.WriteString(buffer, ref offset, SSID);
            BinaryRecord.WriteString(buffer, ref offset, Password);
        }
        protected override void DeserializeChild(byte[] buffer, int offset)
        {
            SSID = BinaryRecord.ReadString(buffer, ref offset);
            Password = BinaryRecord.ReadString(buffer, ref offset);
        }
        #endregion
    }
}