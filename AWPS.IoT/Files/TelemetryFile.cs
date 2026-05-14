using System.IO;
using AWPS.IoT.Services;
using System.Collections;
using AWPS.IoT.MsgPack.Models;
using nanoFramework.MessagePack;

namespace AWPS.IoT.Files
{
    public static class TelemetryFile
    {
        public const string FilePath = "I:\\Telemetry.msgpk";

        private static ArrayList? records;

        internal static ArrayList Records
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
            Logger.LogInfo($"'{FilePath}' file loaded");
            var records = (TelemetryRecord[])MessagePackSerializer.Deserialize(typeof(TelemetryRecord[]), content)!;

            ArrayList list = new();
            foreach(TelemetryRecord record in records)
            {
                list.Add(record);
            }
            return list;
        }
        public static void Add(TelemetryRecord record)
        {
            Records.Add(record);
        }
        public static void Remove(TelemetryRecord record)
        {
            Records.Remove(record);
        }
        public static int IndexOf(TelemetryRecord record)
        {
            return Records.IndexOf(record);
        }
        public static bool Contains(TelemetryRecord record)
        {
            return Records.Contains(record);
        }
        public static TelemetryRecord Get(int index)
        {
            return (TelemetryRecord)Records[index];
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
            Logger.LogInfo($"'{FilePath}' file saved");
        }
        public static void Reset()
        {
            Records.Clear();
            File.Delete(FilePath);
            Logger.LogInfo($"'{FilePath}' file deleted");
        }
    }
}