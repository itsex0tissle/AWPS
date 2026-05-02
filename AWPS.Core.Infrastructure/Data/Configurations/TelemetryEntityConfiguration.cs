using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AWPS.Core.Infrastructure.Data.Configurations;

public sealed class TelemetryEntityConfiguration : IEntityTypeConfiguration<TelemetryEntity>
{
    #region IEntityTypeConfiguration<TelemetryEntity>
    public void Configure(EntityTypeBuilder<TelemetryEntity> builder)
    {
        builder.ToTable("SensorsData");
        builder.HasKey(x => new { x.Id, x.Timestamp });
        builder.Property(x => x.Id).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Timestamp).IsRequired();
        builder.Property(x => x.Light).IsRequired();
        builder.Property(x => x.Moisture).IsRequired();
        builder.Property(x => x.Humidity).IsRequired();
        builder.Property(x => x.Temperature).IsRequired();
        builder.Property(x => x.DeviceProfileId).HasMaxLength(36).IsRequired();
        builder.HasOne(x => x.DeviceProfile).WithMany(d => d.Telemetry).HasForeignKey(x => x.DeviceProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.DeviceProfileId, x.Timestamp });
    }
    #endregion
}