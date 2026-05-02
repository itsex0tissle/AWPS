using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AWPS.Core.Infrastructure.Data.Configurations;

public sealed class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUserEntity>
{
    #region IEntityTypeConfiguration<ApplicationUserEntity>
    public void Configure(EntityTypeBuilder<ApplicationUserEntity> builder)
    {
        builder.HasMany(x => x.DeviceProfiles).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
    #endregion
}