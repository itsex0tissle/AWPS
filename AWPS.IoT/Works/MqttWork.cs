using AWPS.IoT.BinaryFiles;
using AWPS.IoT.MqttInteraction;

namespace AWPS.IoT.Works
{
    public static class MqttWork
    {
        public static void Start(int timeout = 10000, int retry = 5)
        {
            try
            {
                MqttInteractor interactor = new();
                byte[] request = MainDataFile.Instance.Serialize();
                if(interactor.SendConfirm("sensors-data/request", "sensors-data/response", request, timeout, retry) is true)
                {
                    MainDataFile.Instance.Reset();
                }
            }
            catch { }
        }
    }
}