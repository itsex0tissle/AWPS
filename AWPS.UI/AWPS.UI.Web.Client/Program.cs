using ApexCharts;
using AWPS.UI.Shared.MsgPack;
using AWPS.UI.Shared.Helpers;
using ProGaudi.MsgPack.Light;
using AWPS.UI.Shared.Services;
using AWPS.UI.Web.Client.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<IApplicationDatabaseInteractor, ApplicationDatabaseInteractor>();
builder.Services.AddScoped(static HubConnection(IServiceProvider provider) =>
{
    return new HubConnectionBuilder().WithUrl("https://localhost:7022/client-hub").WithAutomaticReconnect().Build();
});
builder.Services.AddApexCharts();
builder.Services.AddHttpClient();
builder.Services.AddSingleton(provider =>
{
    MsgPackContext context = new();
    context.RegisterConverter(new WifiStateResponseConverter());
    context.RegisterConverter(new WifiCredentialsRequestConverter());
    context.RegisterConverter(new WifiConnectionResultResponseConverter());
    context.RegisterConverter(new WifiAvailableNetworkResponseConverter());
    context.RegisterConverter(new WifiAvailableNetworkResponseArrayConverter());
    return context;
});
builder.Services.AddKeyedScoped(HttpClientKey.Server, (provider, key) => new HttpClient()
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.AddKeyedScoped(HttpClientKey.Device, (provider, key) => new HttpClient()
{
    BaseAddress = new Uri("http://192.168.4.1:80")
});
builder.Services.AddScoped<IServerInteractor, ServerInteractor>();
builder.Services.AddScoped<IDeviceInteractor, DeviceInteractor>();
await builder.Build().RunAsync();