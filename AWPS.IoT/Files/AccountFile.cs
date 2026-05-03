using System.IO;
using AWPS.IoT.Models;
using AWPS.IoT.Services;
using nanoFramework.Json;

namespace AWPS.IoT.Files
{
    public static class AccountFile
    {
        public const string FilePath = "I:\\Account.json";

        private static AccountRecord? record;

        public static AccountRecord Record
        {
            get => record ??= Load();
            set
            {
                record = value;
                Save();
            }
        }

        private static AccountRecord Load()
        {
            if(File.Exists(FilePath) is false)
            {
                return new AccountRecord();
            }
            string content = File.ReadAllText(FilePath);
            Logger.LogInfo($"'{FilePath}' file loaded");
            return (AccountRecord)JsonConvert.DeserializeObject(content, typeof(AccountRecord));
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
            Record = new AccountRecord();
            File.Delete(FilePath);
            Logger.LogInfo($"'{FilePath}' file deleted");
        }
    }
}