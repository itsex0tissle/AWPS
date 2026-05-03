using System;
using AWPS.IoT.Files;
using System.Threading;
using AWPS.IoT.Services;
using System.Device.Gpio;
using AWPS.IoT.MsgPack.Models;

namespace AWPS.IoT.Works
{
    public static class WateringWork
    {
        private static double DrynessIndex(TelemetryRecord record)
        {
            return (100 - record.Moisture) * 0.5 + record.Temperature * 0.5 + record.Light * 0.1 - record.Humidity * 0.1;
        }
        public static void Start()
        {
            Logger.LogInfo($"{nameof(WateringWork)} started");
            try
            {
                if(TelemetryFile.Count is 0)
                {
                    Logger.LogWarning("No telemetry available. Can`t continue work");
                    return;
                }
                TelemetryRecord record = TelemetryFile.Get(TelemetryFile.Count - 1);
                double dryness = DrynessIndex(record);
                Logger.LogInfo($"Dryness: {dryness}");
                if(dryness >= 50.0)
                {
                    Logger.LogInfo("Turn on watering for 5s");
                    using GpioPin pin = new GpioController().OpenPin(27, PinMode.Output);
                    pin.Write(PinValue.High);
                    Thread.Sleep(5000);
                    pin.Write(PinValue.Low);
                    Logger.LogInfo("Watering turned off");
                }
                else
                {
                    Logger.LogWarning("Dryness too low. Can`t continue work");
                }
            }
            catch(Exception exception)
            {
                Logger.LogException(exception);
            }
            Logger.LogInfo($"{nameof(WateringWork)} finished");
        }
    }
}