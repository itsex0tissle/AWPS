using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AWPS.Core.Infrastructure.Data;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    #region IDesignTimeDbContextFactory<ApplicationDbContext>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ApplicationDbContext> options_builder = new();
        return new ApplicationDbContext(options_builder.Options);
    }
    #endregion
}