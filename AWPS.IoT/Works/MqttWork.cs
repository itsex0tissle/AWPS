using System.Diagnostics;
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
                Debug.WriteLine("Start mqtt work");
                MqttInteractor interactor = new();
                byte[] request = MainDataFile.Instance.Serialize();
                if(interactor.SendConfirm("sensors-data/request", "sensors-data/response", request, timeout, retry) is true)
                {
                    MainDataFile.Instance.Reset();
                }
                Debug.WriteLine("Mqtt work finished");
            }
            catch { }
        }
    }
}