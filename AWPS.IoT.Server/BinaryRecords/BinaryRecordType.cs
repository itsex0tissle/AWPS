namespace AWPS.IoT.Server.BinaryRecords;

public enum BinaryRecordType : byte
{
    None,
    MeasuringData,
    GetWifiStatusResponse,
    PostWifiRequest,
    PostWifiResponse,
}