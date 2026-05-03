using AWPS.IoT.MsgPack.Models;
using nanoFramework.MessagePack;
using AWPS.IoT.MsgPack.Converters;

namespace AWPS.IoT.MsgPack
{
    public static class MsgPackContextConfigurator
    {
        public static void Setup()
        {
            ConverterContext.Add(typeof(TelemetryRecord), new TelemetryConverter());
        }
    }
}