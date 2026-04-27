using AWPS.IoT.MsgPack.Models;
using nanoFramework.MessagePack;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Converters;

namespace AWPS.IoT.MsgPack.Converters
{
    public sealed class WifiConnectionResultResponseConverter : IConverter
    {
        #region Static
        private static void InternalWrite(WifiConnectionResultResponse obj, IMessagePackWriter writer)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            IConverter bool_converter = ConverterContext.GetConverter(typeof(bool));
            bool_converter.Write(obj.Connected, writer);
            str_converter.Write(obj.Message, writer);
        }
        private static WifiConnectionResultResponse InternalRead(IMessagePackReader reader)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            IConverter bool_converter = ConverterContext.GetConverter(typeof(bool));
            return new WifiConnectionResultResponse()
            {
                Connected = (bool)bool_converter.Read(reader)!,
                Message = (string)str_converter.Read(reader)!
            };
        }
        #endregion

        #region IConverter
        public void Write(object? obj, [NotNull] IMessagePackWriter writer)
        {
            WifiConnectionResultResponseConverter.InternalWrite((WifiConnectionResultResponse)obj!, writer);
        }
        public object? Read([NotNull] IMessagePackReader reader)
        {
            return WifiConnectionResultResponseConverter.InternalRead(reader);
        }
        #endregion
    }
}