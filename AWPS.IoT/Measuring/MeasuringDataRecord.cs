#nullable enable

using System;

namespace AWPS.IoT.Measuring
{
    public sealed class MeasuringDataRecord
    {
        public long Timestamp { get; set; } = DateTime.UtcNow.Ticks;
        public int Light { get; set; } = 0;
        public int Moisture { get; set; } = 0;
        public int Humidity { get; set; } = 0;
        public int Temperature { get; set; } = 0;
    }
}