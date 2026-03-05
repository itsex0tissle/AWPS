#nullable enable

using AWPS.IoT.BinaryRecords;

namespace AWPS.IoT.WebControllers.Records
{
    public sealed class PostWifiResponseRecord : BinaryRecord
    {
        #region Instance
        public bool Success { get; set; } = false;
        public string Description { get; set; } = "";
        #endregion

        #region BinaryRecord
        protected override int ChildByteLength
        {
            get => Description.Length + 5;
        }
        public override BinaryRecordType Type
        {
            get => BinaryRecordType.PostWifiResponse;
        }
        public override byte Version
        {
            get => 1;
        }

        protected override void SerializeChild(byte[] buffer, int offset)
        {
            BinaryRecord.WriteBool(buffer, ref offset, Success);
            BinaryRecord.WriteString(buffer, ref offset, Description);
        }
        protected override void DeserializeChild(byte[] buffer, int offset)
        {
            Success = BinaryRecord.ReadBool(buffer, ref offset);
            Description = BinaryRecord.ReadString(buffer, ref offset);
        }
        #endregion
    }
}