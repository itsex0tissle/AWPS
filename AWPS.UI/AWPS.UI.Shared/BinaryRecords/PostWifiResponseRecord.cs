namespace AWPS.IoT.BinaryRecords
{
    public sealed class PostWifiResponseRecord : BinaryRecord
    {
        #region Static
        public static PostWifiResponseRecord Deserialize(byte[] buffer)
        {
            int offset = 0;
            return Deserialize(buffer, ref offset);
        }
        new public static PostWifiResponseRecord Deserialize(byte[] buffer, ref int offset)
        {
            BinaryRecord result = new PostWifiResponseRecord();
            result.Deserialize(buffer, ref offset);
            return (PostWifiResponseRecord)result;
        }
        #endregion

        #region Instance
        public bool Success { get; set; } = false;
        public string Description { get; set; } = "";
        #endregion

        #region BinaryRecord
        protected override int ChildByteLength
        {
            get => sizeof(bool) + BinaryRecord.SizeOfString(Description);
        }
        public override BinaryRecord.Type RecordType
        {
            get => BinaryRecord.Type.PostWifiResponse;
        }
        public override byte Version
        {
            get => 1;
        }

        protected override void SerializeChild(byte[] buffer, ref int offset)
        {
            BinaryRecord.WriteBool(buffer, ref offset, Success);
            BinaryRecord.WriteString(buffer, ref offset, Description);
        }
        protected override void DeserializeChild(byte[] buffer, ref int offset)
        {
            Success = BinaryRecord.ReadBool(buffer, ref offset);
            Description = BinaryRecord.ReadString(buffer, ref offset);
        }
        #endregion
    }
}