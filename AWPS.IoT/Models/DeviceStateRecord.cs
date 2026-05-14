namespace AWPS.IoT.Models
{
    public sealed class DeviceStateRecord
    {
        public bool WateringInProcess { get; set; }
        public uint WateringCycle { get; set; }
    }
}