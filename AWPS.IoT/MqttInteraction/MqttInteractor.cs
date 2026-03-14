using System;
using System.Text;
using System.Threading;
using AWPS.IoT.Measuring;
using System.Collections;
using System.Diagnostics;
using nanoFramework.M2Mqtt;
using AWPS.IoT.BinaryRecords;
using nanoFramework.M2Mqtt.Messages;
using System.Security.Cryptography.X509Certificates;

namespace AWPS.IoT.MqttInteraction
{
    public static class MqttInteractor
    {
        private static MqttClient CreateClient()
        {
            string server_url = MqttResources.GetString(MqttResources.StringResources.ServerUrl);
            X509Certificate cert = new(MqttResources.GetString(MqttResources.StringResources.ServerCertificate));
            Debug.WriteLine("Create mqtt client");
            MqttClient client = new(server_url, 8883, true, cert, null, MqttSslProtocols.TLSv1_2);
            string client_id = Guid.NewGuid().ToString();
            string username = MqttResources.GetString(MqttResources.StringResources.UserName);
            string password = MqttResources.GetString(MqttResources.StringResources.Password);
            Debug.WriteLine("Connect to mqtt");
            client.Connect(client_id, username, password, cleanSession: true, keepAlivePeriod: 60);
            return client;
        }
        private static bool SendData(MqttClient client, byte[] buffer, string topic)
        {
            Debug.WriteLine($"Send data on '{topic}' topic");
            string response_topic = $"{topic}/response";
            bool response = false;
            bool response_got = false;
            Timer timer = new(state =>
            {
                Debug.WriteLine($"Waiting for response on '{topic}' timeout");
                response_got = true;
            }, null, 20000, Timeout.Infinite);
            client.MqttMsgPublishReceived += WaitForConfirmResponse;
            client.Subscribe(new string[] { response_topic }, new MqttQoSLevel[] { MqttQoSLevel.AtLeastOnce });
            client.Publish($"{topic}/request", buffer, "", new ArrayList(), MqttQoSLevel.AtLeastOnce, false);
            while(response_got is false)
            {
                Thread.Sleep(500);
            }
            return response;

            void WaitForConfirmResponse(object sender, MqttMsgPublishEventArgs event_args)
            {
                try
                {
                    if(event_args.Topic == response_topic)
                    {
                        response = Encoding.UTF8.GetString(event_args.Message, 0, event_args.Message.Length) is "true";
                        Debug.WriteLine($"Response got on {topic}: {response}");
                    }
                }
                finally
                {
                    client.MqttMsgPublishReceived -= WaitForConfirmResponse;
                    timer.Dispose();
                    response_got = true;
                }
            }
        }
        public static void Start()
        {
            for(int retry = 0; retry <= 5; retry++)
            {
                try
                {
                    Debug.WriteLine("Start mqtt interactor");
                    using MqttClient client = CreateClient();
                    SendMeasuringDataFile(client);
                    break;
                }
                catch(Exception exc)
                {
                    Debug.WriteLine($"Mqtt interactor failed due to exception: {exc}");
                }
            }

            static void SendMeasuringDataFile(MqttClient client)
            {
                MeasuringDataFile.Load();
                if(SendData(client, BinaryRecord.SerializeEnumerable(MeasuringDataFile.Records), "measuring-data") is true)
                {
                    MeasuringDataFile.Reset();
                }
            }
        }
    }
}