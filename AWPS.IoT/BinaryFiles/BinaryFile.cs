using System.IO;
using AWPS.IoT.BinaryRecords;

namespace AWPS.IoT.BinaryFiles
{
    public abstract class BinaryFile : BinaryRecord
    {
        public abstract string FilePath { get; }

        public void Save()
        {
            File.WriteAllBytes(FilePath, base.Serialize());
        }
        public BinaryFile? Load()
        {
            if(File.Exists(FilePath) is false)
            {
                return null;
            }
            else
            {
                int offset = 0;
                byte[] buffer = File.ReadAllBytes(FilePath);
                return (BinaryFile)BinaryRecord.ReadRecord(buffer, ref offset);
            }
        }
        public void Reset()
        {
            File.Delete(FilePath);
        }
    }
}