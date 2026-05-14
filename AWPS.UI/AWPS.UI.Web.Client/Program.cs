using ApexCharts;
using System.Text.Json;
using AWPS.UI.Shared.Helpers;
using AWPS.UI.Shared.Services;
using AWPS.UI.Web.Client.Services;
using AWPS.Core.Infrastructure.MsgPack;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json.Serialization;

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
builder.Services.AddSingleton(provider => new JsonSerializerOptions()
{
    PropertyNameCaseInsensitive = true,
    ReferenceHandler = ReferenceHandler.Preserve
});
await builder.Build().RunAsync();