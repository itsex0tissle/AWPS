using ApexCharts;
using AWPS.UI.Web;
using AWPS.UI.Web.Services;
using AWPS.UI.Shared.Helpers;
using AWPS.UI.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using AWPS.Core.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Authorization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddInteractiveWebAssemblyComponents();
builder.Services.AddApexCharts();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddDbContextFactory<ApplicationDbContext>(lifetime: ServiceLifetime.Scoped);
builder.Services.AddIdentityApiEndpoints<ApplicationUserEntity>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
}).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApplicationDbInteractor, ApplicationDbInteractor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(HttpClientKey.Server, client =>
{
    client.BaseAddress = new Uri("https+http://awps-ui-web");
});
builder.Services.AddHttpClient(HttpClientKey.IotServer, client =>
{
    client.BaseAddress = new Uri("https+http://awps-iot-server");
});
builder.Services.AddScoped<IotServerInteractor>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddScoped(provider => new TelemetryHubClientService($"{builder.Configuration["services:awps-iot-server:https:0"]}/telemetry-hub"));

WebApplication app = builder.Build();
app.MapDefaultEndpoints();
if(app.Environment.IsDevelopment() is true)
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/NotFound", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode().AddInteractiveWebAssemblyRenderMode().AddAdditionalAssemblies([
    typeof(AWPS.UI.Shared._Imports).Assembly,
    typeof(AWPS.UI.Web.Client._Imports).Assembly
]);
app.MapGroup("/identity").MapIdentityApi<ApplicationUserEntity>();
app.MapPost("/identity/logout", async([FromServices] SignInManager<ApplicationUserEntity> manager) =>
{
    await manager.SignOutAsync();
    return Results.Ok();
}).RequireAuthorization();
app.MapGet("/ping", () =>
{
    return Results.Ok();
}).RequireAuthorization();

RouteGroupBuilder appDbGroup = app.MapGroup("/app-db").RequireAuthorization();
appDbGroup.MapGet("/user-email", GetCurrentUser).WithName("GetCurrentUserEmail");
appDbGroup.MapDelete("/delete-current-user", DeleteCurrentUser).WithName("DeleteCurrentUser");
appDbGroup.MapGet("/device-profiles", GetDeviceProfiles).WithName("GetDeviceProfiles");
appDbGroup.MapGet("/device-profile/{device_profile_id}", GetDeviceProfile).WithName("GetDeviceProfile");
appDbGroup.MapPost("/device-profile", CreateDeviceProfile).WithName("CreateDeviceProfile");
appDbGroup.MapPut("/device-profile", UpdateDeviceProfile).WithName("UpdateDeviceProfile");
appDbGroup.MapDelete("/device-profile/{device_profile_id}", DeleteDeviceProfile).WithName("DeleteDeviceProfile");
appDbGroup.MapGet("/telemetry/{device_profile_id}", GetTelemetry).WithName("GetTelemetry");
appDbGroup.MapDelete("/telemetry/{device_profile_id}/clear", ClearTelemetry).WithName("ClearTelemetry");
app.Run();

async Task<IResult> GetCurrentUser([FromServices] IApplicationDbInteractor interactor)
{
    var result = await interactor.GetCurrentUserEmail();
    return result.IsSuccess ? Results.Ok(result.Value) : MapError(result);
}
async Task<IResult> DeleteCurrentUser([FromServices] IApplicationDbInteractor interactor)
{
    var result = await interactor.DeleteCurrentUser();
    return MapError(result);
}
async Task<IResult> GetDeviceProfiles([FromServices] IApplicationDbInteractor interactor)
{
    var result = await interactor.GetDeviceProfiles();
    return result.IsSuccess ? Results.Ok(result.Value) : MapError(result);
}
async Task<IResult> GetDeviceProfile(string device_profile_id, [FromServices] IApplicationDbInteractor interactor)
{
    var result = await interactor.GetDeviceProfile(device_profile_id);
    return result.IsSuccess ? Results.Ok(result.Value) : MapError(result);
}
async Task<IResult> CreateDeviceProfile(string name, [FromServices] IApplicationDbInteractor interactor)
{
    var result = await interactor.CreateDeviceProfile(name);
    return result.IsSuccess ? Results.Created($"/app-db/device-profile/{result.Value.Id}", result.Value) : MapError(result);
}
async Task<IResult> UpdateDeviceProfile([FromBody] DeviceProfileEntity profile, [FromServices] IApplicationDbInteractor interactor)
{
    var result = await interactor.UpdateDeviceProfile(profile);
    return result.IsSuccess ? Results.Ok(result.Value) : MapError(result);
}
async Task<IResult> DeleteDeviceProfile(string device_profile_id, [FromServices] IApplicationDbInteractor interactor)
{
    var result = await interactor.DeleteDeviceProfile(device_profile_id);
    return MapError(result);
}
async Task<IResult> GetTelemetry(string device_profile_id, [FromServices] IApplicationDbInteractor interactor)
{
    var result = await interactor.GetTelemetry(device_profile_id);
    return result.IsSuccess ? Results.Ok(result.Value) : MapError(result);
}
async Task<IResult> ClearTelemetry(string device_profile_id, [FromServices] IApplicationDbInteractor interactor)
{
    var result = await interactor.ClearTelemetry(device_profile_id);
    return MapError(result);
}
IResult MapError(Ardalis.Result.IResult result) => result.Status switch
{
    Ardalis.Result.ResultStatus.Ok => Results.Ok(),
    Ardalis.Result.ResultStatus.NotFound => Results.NotFound(),
    Ardalis.Result.ResultStatus.Unauthorized => Results.Unauthorized(),
    Ardalis.Result.ResultStatus.Error => Results.BadRequest(result.Errors),
    Ardalis.Result.ResultStatus.CriticalError => Results.StatusCode(StatusCodes.Status500InternalServerError),
    _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
};