using System;
using AWPS.IoT.Works;
using System.Threading;
using System.Diagnostics;

namespace AWPS.IoT
{
    public static class Program
    {   
        public static void Main()
        {
#if DEBUG
            Thread.Sleep(10000); //Allows us to connect external serial port reader
#endif
            try
            {
                Debug.WriteLine("Program started");
                PrepareDeviceWork.Start();
                GatherAndSaveSensorsDataWork.Start();
                WateringWork.Start();
                SendSensorsDataToMqttWork.Start();
            }
            catch(Exception exc)
            {
                Debug.WriteLine($"Program failed: {exc}");
            }
            Helper.EnterDeepSleep(TimeSpan.FromSeconds(30));
        }
    }
}