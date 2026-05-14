namespace AWPS.Core.Infrastructure.Data;

public sealed class DeviceProfileEntity
{
    //Value properties
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string Name { get; set; } = "Untitled";
    public DeviceSettingsEntity DeviceSettings { get; set; } = new();
    public string UserId { get; set; } = "";

    //Navigation properties
    public ApplicationUserEntity? User { get; set; }
    public List<TelemetryEntity>? Telemetry { get; set; }
}