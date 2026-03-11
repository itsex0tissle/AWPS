using AWPS.IoT.Server.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AWPS.IoT.Server.EFCore;

public sealed class ApplicationDatabase(DbContextOptions<ApplicationDatabase> options) : DbContext(options)
{
    public DbSet<MeasuringDataModel> MeasuringDataSet { get; init; }
}