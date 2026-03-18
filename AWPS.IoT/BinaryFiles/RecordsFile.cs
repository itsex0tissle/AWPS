using System.Collections;
using AWPS.IoT.BinaryRecords;

namespace AWPS.IoT.BinaryFiles
{
    public abstract class RecordsFile : BinaryFile
    {
        #region Instance
        protected abstract int ChildByteLength2 { get; }
        public ArrayList Records { get; protected set; } = new();

        protected RecordsFile(string file_path)
        {
            FilePath = file_path;
        }

        protected abstract void SerializeChild2(byte[] buffer, ref int offset);
        protected abstract void DeserializeChild2(byte[] buffer, ref int offset);
        #endregion

        #region BinaryFile
        public override string FilePath { get; }
        #endregion

        #region BinaryRecord
        protected override sealed int ChildByteLength
        {
            get => BinaryRecord.SizeOfCollection(BinaryRecords) + ChildByteLength2;
        }

        protected override sealed void SerializeChild(byte[] buffer, ref int offset)
        {
            BinaryRecord.WriteCollection(buffer, ref offset, Records);
            SerializeChild2(buffer, ref offset);
        }
        protected override sealed void DeserializeChild(byte[] buffer, ref int offset)
        {
            Records = BinaryRecord.ReadCollection(buffer, ref offset, typeof(BinaryRecord));
            DeserializeChild2(buffer, ref offset);
        }
        #endregion
    }
}