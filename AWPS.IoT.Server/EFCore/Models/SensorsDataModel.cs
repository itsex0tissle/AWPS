using AWPS.IoT.BinaryRecords;
using System.ComponentModel.DataAnnotations;

namespace AWPS.IoT.Server.EFCore.Models;

public sealed class SensorsDataModel
{
    #region Static
    public static SensorsDataModel CreateFromRecord(SensorsDataRecord record)
    {
        return new SensorsDataModel()
        {
            Timestamp = record.Timestamp,
            Light = record.Light,
            Moisture = record.Moisture,
            Humidity = record.Humidity,
            Temperature = record.Temperature
        };
    }
    #endregion

    #region Instance
    [Key] public Guid Guid { get; set; }
    public long Timestamp { get; set; }
    public byte Light { get; set; } = 0;
    public byte Moisture { get; set; } = 0;
    public byte Humidity { get; set; } = 0;
    public sbyte Temperature { get; set; } = 0;
    #endregion
}