using MQTTnet;
using AWPS.IoT.Server.Hubs;
using Microsoft.AspNetCore.Mvc;
using AWPS.IoT.Server.Services;
using AWPS.Core.Infrastructure.Data;
using AWPS.Core.Infrastructure.MsgPack;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddDbContextFactory<ApplicationDbContext>();
builder.Services.AddSingleton<MqttClientFactory>();
builder.Services.AddSingleton<MqttServer>();
builder.Services.AddSingleton(provider => MsgPackContextProvider.Instance);
builder.Services.AddSignalR();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();
app.Services.CreateScope().ServiceProvider.GetRequiredService<MqttServer>().StartAsync();
app.MapPost("/device-profile/{id}", async([FromRoute] string id, [FromServices] MqttServer mqttServer) =>
{
    await mqttServer.SubscribeAsync(id);
});
app.MapDelete("/device-profile/{id}", async([FromRoute] string id, [FromServices] MqttServer mqttServer) =>
{
    await mqttServer.UnsubscribeAsync(id);
});
app.MapHub<TelemetryHub>("/telemetry-hub");
app.Run();