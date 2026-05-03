using System.IO;
using AWPS.IoT.Models;
using AWPS.IoT.Services;
using nanoFramework.Json;

namespace AWPS.IoT.Files
{
    public static class DeviceStateFile
    {
        public const string FilePath = "I:\\DeviceState.json";

        private static DeviceStateRecord? record;

        public static DeviceStateRecord Record
        {
            get => record ??= Load();
            set
            {
                record = value;
                Save();
            }
        }

        private static DeviceStateRecord Load()
        {
            if (File.Exists(FilePath) is false)
            {
                return new DeviceStateRecord();
            }
            string content = File.ReadAllText(FilePath);
            Logger.LogInfo($"'{FilePath}' file loaded");
            return (DeviceStateRecord)JsonConvert.DeserializeObject(content, typeof(DeviceStateRecord));
        }
        public static string Serialize()
        {
            return JsonConvert.SerializeObject(Record);
        }
        public static void Save()
        {
            File.WriteAllText(FilePath, Serialize());
            Logger.LogInfo($"'{FilePath}' file saved");
        }
        public static void Reset()
        {
            Record = new DeviceStateRecord();
            File.Delete(FilePath);
            Logger.LogInfo($"'{FilePath}' file deleted");
        }
    }
}