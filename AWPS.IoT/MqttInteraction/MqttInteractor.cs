using System;
using System.Text;
using System.Threading;
using AWPS.IoT.Services;
using System.Collections;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System.Security.Cryptography.X509Certificates;

namespace AWPS.IoT.MqttInteraction
{
    public sealed class MqttInteractor
    {
        private MqttClient Mqtt { get; }

        public MqttInteractor()
        {
            string server_url = MqttResources.GetString(MqttResources.StringResources.ServerUrl);
            X509Certificate cert = new(MqttResources.GetString(MqttResources.StringResources.ServerCertificate));
            Mqtt = new MqttClient(server_url, 8883, true, cert, null, MqttSslProtocols.TLSv1_2);
            Logger.LogInfo("Mqtt client created");
        }

        public bool EnsureConnected(int retry = 5)
        {
            if(Mqtt.IsConnected is true)
            {
                Logger.LogInfo("Mqtt client already connected");
                return true;
            }
            string username = MqttResources.GetString(MqttResources.StringResources.UserName);
            string password = MqttResources.GetString(MqttResources.StringResources.Password);
            return Helper.Retry(delegate()
            {
                string client_id = Guid.NewGuid().ToString();
                MqttReasonCode code = Mqtt.Connect(client_id, username, password, cleanSession: true, keepAlivePeriod: 60);
                if(code is not MqttReasonCode.Success)
                {
                    throw new Exception($"Failed to connect to mqtt: ReasonCode = {code}");
                }
                Logger.LogInfo("Mqtt client connected");
            }, retry);
        }
        public byte[]? SendReceive(string send_topic, string receive_topic, byte[] buffer, int timeout = 10000, int retry = 5)
        {
            Logger.LogInfo($"Send request on topic '{send_topic}'. Receive response on topic '{receive_topic}'");
            byte[]? response = null;
            bool response_got = false;
            Timer timer = new(delegate(object? state)
            {
                Logger.LogWarning($"Receive response on topic '{receive_topic}' timeout");
                response_got = true;
            }, null, timeout, Timeout.Infinite);
            Mqtt.MqttMsgPublishReceived += SaveMessage;
            Logger.LogInfo("Subscribe and publish message");
            bool success = Helper.Retry(delegate()
            {
                if(EnsureConnected(retry) is false)
                {
                    throw new Exception("Can`t connect to MQTT");
                }
                Mqtt.Subscribe(new string[] { receive_topic }, new MqttQoSLevel[] { MqttQoSLevel.AtLeastOnce });
                Mqtt.Publish(send_topic, buffer, "", new ArrayList(), MqttQoSLevel.AtLeastOnce, false);
                Logger.LogInfo("Subscribed and message published");
            }, retry);
            if(success is false)
            {
                Logger.LogWarning("Subscribe and publish message failed");
                timer.Dispose();
                return null;
            }
            while(response_got is false)
            {
                Thread.Sleep(100);
            }
            return response;

            void SaveMessage(object sender, MqttMsgPublishEventArgs event_args)
            {
                Logger.LogInfo($"Response received on topic '{event_args.Topic}' in handler for '{receive_topic}' topic");
                if(event_args.Topic != receive_topic)
                {
                    return;
                }
                Logger.LogInfo("Saving message");
                response = event_args.Message;
                response_got = true;
                timer.Dispose();
            }
        }
        public bool SendConfirm(string send_topic, string receive_topic, byte[] buffer, int timeout = 10000, int retry = 5)
        {
            try
            {
                if(SendReceive(send_topic, receive_topic, buffer, timeout, retry) is byte[] response)
                {
                    bool result = Encoding.UTF8.GetString(response, 0, response.Length) is "true";
                    Logger.LogInfo($"Response on topic '{receive_topic}': {result}");
                    return result;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}