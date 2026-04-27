using ProGaudi.MsgPack.Light;
using AWPS.UI.Shared.MsgPack.Models;
using System.Diagnostics.CodeAnalysis;

namespace AWPS.UI.Shared.MsgPack.Converters
{
    public sealed class WifiConnectionResultResponseConverter : IMsgPackConverter<WifiConnectionResultResponse>
    {
        #region Instance
        private IMsgPackConverter<bool> BoolConverter { get; set; } = null!; //Init by Initialize(MsgPackContext context)
        private IMsgPackConverter<string> StrConverter { get; set; } = null!; //Init by Initialize(MsgPackContext context)
        #endregion  

        #region IConverter
        public void Initialize(MsgPackContext context)
        {
            BoolConverter = context.GetConverter<bool>();
            StrConverter = context.GetConverter<string>();
        }
        public void Write(WifiConnectionResultResponse obj, [NotNull] IMsgPackWriter writer)
        {
            BoolConverter.Write(obj.Connected, writer);
            StrConverter.Write(obj.Message, writer);
        }
        public WifiConnectionResultResponse Read([NotNull] IMsgPackReader reader)
        {
            return new WifiConnectionResultResponse()
            {
                Connected = BoolConverter.Read(reader),
                Message = StrConverter.Read(reader)
            };
        }
        #endregion
    }
}