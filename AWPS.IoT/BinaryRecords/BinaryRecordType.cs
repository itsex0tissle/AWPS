#nullable enable

namespace AWPS.IoT.BinaryRecords
{
    public enum BinaryRecordType : byte
    {
        None,
        MeasuringData,
        GetWifiStatusResponse,
        PostWifiRequest,
        PostWifiResponse,
    }
}