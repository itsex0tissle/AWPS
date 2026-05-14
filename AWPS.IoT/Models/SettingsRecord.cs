using System;

namespace AWPS.IoT.Models
{
    public sealed class SettingsRecord
    {
        public byte MoisturePivot { get; set; } = 25;
        public uint WateringCycleCount { get; set; } = 5;
        public long GatherTelemetryPeriod { get; set; } = TimeSpan.FromHours(1).Ticks;
    }
}