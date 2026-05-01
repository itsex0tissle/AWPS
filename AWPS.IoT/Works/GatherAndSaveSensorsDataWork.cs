using System;
using AWPS.IoT.Files;
using System.Device.Adc;
using System.Diagnostics;
using Iot.Device.DHTxx.Esp32;
using AWPS.IoT.MsgPack.Models;

namespace AWPS.IoT.Works
{
    public static class GatherAndSaveSensorsDataWork
    {
        public static void Start(int measure_count = 10, int retry = 5)
        {
            try
            {
                Debug.WriteLine("GatherAndSaveSensorsDataWork started");
                AdcController controller = new();
                AdcChannel light_sensor = controller.OpenChannel(4);
                AdcChannel moisture_sensor = controller.OpenChannel(5);
                Dht11 dht11 = new(25, 26);
                Helper.Retry(delegate()
                {
                    double light = light_sensor.ReadRatio();
                    double moisture = moisture_sensor.ReadRatio();
                    double humidity = dht11.Humidity.Percent;
                    double temperature = dht11.Temperature.DegreesCelsius;
                    for(int count = 1; count < measure_count; count++)
                    {
                        light += light_sensor.ReadRatio();
                        moisture += moisture_sensor.ReadRatio();
                        humidity += dht11.Humidity.Percent;
                        temperature += dht11.Temperature.DegreesCelsius;
                    }
                    SensorsDataRecord record = new()
                    {
                        Light = (byte)(light * 100 / measure_count),
                        Moisture = (byte)(moisture * 100 / measure_count),
                        Humidity = (byte)(humidity / measure_count),
                        Temperature = (sbyte)(temperature / measure_count),
                    };
                    SensorsDataFile.Add(record);
                    SensorsDataFile.Save();
                }, retry);
                Debug.WriteLine("GatherAndSaveSensorsDataWork finished");
            }
            catch(Exception exc)
            {
                Debug.WriteLine($"GatherAndSaveSensorsDataWork failed: {exc}");
            }
        }
    }
}