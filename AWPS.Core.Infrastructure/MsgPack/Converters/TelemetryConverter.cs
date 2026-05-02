using ProGaudi.MsgPack.Light;
using System.Diagnostics.CodeAnalysis;
using AWPS.Core.Infrastructure.MsgPack.Models;

namespace AWPS.Core.Infrastructure.MsgPack.Converters;

public sealed class TelemetryConverter : IMsgPackConverter<TelemetryRecord>
{
    #region Instance
    private IMsgPackConverter<long> LongConverter { get; set; } = null!;  //Init by Initialize(MsgPackContext context)
    private IMsgPackConverter<byte> ByteConverter { get; set; } = null!;  //Init by Initialize(MsgPackContext context)
    private IMsgPackConverter<sbyte> SByteConverter { get; set; } = null!;  //Init by Initialize(MsgPackContext context)
    #endregion

    #region IMsgPackConverter<TelemetryRecord>
    public void Initialize(MsgPackContext context)
    {
        LongConverter = context.GetConverter<long>();
        ByteConverter = context.GetConverter<byte>();
        SByteConverter = context.GetConverter<sbyte>();
    }
    public void Write(TelemetryRecord obj, [NotNull] IMsgPackWriter writer)
    {
        LongConverter.Write(obj.Timestamp, writer);
        ByteConverter.Write(obj.Light, writer);
        ByteConverter.Write(obj.Moisture, writer);
        ByteConverter.Write(obj.Humidity, writer);
        SByteConverter.Write(obj.Temperature, writer);
    }
    public TelemetryRecord Read([NotNull] IMsgPackReader reader)
    {
        return new TelemetryRecord()
        {
            Timestamp = LongConverter.Read(reader),
            Light = ByteConverter.Read(reader),
            Moisture = ByteConverter.Read(reader),
            Humidity = ByteConverter.Read(reader),
            Temperature = SByteConverter.Read(reader),
        };
    }
    #endregion
}