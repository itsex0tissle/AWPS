using AWPS.UI.Services;
using AWPS.UI.Shared.Services;
using Microsoft.Extensions.Logging;

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
        builder.Services.AddScoped<IApplicationDatabaseInteractor, ApplicationDatabaseInteractor>();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}