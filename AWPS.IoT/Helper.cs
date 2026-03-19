using System;
using System.Diagnostics;
using nanoFramework.Hardware.Esp32;
using System.Diagnostics.CodeAnalysis;

namespace AWPS.IoT
{
    public static class Helper
    {
        public static bool Retry(Action action, int retry = 5)
        {
            for(; retry >= 0; retry--)
            {
                try
                {
                    action();
                    return true;
                }
                catch { }
            }
            return false;
        }
        [DoesNotReturn] public static void EnterDeepSleep(TimeSpan restart_in)
        {
            Debug.WriteLine($"Enter deep sleep. Restart in: {restart_in}");
            Sleep.EnableWakeupByTimer(restart_in);
            Sleep.StartDeepSleep();
            throw new Exception("Impossible exception");
        }
    }
}