using AWPS.IoT.MsgPack.Models;
using nanoFramework.MessagePack;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Converters;

namespace AWPS.IoT.MsgPack.Converters
{
    public sealed class WifiCredentialsRequestConverter : IConverter
    {
        #region Static
        private static void InternalWrite(WifiCredentialsRequest obj, IMessagePackWriter writer)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            str_converter.Write(obj.SSID, writer);
            str_converter.Write(obj.Password, writer);
        }
        private static WifiCredentialsRequest InternalRead(IMessagePackReader reader)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            return new WifiCredentialsRequest()
            {
                SSID = (string)str_converter.Read(reader)!,
                Password = (string)str_converter.Read(reader)!
            };
        }
        #endregion

        #region IConverter
        public void Write(object? obj, [NotNull] IMessagePackWriter writer)
        {
            WifiCredentialsRequestConverter.InternalWrite((WifiCredentialsRequest)obj!, writer);
        }
        public object? Read([NotNull] IMessagePackReader reader)
        {
            return WifiCredentialsRequestConverter.InternalRead(reader);
        }
        #endregion
    }
}