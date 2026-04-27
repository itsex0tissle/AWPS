using ProGaudi.MsgPack.Light;
using AWPS.UI.Shared.MsgPack.Models;
using System.Diagnostics.CodeAnalysis;

namespace AWPS.UI.Shared.MsgPack.Converters
{
    public sealed class WifiAvailableNetworkResponseConverter : IMsgPackConverter<WifiAvailableNetworkResponse>
    {
        #region Instance
        private IMsgPackConverter<string> StrConverter { get; set; } = null!; //Init by Initialize(MsgPackContext context)
        private IMsgPackConverter<byte> ByteConverter { get; set; } = null!; //Init by Initialize(MsgPackContext context)
        #endregion  

        #region IConverter
        public void Initialize(MsgPackContext context)
        {
            StrConverter = context.GetConverter<string>();
            ByteConverter = context.GetConverter<byte>();
        }
        public void Write(WifiAvailableNetworkResponse obj, [NotNull] IMsgPackWriter writer)
        {
            StrConverter.Write(obj.SSID, writer);
            ByteConverter.Write(obj.SignalBars, writer);
        }
        public WifiAvailableNetworkResponse Read([NotNull] IMsgPackReader reader)
        {
            return new WifiAvailableNetworkResponse()
            {
                SSID = StrConverter.Read(reader),
                SignalBars = ByteConverter.Read(reader)
            };
        }
        #endregion
    }
}