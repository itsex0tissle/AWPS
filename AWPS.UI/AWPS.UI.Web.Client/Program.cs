using ApexCharts;
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
await builder.Build().RunAsync();