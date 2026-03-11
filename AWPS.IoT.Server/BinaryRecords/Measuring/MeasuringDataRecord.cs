namespace AWPS.IoT.Server.BinaryRecords.Measuring;

public sealed class MeasuringDataRecord : BinaryRecord
{
    #region Instance
    public long Timestamp { get; set; } = DateTime.UtcNow.Ticks;
    public byte Light { get; set; } = 0;
    public byte Moisture { get; set; } = 0;
    public byte Humidity { get; set; } = 0;
    public sbyte Temperature { get; set; } = 0;
    #endregion

    #region BinaryRecord
    protected override int ChildByteLength
    {
        get => 12;
    }
    public override BinaryRecordType Type
    {
        get => BinaryRecordType.MeasuringData;
    }
    public override byte Version
    {
        get => 1;
    }

    protected override void SerializeChild(byte[] buffer, int offset)
    {
        BinaryRecord.WriteLong(buffer, ref offset, Timestamp);
        BinaryRecord.WriteByte(buffer, ref offset, Light);
        BinaryRecord.WriteByte(buffer, ref offset, Moisture);
        BinaryRecord.WriteByte(buffer, ref offset, Humidity);
        BinaryRecord.WriteSByte(buffer, ref offset, Temperature);
    }
    protected override void DeserializeChild(byte[] buffer, int offset)
    {
        Timestamp = BinaryRecord.ReadLong(buffer, ref offset);
        Light = BinaryRecord.ReadByte(buffer, ref offset);
        Moisture = BinaryRecord.ReadByte(buffer, ref offset);
        Humidity = BinaryRecord.ReadByte(buffer, ref offset);
        Temperature = BinaryRecord.ReadSByte(buffer, ref offset);
    }
    #endregion
}