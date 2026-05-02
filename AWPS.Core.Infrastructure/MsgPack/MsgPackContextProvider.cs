using ProGaudi.MsgPack.Light;
using AWPS.Core.Infrastructure.MsgPack.Models;
using AWPS.Core.Infrastructure.MsgPack.Converters;

namespace AWPS.Core.Infrastructure.MsgPack;

public static class MsgPackContextProvider
{
    public static MsgPackContext Instance
    {
        get
        {
            if(field is null)
            {
                field = new MsgPackContext();
                field.RegisterConverter(new TelemetryConverter());
                field.RegisterConverter(new ArrayConverter<TelemetryRecord>());
            }
            return field;
        }
    }
}