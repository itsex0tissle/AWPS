namespace AWPS.UI.Shared.Models;

public sealed record class MeasuringDataModel(DateTime Timestamp, byte Light, byte Moisture, byte Humidity, sbyte Temperature);