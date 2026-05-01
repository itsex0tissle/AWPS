using System.IO;
using System.Collections;
using System.Diagnostics;
using AWPS.IoT.MsgPack.Models;
using nanoFramework.MessagePack;

namespace AWPS.IoT.Files
{
    public static class SensorsDataFile
    {
        public const string FilePath = "I:\\SensorsData.msgpk";

        private static ArrayList? records;

        private static ArrayList Records
        {
            get => records ??= Load();
        }
        public static int Count
        {
            get => Records.Count;
        }

        private static ArrayList Load()
        {
            if(File.Exists(FilePath) is false)
            {
                return new ArrayList();
            }
            byte[] content = File.ReadAllBytes(FilePath);
            Debug.WriteLine($"'{FilePath}' file loaded");
            var records = (SensorsDataRecord[])MessagePackSerializer.Deserialize(typeof(SensorsDataRecord[]), content)!;

            ArrayList list = new();
            foreach(SensorsDataRecord record in records)
            {
                list.Add(record);
            }
            return list;
        }
        public static void Add(SensorsDataRecord record)
        {
            Records.Add(record);
        }
        public static void Remove(SensorsDataRecord record)
        {
            Records.Remove(record);
        }
        public static int IndexOf(SensorsDataRecord record)
        {
            return Records.IndexOf(record);
        }
        public static bool Contains(SensorsDataRecord record)
        {
            return Records.Contains(record);
        }
        public static SensorsDataRecord Get(int index)
        {
            return (SensorsDataRecord)Records[index];
        }
        public static void RemoveAt(int index)
        {
            Records.RemoveAt(index);
        }
        public static byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(Records);
        }
        public static void Save()
        {
            File.WriteAllBytes(FilePath, Serialize());
            Debug.WriteLine($"'{FilePath}' file saved");
        }
        public static void Reset()
        {
            Records.Clear();
            File.Delete(FilePath);
            Debug.WriteLine($"'{FilePath}' file deleted");
        }
    }
}