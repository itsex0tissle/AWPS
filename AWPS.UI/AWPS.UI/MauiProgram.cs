using ApexCharts;
using AWPS.UI.Helpers;
using AWPS.UI.Services;
using AWPS.UI.Shared.Helpers;
using AWPS.UI.Shared.Services;
using Microsoft.Extensions.Logging;
using AWPS.Core.Infrastructure.MsgPack;
using Microsoft.AspNetCore.Components.Authorization;

namespace AWPS.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(static void(IFontCollection fonts) =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        });
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddApexCharts();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddSingleton(provider => MsgPackContextProvider.Instance);
        builder.Services.AddKeyedSingleton(HttpClientKey.Server, (provider, key) => HttpClientHelper.CreatePlatformHttpClient());
        builder.Services.AddKeyedSingleton(HttpClientKey.Device, (provider, key) =>
        {
            HttpClient client = HttpClientHelper.CreatePlatformHttpClient();
            client.BaseAddress = new Uri("http://192.168.4.1:80");
            return client;
        });
        builder.Services.AddSingleton<MauiAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
        {
            return provider.GetRequiredService<MauiAuthenticationStateProvider>();
        });
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<IAuthService>(provider =>
        {
            return provider.GetRequiredService<AuthService>();
        });
        builder.Services.AddScoped<IServerInteractor, ServerInteractor>();
        builder.Services.AddScoped<IDeviceInteractor, DeviceInteractor>();
        builder.Services.AddScoped<IApplicationDbInteractor, ApplicationDbInteractor>();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}