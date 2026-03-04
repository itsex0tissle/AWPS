#nullable enable

using System;
using System.IO;
using nanoFramework.Json;
using System.Collections;
using System.Diagnostics;

namespace AWPS.IoT.Measuring
{
    public static class MeasuringDataFile
    {
        public const string FilePath = "I:\\MeasuringData.json";

        public static ArrayList Records { get; private set; } = new();

        public static void AddRecord(double light, double moisture, double temperature, double humidity)
        {
            MeasuringDataRecord record = new()
            {
                Light = (int)Math.Round(light),
                Moisture = (int)Math.Round(moisture),
                Temperature = (int)Math.Round(temperature),
                Humidity = (int)Math.Round(humidity)
            };
            Debug.WriteLine($"Add record to MeasuringData file: {JsonConvert.SerializeObject(record)}");
            Records.Add(record);
        }
        public static void Save()
        {
            string content = JsonConvert.SerializeObject(Records);
            Debug.WriteLine($"Saving MeasuringData file: {content}");
            File.WriteAllText(FilePath, content);
        }
        public static void Load()
        {
            if(File.Exists(FilePath) is false)
            {
                Debug.WriteLine("Can`t load MeasuringData file, because it doesn`t exists");
                return;
            }
            try
            {
                string content = File.ReadAllText(FilePath);
                Debug.WriteLine($"Loading MeasureData file: {content}");
                var records = (MeasuringDataRecord[])JsonConvert.DeserializeObject(content, typeof(MeasuringDataRecord[]));
                Records = new ArrayList();
                foreach(MeasuringDataRecord record in records)
                {
                    Records.Add(record);
                }
                Debug.WriteLine($"MeasureData file loaded");
            }
            catch(Exception exc)
            {
                Debug.WriteLine($"Loading MeasureData file failed due to exception: {exc}");
            }
        }
        public static void Reset()
        {
            Records.Clear();
            File.Delete(FilePath);
            Debug.WriteLine("Measuring file cleared");
        }
    }
}