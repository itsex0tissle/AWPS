#nullable enable

using AWPS.IoT.BinaryRecords;

namespace AWPS.IoT.WebControllers.Records
{
    public sealed class GetWifiStatusResponseRecord : BinaryRecord
    {
        #region Instance
        public bool Connected { get; set; } = false;
        public string SSID { get; set; } = "";
        #endregion

        #region BinaryRecord
        protected override int ChildByteLength
        {
            get => SSID.Length + 5;
        }
        public override BinaryRecordType Type
        {
            get => BinaryRecordType.GetWifiStatusResponse;
        }
        public override byte Version
        {
            get => 1;
        }

        protected override void SerializeChild(byte[] buffer, int offset)
        {
            BinaryRecord.WriteBool(buffer, ref offset, Connected);
            BinaryRecord.WriteString(buffer, ref offset, SSID);
        }
        protected override void DeserializeChild(byte[] buffer, int offset)
        {
            Connected = BinaryRecord.ReadBool(buffer, ref offset);
            SSID = BinaryRecord.ReadString(buffer, ref offset);
        }
        #endregion
    }
}