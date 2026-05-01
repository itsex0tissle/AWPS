using AWPS.IoT.MsgPack.Models;
using nanoFramework.MessagePack;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Stream;
using nanoFramework.MessagePack.Converters;

namespace AWPS.IoT.MsgPack.Converters
{
    public sealed class WifiConnectionResultRecordConverter : IConverter
    {
        #region Static
        private static void InternalWrite(WifiConnectionResultRecord obj, IMessagePackWriter writer)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            IConverter bool_converter = ConverterContext.GetConverter(typeof(bool));
            bool_converter.Write(obj.Connected, writer);
            str_converter.Write(obj.Message, writer);
        }
        private static WifiConnectionResultRecord InternalRead(IMessagePackReader reader)
        {
            IConverter str_converter = ConverterContext.GetConverter(typeof(string));
            IConverter bool_converter = ConverterContext.GetConverter(typeof(bool));
            return new WifiConnectionResultRecord()
            {
                Connected = (bool)bool_converter.Read(reader)!,
                Message = (string)str_converter.Read(reader)!
            };
        }
        #endregion

        #region IConverter
        public void Write(object? obj, [NotNull] IMessagePackWriter writer)
        {
            WifiConnectionResultRecordConverter.InternalWrite((WifiConnectionResultRecord)obj!, writer);
        }
        public object? Read([NotNull] IMessagePackReader reader)
        {
            return WifiConnectionResultRecordConverter.InternalRead(reader);
        }
        #endregion
    }
}