using System.ComponentModel.DataAnnotations;
using AWPS.IoT.Server.BinaryRecords.Measuring;

namespace AWPS.IoT.Server.EFCore.Models;

public sealed class MeasuringDataModel
{
    #region Static
    public static MeasuringDataModel CreateFromRecord(MeasuringDataRecord record)
    {
        return new MeasuringDataModel()
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