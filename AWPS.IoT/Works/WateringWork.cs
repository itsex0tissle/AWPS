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
        private static bool NeedWatering(TelemetryRecord record)
        {
            double moisture = record.Moisture;
            //if(moisture < 5)
            //{
            //    //Moisture sensor probably not used or not working
            //    return false;
            //}
            moisture -= (record.Temperature - 20) / 2;
            moisture += (record.Humidity - 50) / 10;
            moisture -= (record.Light - 50) / 10;
            Logger.LogInfo(
                $"Moisture: {record.Moisture}%; " +
                $"Temperature: {record.Temperature}C; " +
                $"Humidity: {record.Humidity}%; " +
                $"Light: {record.Light}%; " +
                $"PivotMoisture: {SettingsFile.Record.MoisturePivot}%; " +
                $"FinalMoisture: {moisture}%"
            );
            return moisture < SettingsFile.Record.MoisturePivot;
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
                if(RequireWatering(record) is true)
                {
                    DeviceStateFile.Record.WateringInProcess = true;
                    DeviceStateFile.Record.WateringCycle += 1;
                    DeviceStateFile.Save();

                    Logger.LogInfo("Turn on watering for 5s");
                    using GpioPin pin = new GpioController().OpenPin(27, PinMode.Output);
                    pin.Write(PinValue.High);
                    Thread.Sleep(5000);
                    pin.Write(PinValue.Low);
                    Logger.LogInfo("Watering turned off");
                }
                else
                {
                    Logger.LogWarning("Watering not required");
                    DeviceStateFile.Reset();
                }

                static bool RequireWatering(TelemetryRecord record)
                {
                    if(DeviceStateFile.Record.WateringInProcess is true && DeviceStateFile.Record.WateringCycle <= SettingsFile.Record.WateringCycleCount)
                    {
                        return true;
                    }
                    return NeedWatering(record);
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