using MQTTnet;
using System.Buffers;
using System.Text.Json;
using MQTTnet.Protocol;
using MQTTnet.Formatter;
using AWPS.IoT.Server.EFCore;
using AWPS.IoT.Server.SignalR;
using Microsoft.AspNetCore.SignalR;
using AWPS.IoT.Server.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using AWPS.IoT.Server.BinaryRecords;
using System.Security.Authentication;
using AWPS.IoT.Server.BinaryRecords.Measuring;
using System.Security.Cryptography.X509Certificates;

namespace AWPS.IoT.Server.MqttInteraction;

public sealed class MqttInteractor
{
    private IMqttClient Client { get; set; }
    private MqttClientOptions ClientOptions { get; set; }
    private IHubContext<ClientHub> SignalRHub { get; set; }
    private IDbContextFactory<ApplicationDatabase> DatabaseFactory { get; set; }

    public MqttInteractor(MqttClientFactory factory, IDbContextFactory<ApplicationDatabase> database_factory, IHubContext<ClientHub> signalr_hub)
    {
        MqttClientOptionsBuilder builder = factory.CreateClientOptionsBuilder();
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
        Client = factory.CreateMqttClient();
        DatabaseFactory = database_factory;
        SignalRHub = signalr_hub;
    }

    public async Task StartAsync()
    {
        while(true)
        {
            try
            {
                if(Client.IsConnected is false)
                {
                    MqttClientConnectResult result = await Client.ConnectAsync(ClientOptions);
                    if(result.ResultCode is MqttClientConnectResultCode.Success)
                    {
                        Console.WriteLine("Mqtt client connected");
                    }
                    else
                    {
                        Console.WriteLine($"Mqtt client connection failed: {result.ResultCode}");
                        continue;
                    }
                }
                Client.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;
                await Client.SubscribeAsync("measuring-data/request", MqttQualityOfServiceLevel.AtLeastOnce);
                return;

                async Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs event_args)
                {
                    if(event_args.ApplicationMessage.Topic is "measuring-data/request")
                    {
                        try
                        {
                            MeasuringDataModel[] models = BinaryRecord.DeserializeEnumerable(event_args.ApplicationMessage.Payload.ToArray()).OfType<MeasuringDataRecord>().Select(MeasuringDataModel.CreateFromRecord).ToArray();
                            Console.WriteLine($"Models: {JsonSerializer.Serialize(models)}");
                            await using(ApplicationDatabase database = DatabaseFactory.CreateDbContext())
                            {
                                foreach(MeasuringDataModel model in models)
                                {
                                    if(database.MeasuringDataSet.AsNoTracking().FirstOrDefault(db_model => db_model.Timestamp == model.Timestamp) is not null)
                                    {
                                        database.MeasuringDataSet.Update(model);
                                    }
                                    else
                                    {
                                        database.MeasuringDataSet.Add(model);
                                    }
                                }
                                database.SaveChanges();
                            }
                            await Client.PublishStringAsync("measuring-data/response", "true", MqttQualityOfServiceLevel.AtLeastOnce);
                            await SignalRHub.Clients.All.SendAsync("UpdateClient");
                        }
                        catch(Exception exc)
                        {
                            Console.WriteLine(exc);
                            await Client.PublishStringAsync("measuring-data/response", "false", MqttQualityOfServiceLevel.AtLeastOnce);
                        }
                    }
                }
            }
            catch { }
        }
    }
}