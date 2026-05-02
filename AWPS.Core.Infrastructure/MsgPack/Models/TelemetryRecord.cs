namespace AWPS.Core.Infrastructure.MsgPack.Models;

public sealed class TelemetryRecord
{
    public long Timestamp { get; set; } = DateTime.UtcNow.Ticks;
    public byte Light { get; set; }
    public byte Moisture { get; set; }
    public byte Humidity { get; set; }
    public sbyte Temperature { get; set; }
}