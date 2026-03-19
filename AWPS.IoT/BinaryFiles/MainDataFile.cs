using System;
using AWPS.IoT.BinaryRecords;

namespace AWPS.IoT.BinaryFiles
{
    public sealed class MainDataFile : RecordsFile
    {
        #region Static
        public const string Path = "I:\\MainData.bin";

        private static MainDataFile? instance;
        public static MainDataFile Instance
        {
            get => instance ??= InitLoad();
        }

        private static MainDataFile InitLoad()
        {
            MainDataFile file = new();
            return (file.Load() is MainDataFile loaded_file) ? loaded_file : file;
        }
        public static MainDataFile Deserialize(byte[] buffer)
        {
            int offset = 0;
            return Deserialize(buffer, ref offset);
        }
        new public static MainDataFile Deserialize(byte[] buffer, ref int offset)
        {
            BinaryRecord result = new MainDataFile();
            result.Deserialize(buffer, ref offset);
            return (MainDataFile)result;
        }
        public static void AddSensorsData(double light, double moisture, double temperature, double humidity)
        {
            SensorsDataRecord record = new()
            {
                Light = (byte)Math.Round(light),
                Moisture = (byte)Math.Round(moisture),
                Temperature = (sbyte)Math.Round(temperature),
                Humidity = (byte)Math.Round(humidity)
            };
            Instance.Records.Add(record);
        }
        public static void AddMessage(string message)
        {
            TimestampMessageRecord record = new()
            {
                Message = message
            };
            Instance.Records.Add(record);
        }
        #endregion

        #region Instance
        public MainDataFile() : base(MainDataFile.Path)
        {
            
        }
        #endregion

        #region BinaryRecord
        protected override int ChildByteLength2
        {
            get => 0;
        }
        public override BinaryRecord.Type RecordType
        {
            get => BinaryRecord.Type.MainDataFile;
        }
        public override byte Version
        {
            get => 1;
        }

        protected override void SerializeChild2(byte[] buffer, ref int offset)
        {
            
        }
        protected override void DeserializeChild2(byte[] buffer, ref int offset)
        {
            
        }
        #endregion
    }
}