using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AWPS.Core.Infrastructure.Data.Configurations;

public sealed class DeviceProfileEntityConfiguration : IEntityTypeConfiguration<DeviceProfileEntity>
{
    #region IEntityTypeConfiguration<DeviceProfileEntity>
    public void Configure(EntityTypeBuilder<DeviceProfileEntity> builder)
    {
        builder.ToTable("DeviceProfiles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.OwnsOne(x => x.DeviceSettings);
        builder.Property(x => x.UserId).IsRequired();
        builder.HasOne(x => x.User).WithMany(u => u.DeviceProfiles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.UserId);
    }
    #endregion
}