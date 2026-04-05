using System.Threading;
using System.Device.Gpio;
using System.Diagnostics;
using AWPS.IoT.BinaryFiles;
using AWPS.IoT.BinaryRecords;

namespace AWPS.IoT.Works
{
    public static class WateringWork
    {
        private static double DrynessIndex(SensorsDataRecord record)
        {
            return (100 - record.Moisture) * 0.5 + record.Temperature * 0.5 + record.Light * 0.1 - record.Humidity * 0.1;
        }
        public static void Start()
        {
            try
            {
                Debug.WriteLine("Start watering work");
                if(MainDataFile.Instance.GetRecords().LastOrDefault() is not SensorsDataRecord record)
                {
                    Debug.WriteLine("No sensors data got. Can`t continue watering");
                    return;
                }
                double dryness = DrynessIndex(record);
                Debug.WriteLine($"Dryness: {dryness}");
                if(dryness >= 50.0)
                {
                    Debug.WriteLine("Turn on watering for 5s");
                    using GpioPin pin = new GpioController().OpenPin(27, PinMode.Output);
                    pin.Write(PinValue.High);
                    Thread.Sleep(5000);
                    pin.Write(PinValue.Low);
                    Debug.WriteLine("Watering turned off");
                }
                else
                {
                    Debug.WriteLine("Dryness too low to continue work");
                }
                Debug.WriteLine("Watering work finished");
            }
            catch { }
        }
    }
}