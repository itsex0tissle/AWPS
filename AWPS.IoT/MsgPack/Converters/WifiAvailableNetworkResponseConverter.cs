using AWPS.IoT.MsgPack.Models;
using nanoFramework.MessagePack;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Converters;

namespace AWPS.IoT.MsgPack.Converters
{
    public sealed class WifiAvailableNetworkResponseConverter : IConverter
    {
        #region Static
        private static void InternalWrite(WifiAvailableNetworkResponse obj, IMessagePackWriter writer)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            IConverter byte_converter = ConverterContext.GetConverter(typeof(byte));
            str_converter.Write(obj.SSID, writer);
            byte_converter.Write(obj.SignalBars, writer);
        }
        private static WifiAvailableNetworkResponse InternalRead(IMessagePackReader reader)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            IConverter byte_converter = ConverterContext.GetConverter(typeof(byte));
            return new WifiAvailableNetworkResponse()
            {
                SSID = (string)str_converter.Read(reader)!,
                SignalBars = (byte)byte_converter.Read(reader)!
            };
        }
        #endregion

        #region IConverter
        public void Write(object? obj, [NotNull] IMessagePackWriter writer)
        {
            WifiAvailableNetworkResponseConverter.InternalWrite((WifiAvailableNetworkResponse)obj!, writer);
        }
        public object? Read([NotNull] IMessagePackReader reader)
        {
            return WifiAvailableNetworkResponseConverter.InternalRead(reader);
        }
        #endregion
    }
}