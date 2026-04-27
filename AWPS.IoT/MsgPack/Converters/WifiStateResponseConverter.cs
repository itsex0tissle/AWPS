using AWPS.IoT.MsgPack.Models;
using nanoFramework.MessagePack;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Converters;

namespace AWPS.IoT.MsgPack.Converters
{
    public sealed class WifiStateResponseConverter : IConverter
    {
        #region Static
        private static void InternalWrite(WifiStateResponse obj, IMessagePackWriter writer)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            IConverter bool_converter = ConverterContext.GetConverter(typeof(bool));
            str_converter.Write(obj.SSID, writer);
            bool_converter.Write(obj.Connected, writer);
        }
        private static WifiStateResponse InternalRead(IMessagePackReader reader)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            IConverter bool_converter = ConverterContext.GetConverter(typeof(bool));
            return new WifiStateResponse()
            {
                SSID = (string)str_converter.Read(reader)!,
                Connected = (bool)bool_converter.Read(reader)!
            };
        }
        #endregion

        #region IConverter
        public void Write(object? obj, [NotNull] IMessagePackWriter writer)
        {
            WifiStateResponseConverter.InternalWrite((WifiStateResponse)obj!, writer);
        }
        public object? Read([NotNull] IMessagePackReader reader)
        {
            return WifiStateResponseConverter.InternalRead(reader);
        }
        #endregion
    }
}