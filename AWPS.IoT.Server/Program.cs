using MQTTnet;
using AWPS.IoT.Server.EFCore;
using AWPS.IoT.Server.SignalR;
using Microsoft.EntityFrameworkCore;
using AWPS.IoT.Server.MqttInteraction;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<MqttClientFactory>();
builder.Services.AddScoped<MqttInteractor>();
builder.Services.AddDbContextFactory<ApplicationDatabase>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string not found"));
});
builder.Services.AddSignalR();

WebApplication app = builder.Build();
using(IServiceScope scope = app.Services.CreateScope())
{
    Console.WriteLine("Migrating ApplicationDatabase");
    await using(ApplicationDatabase database = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDatabase>>().CreateDbContext())
    {
        database.Database.Migrate();
    }
}
using(IServiceScope scope = app.Services.CreateScope())
{
    Console.WriteLine("Starting MqttInteractor");
    await scope.ServiceProvider.GetRequiredService<MqttInteractor>().StartAsync();
}
app.MapHub<ClientHub>("/client-hub");
app.Run();