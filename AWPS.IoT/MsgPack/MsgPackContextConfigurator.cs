using AWPS.IoT.MsgPack.Models;
using nanoFramework.MessagePack;
using AWPS.IoT.MsgPack.Converters;

namespace AWPS.IoT.MsgPack
{
    public static class MsgPackContextConfigurator
    {
        public static void Setup()
        {
            ConverterContext.Add(typeof(WifiStateRecord), new WifiStateRecordConverter());
            ConverterContext.Add(typeof(WifiCredentialsRecord), new WifiCredentialsRecordConverter());
            ConverterContext.Add(typeof(WifiConnectionResultRecord), new WifiConnectionResultRecordConverter());
            ConverterContext.Add(typeof(WifiAvailableNetworkRecord), new WifiAvailableNetworkRecordConverter());
            ConverterContext.Add(typeof(SensorsDataRecord), new SensorsDataRecordConverter());
        }
    }
}