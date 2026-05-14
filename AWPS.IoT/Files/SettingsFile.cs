using System.IO;
using AWPS.IoT.Models;
using AWPS.IoT.Services;
using nanoFramework.Json;

namespace AWPS.IoT.Files
{
    public static class SettingsFile
    {
        public const string FilePath = "I:\\Settings.json";

        private static SettingsRecord? record;

        public static SettingsRecord Record
        {
            get => record ??= Load();
            set
            {
                record = value;
                Save();
            }
        }

        private static SettingsRecord Load()
        {
            if (File.Exists(FilePath) is false)
            {
                return new SettingsRecord();
            }
            string content = File.ReadAllText(FilePath);
            Logger.LogInfo($"'{FilePath}' file loaded");
            return (SettingsRecord)JsonConvert.DeserializeObject(content, typeof(SettingsRecord));
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
            Record = new SettingsRecord();
            File.Delete(FilePath);
            Logger.LogInfo($"'{FilePath}' file deleted");
        }
    }
}