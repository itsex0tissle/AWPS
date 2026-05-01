using System;
using AWPS.IoT.Files;
using System.Diagnostics;
using AWPS.IoT.MqttInteraction;

namespace AWPS.IoT.Works
{
    public static class SendSensorsDataToMqttWork
    {
        public static void Start(int timeout = 10000, int retry = 5)
        {
            try
            {
                Debug.WriteLine("SendSensorsDataToMqttWork started");
                MqttInteractor interactor = new();
                byte[] request = SensorsDataFile.Serialize();
                if(interactor.SendConfirm("sensors-data/request", "sensors-data/response", request, timeout, retry) is true)
                {
                    SensorsDataFile.Reset();
                }
                Debug.WriteLine("SendSensorsDataToMqttWork finished");
            }
            catch(Exception exc)
            {
                Debug.WriteLine($"SendSensorsDataToMqttWork failed: {exc}");
            }
        }
    }
}