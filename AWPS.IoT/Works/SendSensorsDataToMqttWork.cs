using System;
using AWPS.IoT.Files;
using AWPS.IoT.Services;
using AWPS.IoT.MqttInteraction;

namespace AWPS.IoT.Works
{
    public static class SendSensorsDataToMqttWork
    {
        public static void Start(int timeout = 10000, int retry = 5)
        {
            Logger.LogInfo($"{nameof(SendSensorsDataToMqttWork)} started");
            try
            {
                if(AccountFile.Record.DeviceProfileId is not string device_profile_id)
                {
                    Logger.LogWarning("No device profile setted. Can`t continue work");
                    return;
                }
                MqttInteractor interactor = new();
                byte[] request = TelemetryFile.Serialize();
                if(interactor.SendConfirm($"{device_profile_id}/telemetry/request", $"{device_profile_id}/telemetry/response", request, timeout, retry) is true)
                {
                    TelemetryFile.Reset();
                }
            }
            catch(Exception exception)
            {
                Logger.LogException(exception);
            }
            Logger.LogInfo($"{nameof(SendSensorsDataToMqttWork)} finished");
        }
    }
}