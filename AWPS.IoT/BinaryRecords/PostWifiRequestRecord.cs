namespace AWPS.IoT.BinaryRecords
{
    public sealed class PostWifiRequestRecord : BinaryRecord
    {
        #region Static
        new public static PostWifiRequestRecord Deserialize(byte[] buffer, ref int offset)
        {
            BinaryRecord result = new PostWifiRequestRecord();
            result.Deserialize(buffer, ref offset);
            return (PostWifiRequestRecord)result;
        }
        #endregion

        #region Instance
        public string SSID { get; set; } = "";
        public string Password { get; set; } = "";
        #endregion

        #region BinaryRecord
        protected override int ChildByteLength
        {
            get => BinaryRecord.SizeOfString(SSID) + BinaryRecord.SizeOfString(Password);
        }
        public override BinaryRecord.Type RecordType
        {
            get => BinaryRecord.Type.PostWifiRequest;
        }
        public override byte Version
        {
            get => 1;
        }

        protected override void SerializeChild(byte[] buffer, ref int offset)
        {
            BinaryRecord.WriteString(buffer, ref offset, SSID);
            BinaryRecord.WriteString(buffer, ref offset, Password);
        }
        protected override void DeserializeChild(byte[] buffer, ref int offset)
        {
            SSID = BinaryRecord.ReadString(buffer, ref offset);
            Password = BinaryRecord.ReadString(buffer, ref offset);
        }
        #endregion
    }
}