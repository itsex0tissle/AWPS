using System;
using System.Device.Adc;
using System.Diagnostics;
using Iot.Device.DHTxx.Esp32;

namespace AWPS.IoT.Measuring
{
    public static class MeasuringWork
    {
        public static void Start()
        {
            MeasuringDataFile.Load();
            AdcController controller = new();
            AdcChannel light = controller.OpenChannel(4);
            AdcChannel moisture = controller.OpenChannel(5);
            Dht11 dht11 = new(25, 26);
            for(int retry = 0; retry < 5; retry++)
            {
                try
                {
                    Debug.WriteLine("Start measuring...");
                    double light_value = light.ReadRatio() * 100;
                    double moisture_value = moisture.ReadRatio() * 100;
                    double temperature_value = dht11.Temperature.DegreesCelsius;
                    double humidity_value = dht11.Humidity.Percent;
                    for(int count = 1; count < 10; count++)
                    {
                        light_value += light.ReadRatio() * 100;
                        light_value /= 2;
                        moisture_value += moisture.ReadRatio() * 100;
                        moisture_value /= 2;
                        temperature_value += dht11.Temperature.DegreesCelsius;
                        temperature_value /= 2;
                        humidity_value += dht11.Humidity.Percent;
                        humidity_value /= 2;
                    }
                    MeasuringDataFile.AddRecord(light_value, moisture_value, temperature_value, humidity_value);
                    MeasuringDataFile.Save();
                    Debug.WriteLine("Measuring finished");
                    return;
                }
                catch(Exception exc)
                {
                    Debug.WriteLine($"Measuring failed due to exception: {exc}");
                }
            }
        }
    }
}