namespace AWPS.IoT.Shared.BinaryRecords
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