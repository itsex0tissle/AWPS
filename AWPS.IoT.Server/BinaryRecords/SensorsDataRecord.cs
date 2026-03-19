namespace AWPS.IoT.BinaryRecords
{
    public sealed class SensorsDataRecord : TimestampBinaryRecord
    {
        #region Static
        public static SensorsDataRecord Deserialize(byte[] buffer)
        {
            int offset = 0;
            return Deserialize(buffer, ref offset);
        }
        new public static SensorsDataRecord Deserialize(byte[] buffer, ref int offset)
        {
            BinaryRecord result = new SensorsDataRecord();
            result.Deserialize(buffer, ref offset);
            return (SensorsDataRecord)result;
        }
        #endregion

        #region Instance
        public byte Light { get; set; } = 0;
        public byte Moisture { get; set; } = 0;
        public sbyte Temperature { get; set; } = 0;
        public byte Humidity { get; set; } = 0;
        #endregion

        #region TimestampBinaryRecord
        protected override int ChildByteLength2
        {
            get => sizeof(byte) + sizeof(byte) + sizeof(sbyte) + sizeof(byte);
        }

        protected override void SerializeChild2(byte[] buffer, ref int offset)
        {
            BinaryRecord.WriteByte(buffer, ref offset, Light);
            BinaryRecord.WriteByte(buffer, ref offset, Moisture);
            BinaryRecord.WriteSByte(buffer, ref offset, Temperature);
            BinaryRecord.WriteByte(buffer, ref offset, Humidity);
        }
        protected override void DeserializeChild2(byte[] buffer, ref int offset)
        {
            Light = BinaryRecord.ReadByte(buffer, ref offset);
            Moisture = BinaryRecord.ReadByte(buffer, ref offset);
            Temperature = BinaryRecord.ReadSByte(buffer, ref offset);
            Humidity = BinaryRecord.ReadByte(buffer, ref offset);
        }
        #endregion

        #region BinaryRecord
        public override BinaryRecord.Type RecordType
        {
            get => BinaryRecord.Type.SensorsData;
        }
        public override byte Version
        {
            get => 1;
        }
        #endregion
    }
}