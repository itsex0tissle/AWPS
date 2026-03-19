using System;

namespace AWPS.IoT.BinaryRecords
{
    public abstract class TimestampBinaryRecord : BinaryRecord
    {
        #region Instance
        protected abstract int ChildByteLength2 { get; }   
        public long Timestamp { get; private set; } = DateTime.UtcNow.Ticks;

        protected abstract void SerializeChild2(byte[] buffer, ref int offset);
        protected abstract void DeserializeChild2(byte[] buffer, ref int offset);
        #endregion

        #region BinaryRecord
        protected sealed override int ChildByteLength
        {
            get => ChildByteLength2 + sizeof(long);
        }

        protected sealed override void SerializeChild(byte[] buffer, ref int offset)
        {
            BinaryRecord.WriteLong(buffer, ref offset, Timestamp);
            SerializeChild2(buffer, ref offset);
        }
        protected sealed override void DeserializeChild(byte[] buffer, ref int offset)
        {
            Timestamp = BinaryRecord.ReadLong(buffer, ref offset);
            DeserializeChild2(buffer, ref offset);
        }
        #endregion
    }
}