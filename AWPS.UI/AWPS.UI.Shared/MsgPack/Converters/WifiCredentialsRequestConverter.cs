using ProGaudi.MsgPack.Light;
using AWPS.UI.Shared.MsgPack.Models;
using System.Diagnostics.CodeAnalysis;

namespace AWPS.UI.Shared.MsgPack.Converters
{
    public sealed class WifiCredentialsRequestConverter : IMsgPackConverter<WifiCredentialsRequest>
    {
        #region Instance
        private IMsgPackConverter<string> StrConverter { get; set; } = null!; //Init by Initialize(MsgPackContext context)
        #endregion  

        #region IConverter
        public void Initialize(MsgPackContext context)
        {
            StrConverter = context.GetConverter<string>();
        }
        public void Write(WifiCredentialsRequest obj, [NotNull] IMsgPackWriter writer)
        {
            StrConverter.Write(obj.SSID, writer);
            StrConverter.Write(obj.Password, writer);
        }
        public WifiCredentialsRequest Read([NotNull] IMsgPackReader reader)
        {
            return new WifiCredentialsRequest()
            {
                SSID = StrConverter.Read(reader),
                Password = StrConverter.Read(reader)
            };
        }
        #endregion
    }
}