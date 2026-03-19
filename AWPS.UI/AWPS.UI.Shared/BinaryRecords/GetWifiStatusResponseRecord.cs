namespace AWPS.IoT.BinaryRecords
{
    public sealed class GetWifiStatusResponseRecord : BinaryRecord
    {
        #region Static
        public static GetWifiStatusResponseRecord Deserialize(byte[] buffer)
        {
            int offset = 0;
            return Deserialize(buffer, ref offset);
        }
        new public static GetWifiStatusResponseRecord Deserialize(byte[] buffer, ref int offset)
        {
            BinaryRecord result = new GetWifiStatusResponseRecord();
            result.Deserialize(buffer, ref offset);
            return (GetWifiStatusResponseRecord)result;
        }
        #endregion

        #region Instance
        public bool Connected { get; set; } = false;
        public string SSID { get; set; } = "";
        #endregion

        #region BinaryRecord
        protected override int ChildByteLength
        {
            get => sizeof(bool) + BinaryRecord.SizeOfString(SSID);
        }
        public override BinaryRecord.Type RecordType
        {
            get => BinaryRecord.Type.GetWifiStatusResponse;
        }
        public override byte Version
        {
            get => 1;
        }

        protected override void SerializeChild(byte[] buffer, ref int offset)
        {
            BinaryRecord.WriteBool(buffer, ref offset, Connected);
            BinaryRecord.WriteString(buffer, ref offset, SSID);
        }
        protected override void DeserializeChild(byte[] buffer, ref int offset)
        {
            Connected = BinaryRecord.ReadBool(buffer, ref offset);
            SSID = BinaryRecord.ReadString(buffer, ref offset);
        }
        #endregion
    }
}