using AWPS.UI.Shared.Models;
using ProGaudi.MsgPack.Light;
using System.Diagnostics.CodeAnalysis;

namespace AWPS.UI.Shared.MsgPack;

public sealed class WifiStateResponseConverter : IMsgPackConverter<WifiStateResponse>
{
    #region Instance
    private IMsgPackConverter<string> StrConverter { get; set; } = null!; //Init by Initialize(MsgPackContext context)
    private IMsgPackConverter<bool> BoolConverter { get; set; } = null!; //Init by Initialize(MsgPackContext context)
    #endregion  

    #region IConverter
    public void Initialize(MsgPackContext context)
    {
        StrConverter = context.GetConverter<string>();
        BoolConverter = context.GetConverter<bool>();
    }
    public void Write(WifiStateResponse obj, [NotNull] IMsgPackWriter writer)
    {
        StrConverter.Write(obj.SSID, writer);
        BoolConverter.Write(obj.Connected, writer);
    }
    public WifiStateResponse Read([NotNull] IMsgPackReader reader)
    {
        return new WifiStateResponse()
        {
            SSID = StrConverter.Read(reader),
            Connected = BoolConverter.Read(reader)
        };
    }
    #endregion
}