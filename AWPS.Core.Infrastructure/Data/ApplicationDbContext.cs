using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AWPS.Core.Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUserEntity>(options)
{
    #region Instance
    public DbSet<DeviceProfileEntity> DeviceProfiles { get; private init; } = null!; //Init by EFCore
    public DbSet<TelemetryEntity> Telemetry { get; private init; } = null!; //Init by EFCore
    #endregion

    #region Base
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AWPS;Trusted_Connection=True;MultipleActiveResultSets=true;");
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    #endregion
}