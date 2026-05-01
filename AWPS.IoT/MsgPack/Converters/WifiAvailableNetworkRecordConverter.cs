using AWPS.IoT.MsgPack.Models;
using nanoFramework.MessagePack;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Converters;

namespace AWPS.IoT.MsgPack.Converters
{
    public sealed class WifiAvailableNetworkRecordConverter : IConverter
    {
        #region Static
        private static void InternalWrite(WifiAvailableNetworkRecord obj, IMessagePackWriter writer)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            IConverter byte_converter = ConverterContext.GetConverter(typeof(byte));
            str_converter.Write(obj.SSID, writer);
            byte_converter.Write(obj.SignalBars, writer);
        }
        private static WifiAvailableNetworkRecord InternalRead(IMessagePackReader reader)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            IConverter byte_converter = ConverterContext.GetConverter(typeof(byte));
            return new WifiAvailableNetworkRecord()
            {
                SSID = (string)str_converter.Read(reader)!,
                SignalBars = (byte)byte_converter.Read(reader)!
            };
        }
        #endregion

        #region IConverter
        public void Write(object? obj, [NotNull] IMessagePackWriter writer)
        {
            WifiAvailableNetworkRecordConverter.InternalWrite((WifiAvailableNetworkRecord)obj!, writer);
        }
        public object? Read([NotNull] IMessagePackReader reader)
        {
            return WifiAvailableNetworkRecordConverter.InternalRead(reader);
        }
        #endregion
    }
}