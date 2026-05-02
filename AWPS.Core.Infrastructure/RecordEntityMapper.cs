using AWPS.Core.Infrastructure.Data;
using AWPS.Core.Infrastructure.MsgPack.Models;

namespace AWPS.Core.Infrastructure;

public static class RecordEntityMapper
{
    public static TelemetryEntity ToEntity(this TelemetryRecord instance, string deviceProfileId)
    {
        return new TelemetryEntity()
        {
            Timestamp = instance.Timestamp,
            Light = instance.Light,
            Moisture = instance.Moisture,
            Humidity = instance.Humidity,
            Temperature = instance.Temperature,
            DeviceProfileId = deviceProfileId,
        };
    }
    public static TelemetryRecord ToRecord(this TelemetryEntity instance)
    {
        return new TelemetryRecord()
        {
            Timestamp = instance.Timestamp,
            Light = instance.Light,
            Moisture = instance.Moisture,
            Humidity = instance.Humidity,
            Temperature = instance.Temperature,
        };
    }
}