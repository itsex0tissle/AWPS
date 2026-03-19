using System;

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
    }
}