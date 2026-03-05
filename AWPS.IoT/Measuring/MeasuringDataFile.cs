#nullable enable

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using AWPS.IoT.BinaryRecords;

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
                Light = (byte)Math.Round(light),
                Moisture = (byte)Math.Round(moisture),
                Humidity = (byte)Math.Round(humidity),
                Temperature = (sbyte)Math.Round(temperature)
            };
            Debug.WriteLine($"Add record to MeasuringDataFile");
            Records.Add(record);
        }
        public static void Save()
        {
            byte[] buffer = BinaryRecord.SerializeEnumerable(Records);
            Debug.WriteLine($"Saving MeasuringDataFile");
            File.WriteAllBytes(FilePath, buffer);
        }
        public static void Load()
        {
            if(File.Exists(FilePath) is false)
            {
                Debug.WriteLine("Can`t load MeasuringDataFile, because it doesn`t exists");
                return;
            }
            try
            {
                byte[] buffer = File.ReadAllBytes(FilePath);
                Records = BinaryRecord.DeserializeEnumerable(buffer);
                Debug.WriteLine($"MeasureDataFile loaded");
            }
            catch(Exception exc)
            {
                Debug.WriteLine($"Loading MeasureDataFile failed due to exception: {exc}");
            }
        }
        public static void Reset()
        {
            Records.Clear();
            File.Delete(FilePath);
            Debug.WriteLine("MeasuringDataFile cleared");
        }
    }
}