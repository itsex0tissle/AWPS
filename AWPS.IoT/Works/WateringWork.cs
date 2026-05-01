using System;
using AWPS.IoT.Files;
using System.Threading;
using System.Device.Gpio;
using System.Diagnostics;
using AWPS.IoT.MsgPack.Models;

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
                Debug.WriteLine("WateringWork started");
                if(SensorsDataFile.Count is 0)
                {
                    Debug.WriteLine("No sensors data available. Can`t continue watering");
                    return;
                }
                SensorsDataRecord record = SensorsDataFile.Get(SensorsDataFile.Count - 1);
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
                Debug.WriteLine("WateringWork finished");
            }
            catch(Exception exc)
            {
                Debug.WriteLine($"WateringWork failed: {exc}");
            }
        }
    }
}