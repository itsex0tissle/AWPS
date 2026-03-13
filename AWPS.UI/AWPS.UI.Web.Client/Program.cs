using ApexCharts;
using AWPS.UI.Shared.Services;
using AWPS.UI.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<IApplicationDatabaseInteractor, ApplicationDatabaseInteractor>();
builder.Services.AddApexCharts();
builder.Services.AddHttpClient();

await builder.Build().RunAsync();