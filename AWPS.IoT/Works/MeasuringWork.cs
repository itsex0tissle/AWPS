using System.Device.Adc;
using System.Diagnostics;
using AWPS.IoT.BinaryFiles;
using Iot.Device.DHTxx.Esp32;

namespace AWPS.IoT.Works
{
    public static class MeasuringWork
    {
        public static void Start(int measure_count = 10, int retry = 5)
        {
            try
            {
                Debug.WriteLine("Start measuring work");
                AdcController controller = new();
                AdcChannel light = controller.OpenChannel(4);
                AdcChannel moisture = controller.OpenChannel(5);
                Dht11 dht11 = new(25, 26);
                Helper.Retry(delegate()
                {
                    double light_value = light.ReadRatio() * 100;
                    double moisture_value = moisture.ReadRatio() * 100;
                    double temperature_value = dht11.Temperature.DegreesCelsius;
                    double humidity_value = dht11.Humidity.Percent;
                    for(int count = 1; count < measure_count; count++)
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
                    MainDataFile.Instance.AddSensorsData(light_value, moisture_value, temperature_value, humidity_value);
                    MainDataFile.Instance.Save();
                }, retry);
                Debug.WriteLine("Measuring work finished");
            }
            catch { }
        }
    }
}