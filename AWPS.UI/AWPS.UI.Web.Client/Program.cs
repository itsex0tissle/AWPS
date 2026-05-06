using ApexCharts;
using AWPS.UI.Shared.Helpers;
using AWPS.UI.Shared.Services;
using AWPS.UI.Web.Client.Services;
using AWPS.Core.Infrastructure.MsgPack;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddApexCharts();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton(provider => MsgPackContextProvider.Instance);
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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApplicationDbInteractor, ApplicationDbInteractor>();
builder.Services.AddScoped(provider => new TelemetryHubClientService("https://localhost:7022/telemetry-hub"));
await builder.Build().RunAsync();