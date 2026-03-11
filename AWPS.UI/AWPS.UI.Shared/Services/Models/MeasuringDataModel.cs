namespace AWPS.UI.Shared.Services.Models;

public sealed record class MeasuringDataModel(DateTime Timestamp, byte Light, byte Moisture, byte Humidity, sbyte Temperature);