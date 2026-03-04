#nullable enable

using System;
using System.Text;
using System.Threading;
using AWPS.IoT.Measuring;
using System.Collections;
using System.Diagnostics;
using nanoFramework.Json;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System.Security.Cryptography.X509Certificates;

namespace AWPS.IoT.MqttInteraction
{
    public static class MqttInteractor
    {
        public static void Start()
        {
            for(int retry = 0; retry <= 5; retry++)
            {
                try
                {
                    Debug.WriteLine("Start mqtt interactor");
                    Timer? timer = null;
                    string server_url = MqttResources.GetString(MqttResources.StringResources.ServerUrl);
                    X509Certificate cert = new(MqttResources.GetString(MqttResources.StringResources.ServerCertificate));
                    Debug.WriteLine("Create mqtt client");
                    using MqttClient client = new(server_url, 8883, true, cert, null, MqttSslProtocols.TLSv1_2);
                    string client_id = Guid.NewGuid().ToString();
                    string username = MqttResources.GetString(MqttResources.StringResources.UserName);
                    string password = MqttResources.GetString(MqttResources.StringResources.Password);
                    Debug.WriteLine("Connect to mqtt");
                    client.Connect(client_id, username, password, cleanSession: true, keepAlivePeriod: 60);
                    MeasuringDataFile.Load();
                    string content = JsonConvert.SerializeObject(MeasuringDataFile.Records);
                    Debug.WriteLine($"Sending MeasureData file to MQTT: {content}");
                    client.MqttMsgPublishReceived += WaitForConfirmResponse;
                    client.Subscribe(new string[] { "measure-data/response" }, new MqttQoSLevel[] { MqttQoSLevel.AtLeastOnce });
                    client.Publish("measure-data/request", Encoding.UTF8.GetBytes(content), "", new ArrayList(), MqttQoSLevel.AtLeastOnce, false);
                    bool response_got = false;
                    bool response = false;
                    Debug.WriteLine("Waiting for response...");
                    timer = new(state =>
                    {
                        Debug.WriteLine("Waiting for response timeout");
                        response_got = true;
                    }, null, 20000, Timeout.Infinite);
                    while(response_got is false)
                    {
                        Thread.Sleep(500);
                    }
                    Debug.WriteLine($"Mqtt response got: {response}");
                    if(response is true)
                    {
                        MeasuringDataFile.Reset();
                    }
                    break;

                    void WaitForConfirmResponse(object sender, MqttMsgPublishEventArgs event_args)
                    {
                        try
                        {
                            if(event_args.Topic is "measure-data/response")
                            {
                                response = Encoding.UTF8.GetString(event_args.Message, 0, event_args.Message.Length) is "true";
                            }
                        }
                        finally
                        {
                            client.MqttMsgPublishReceived -= WaitForConfirmResponse;
                            timer?.Dispose();
                            response_got = true;
                        }
                    }
                }
                catch(Exception exc)
                {
                    Debug.WriteLine($"Mqtt interactor failed due to exception: {exc}");
                }
            }
        }
    }
}