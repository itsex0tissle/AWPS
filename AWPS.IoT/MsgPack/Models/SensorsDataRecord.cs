using System;

namespace AWPS.IoT.MsgPack.Models
{
    public sealed class SensorsDataRecord
    {
        public long Timestamp { get; set; } = DateTime.UtcNow.Ticks;
        public byte Light { get; set; }
        public byte Moisture { get; set; }
        public byte Humidity { get; set; }
        public sbyte Temperature { get; set; }
    }
}