using System;
using AWPS.IoT.Files;
using AWPS.IoT.Works;
using AWPS.IoT.Services;

namespace AWPS.IoT
{
    public static class Program
    {   
        public static void Main()
        {
            Logger.LogInfo($"{nameof(Program)}.{nameof(Main)} started");
            try
            {
                PrepareDeviceWork.Start();
                GatherAndSaveSensorsDataWork.Start();
                WateringWork.Start();
                SendSensorsDataToMqttWork.Start();
            }
            catch(Exception exc)
            {
                Logger.LogError(exc.ToString());
            }
            finally
            {
                if(DeviceStateFile.Record.WareringInProcess is true)
                {
                    Helper.EnterDeepSleep(TimeSpan.FromMinutes(2));
                }
                Helper.EnterDeepSleep(TimeSpan.FromMinutes(1));
            }
            Logger.LogInfo($"{nameof(Program)}.{nameof(Main)} finished");
        }
    }
}