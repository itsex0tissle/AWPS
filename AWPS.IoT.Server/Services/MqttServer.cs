using MQTTnet;
using System.Buffers;
using MQTTnet.Protocol;
using MQTTnet.Formatter;
using ProGaudi.MsgPack.Light;
using AWPS.IoT.Server.Helpers;
using AWPS.Core.Infrastructure;
using AWPS.IoT.Server.Resources;
using Microsoft.EntityFrameworkCore;
using AWPS.Core.Infrastructure.Data;
using System.Security.Authentication;
using AWPS.Core.Infrastructure.MsgPack.Models;
using System.Security.Cryptography.X509Certificates;

namespace AWPS.IoT.Server.Services;

public sealed class MqttServer
{
    private IMqttClient MqttClient { get; set; }
    private MsgPackContext MsgPackContext { get; set; }
    private MqttClientOptions ClientOptions { get; set; }
    private IDbContextFactory<ApplicationDbContext> DatabaseFactory { get; set; }

    public MqttServer(MqttClientFactory mqttFactory, IDbContextFactory<ApplicationDbContext> databaseFactory, MsgPackContext msgPackContext)
    {
        MqttClientOptionsBuilder builder = mqttFactory.CreateClientOptionsBuilder();
        builder.WithTcpServer(MqttResources.ServerUrl, 8883);
        builder.WithTlsOptions(static void(MqttClientTlsOptionsBuilder builder) =>
        {
            builder.UseTls();
            builder.WithSslProtocols(SslProtocols.Tls12 | SslProtocols.Tls13);
            builder.WithClientCertificates([X509CertificateLoader.LoadCertificate(MqttResources.ServerCertificate)]);
        });
        builder.WithClientId(Guid.NewGuid().ToString());
        builder.WithCleanSession();
        builder.WithCredentials(MqttResources.UserName, MqttResources.Password);
        builder.WithKeepAlivePeriod(TimeSpan.FromMinutes(1));
        builder.WithProtocolVersion(MqttProtocolVersion.V310);
        ClientOptions = builder.Build();
        MqttClient = mqttFactory.CreateMqttClient();
        MsgPackContext = msgPackContext;
        DatabaseFactory = databaseFactory;
    }

    private async Task SubscribeAllAsync()
    {
        await using ApplicationDbContext database = await DatabaseFactory.CreateDbContextAsync();
        foreach(string device_profile_id in database.DeviceProfiles.AsNoTracking().Select(p => p.Id))
        {
            await SubscribeAsync(device_profile_id);
        }
    }
    public async void StartAsync()
    {
        while(true)
        {
            try
            {
                if(MqttClient.IsConnected is false)
                {
                    MqttClientConnectResult result = await MqttClient.ConnectAsync(ClientOptions);
                    if(result.ResultCode is MqttClientConnectResultCode.Success)
                    {
                        Console.WriteLine("Mqtt client connected");
                        MqttClient.ApplicationMessageReceivedAsync += HandleMessagesAsync;
                        await SubscribeAllAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Mqtt client connection failed: {result.ResultCode}");
                        continue;
                    }
                }
                break;

                async Task HandleMessagesAsync(MqttApplicationMessageReceivedEventArgs event_args)
                {
                    string[] topic_parts = event_args.ApplicationMessage.Topic.Split('/');
                    if(topic_parts.Length is not 3)
                    {
                        return;
                    }
                    string device_profile_id = topic_parts[0];
                    string content_type = topic_parts[1];
                    string message_type = topic_parts[2];
                    if(content_type is MqttContentType.Telemetry && message_type is MqttMessageType.Request)
                    {
                        try
                        {
                            TelemetryRecord[] records = MsgPackSerializer.Deserialize<TelemetryRecord[]>(event_args.ApplicationMessage.Payload.ToArray(), MsgPackContext);
                            await using(ApplicationDbContext database = await DatabaseFactory.CreateDbContextAsync())
                            {
                                if(await database.DeviceProfiles.FindAsync(device_profile_id) is not DeviceProfileEntity device_profile)
                                {
                                    return;
                                }
                                foreach(TelemetryRecord record in records)
                                {
                                    if(await database.Telemetry.Where(t => t.DeviceProfileId == device_profile_id && t.Timestamp == record.Timestamp).AnyAsync() is false)
                                    {
                                        await database.Telemetry.AddAsync(record.ToEntity(device_profile_id));
                                    }
                                }
                                device_profile.LastUpdated = DateTime.UtcNow;
                                await database.SaveChangesAsync();
                            }
                            await MqttClient.PublishStringAsync($"{device_profile_id}/{content_type}/{MqttMessageType.Response}", "true", MqttQualityOfServiceLevel.AtLeastOnce);
                        }
                        catch(Exception exc)
                        {
                            Console.WriteLine(exc);
                            await MqttClient.PublishStringAsync($"{device_profile_id}/{content_type}/{MqttMessageType.Response}", "false", MqttQualityOfServiceLevel.AtLeastOnce);
                        }
                    }
                }
            }
            catch { }
        }
        await Task.Run(async void() =>
        {
            while(true)
            {
                if(MqttClient.IsConnected is false)
                {
                    try
                    {
                        await MqttClient.ReconnectAsync();
                        if(MqttClient.IsConnected is true)
                        {
                            await SubscribeAllAsync();
                        }
                    }
                    catch { }
                }
                Thread.Yield();
            }
        });
    }
    public async Task SubscribeAsync(string deviceProfileId)
    {
        await using(ApplicationDbContext database = await DatabaseFactory.CreateDbContextAsync())
        {
            if(await database.DeviceProfiles.FindAsync(deviceProfileId) is null)
            {
                throw new InvalidOperationException($"Device profile not found");
            }
        }
        await MqttClient.SubscribeAsync($"{deviceProfileId}/{MqttContentType.Telemetry}/{MqttMessageType.Request}", MqttQualityOfServiceLevel.AtLeastOnce);
    }
    public async Task UnsubscribeAsync(string deviceProfileId)
    {
        await MqttClient.UnsubscribeAsync($"{deviceProfileId}/{MqttContentType.Telemetry}/{MqttMessageType.Request}");
    }
}