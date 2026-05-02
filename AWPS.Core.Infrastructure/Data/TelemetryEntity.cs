namespace AWPS.Core.Infrastructure.Data;

public sealed class TelemetryEntity
{
    //Value properties
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public long Timestamp { get; set; } = 0;
    public byte Light { get; set; } = 0;
    public byte Moisture { get; set; } = 0;
    public byte Humidity { get; set; } = 0;
    public sbyte Temperature { get; set; } = 0;
    public string DeviceProfileId { get; set; } = "";

    //Navigation properties
    public DeviceProfileEntity? DeviceProfile { get; set; }
}