using AWPS.IoT.MsgPack.Models;
using nanoFramework.MessagePack;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Converters;

namespace AWPS.IoT.MsgPack.Converters
{
    public sealed class TelemetryConverter : IConverter
    {
        #region Static
        private static void InternalWrite(TelemetryRecord obj, IMessagePackWriter writer)
        {
            IConverter long_converter = ConverterContext.GetConverter(typeof(long));
            IConverter byte_converter = ConverterContext.GetConverter(typeof(byte));
            IConverter sbyte_converter = ConverterContext.GetConverter(typeof(sbyte));
            long_converter.Write(obj.Timestamp, writer);
            byte_converter.Write(obj.Light, writer);
            byte_converter.Write(obj.Moisture, writer);
            byte_converter.Write(obj.Humidity, writer);
            sbyte_converter.Write(obj.Temperature, writer);
        }
        private static TelemetryRecord InternalRead(IMessagePackReader reader)
        {
            IConverter long_converter = ConverterContext.GetConverter(typeof(long));
            IConverter byte_converter = ConverterContext.GetConverter(typeof(byte));
            IConverter sbyte_converter = ConverterContext.GetConverter(typeof(sbyte));
            return new TelemetryRecord()
            {
                Timestamp = (long)long_converter.Read(reader)!,
                Light = (byte)byte_converter.Read(reader)!,
                Moisture = (byte)byte_converter.Read(reader)!,
                Humidity = (byte)byte_converter.Read(reader)!,
                Temperature = (sbyte)sbyte_converter.Read(reader)!,
            };
        }
        #endregion

        #region IConverter
        public void Write(object? obj, [NotNull] IMessagePackWriter writer)
        {
            TelemetryConverter.InternalWrite((TelemetryRecord)obj!, writer);
        }
        public object? Read([NotNull] IMessagePackReader reader)
        {
            return TelemetryConverter.InternalRead(reader);
        }
        #endregion
    }
}