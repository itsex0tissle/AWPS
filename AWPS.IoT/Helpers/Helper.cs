using System;
using AWPS.IoT.Services;
using nanoFramework.Hardware.Esp32;
using System.Diagnostics.CodeAnalysis;

namespace AWPS.IoT
{
    public static class Helper
    {
        public static bool Retry(Action action, int retry = 5)
        {
            for(int count = 1; count <= retry; retry++)
            {
                try
                {
                    Logger.AddPrefix($"Attempt {count}");
                    action();
                    Logger.RemoveLastPrefix();
                    return true;
                }
                catch { }
            }
            return false;
        }
        [DoesNotReturn] public static void EnterDeepSleep(TimeSpan restart_in)
        {
            Logger.LogInfo($"Enter deep sleep. Restart in: {restart_in}");
            Sleep.EnableWakeupByTimer(restart_in);
            Sleep.StartDeepSleep();
            throw new Exception("Impossible exception");
        }
    }
}