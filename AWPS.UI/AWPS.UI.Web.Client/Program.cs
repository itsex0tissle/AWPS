using ApexCharts;
using ProGaudi.MsgPack.Light;
using AWPS.UI.Shared.Services;
using AWPS.UI.Web.Client.Services;
using AWPS.UI.Shared.MsgPack.Converters;
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
    return context;
});
await builder.Build().RunAsync();