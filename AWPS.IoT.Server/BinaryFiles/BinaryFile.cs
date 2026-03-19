using System.IO;
using System.Diagnostics;
using AWPS.IoT.BinaryRecords;

namespace AWPS.IoT.BinaryFiles
{
    public abstract class BinaryFile : BinaryRecord
    {
        public abstract string FilePath { get; }

        public virtual void Save()
        {
            File.WriteAllBytes(FilePath, base.Serialize());
            Debug.WriteLine($"'{FilePath}' file saved");
        }
        public virtual BinaryFile? Load()
        {
            if (File.Exists(FilePath) is false)
            {
                return null;
            }
            else
            {
                int offset = 0;
                byte[] buffer = File.ReadAllBytes(FilePath);
                Debug.WriteLine($"'{FilePath}' file loaded");
                return (BinaryFile)BinaryRecord.ReadRecord(buffer, ref offset);
            }
        }
        public virtual void Reset()
        {
            File.Delete(FilePath);
            Debug.WriteLine($"'{FilePath}' file deleted");
        }
    }
}