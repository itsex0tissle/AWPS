using AWPS.UI.Shared.Models;
using ProGaudi.MsgPack.Light;
using System.Diagnostics.CodeAnalysis;

namespace AWPS.UI.Shared.MsgPack;

public sealed class WifiAvailableNetworkResponseArrayConverter : IMsgPackConverter<WifiAvailableNetworkResponse[]>
{
    #region Instance
    private IMsgPackConverter<WifiAvailableNetworkResponse> WifiAvailableNetworkResponseConverter { get; set; } = null!;  //Init by Initialize(MsgPackContext context)
    #endregion

    #region IConverter
    public void Initialize(MsgPackContext context)
    {
        WifiAvailableNetworkResponseConverter = context.GetConverter<WifiAvailableNetworkResponse>();
    }
    public void Write(WifiAvailableNetworkResponse[] obj, [NotNull] IMsgPackWriter writer)
    {
        writer.WriteArrayHeader((uint)obj.Length);
        foreach(WifiAvailableNetworkResponse response in obj)
        {
            WifiAvailableNetworkResponseConverter.Write(response, writer);
        }
    }
    public WifiAvailableNetworkResponse[] Read([NotNull] IMsgPackReader reader)
    {
        var array = new WifiAvailableNetworkResponse[reader.ReadArrayLength() ?? throw new InvalidDataException()];
        for(int index = 0; index < array.Length; index++)
        {
            array[index] = WifiAvailableNetworkResponseConverter.Read(reader);
        }
        return array;
    }
    #endregion
}