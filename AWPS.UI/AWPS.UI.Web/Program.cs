using AWPS.UI.Web;
using AWPS.UI.Web.Services;
using AWPS.IoT.Server.EFCore;
using AWPS.UI.Shared.Services;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddInteractiveWebAssemblyComponents();
builder.Services.AddDbContextFactory<ApplicationDatabase>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string not found"));
});
builder.Services.AddScoped<IApplicationDatabaseInteractor, ApplicationDatabaseInteractor>();

WebApplication app = builder.Build();
if(app.Environment.IsDevelopment() is true)
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode().AddInteractiveWebAssemblyRenderMode().AddAdditionalAssemblies([
    typeof(AWPS.UI.Shared._Imports).Assembly,
    typeof(AWPS.UI.Web.Client._Imports).Assembly
]);
app.Run();