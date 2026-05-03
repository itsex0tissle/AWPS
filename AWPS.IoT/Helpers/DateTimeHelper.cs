using System;

namespace AWPS.IoT.Helpers
{
    public static class DateTimeHelper
    {
        public static bool UtcNowValid
        {
            get => DateTime.UtcNow.Year >= 2026;
        }
    }
}