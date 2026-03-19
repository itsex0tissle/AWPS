namespace AWPS.IoT.BinaryRecords
{
    public sealed class TimestampMessageRecord : TimestampBinaryRecord
    {
        #region Static
        public static TimestampMessageRecord Deserialize(byte[] buffer)
        {
            int offset = 0;
            return Deserialize(buffer, ref offset);
        }
        new public static TimestampMessageRecord Deserialize(byte[] buffer, ref int offset)
        {
            BinaryRecord result = new TimestampMessageRecord();
            result.Deserialize(buffer, ref offset);
            return (TimestampMessageRecord)result;
        }
        #endregion

        #region Instance
        public string Message { get; set; } = "";
        #endregion

        #region TimestampBinaryRecord
        protected override int ChildByteLength2
        {
            get => BinaryRecord.SizeOfString(Message);
        }

        protected override void SerializeChild2(byte[] buffer, ref int offset)
        {
            BinaryRecord.WriteString(buffer, ref offset, Message);
        }
        protected override void DeserializeChild2(byte[] buffer, ref int offset)
        {
            Message = BinaryRecord.ReadString(buffer, ref offset);
        }
        #endregion

        #region BinaryRecord
        public override BinaryRecord.Type RecordType
        {
            get => BinaryRecord.Type.TimestampMessage;
        }
        public override byte Version
        {
            get => 1;
        }
        #endregion
    }
}