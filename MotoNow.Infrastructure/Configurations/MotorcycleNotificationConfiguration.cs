using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotoNow.Domain.Entities;

namespace MotoNow.Infrastructure.Configurations;

public class MotorcycleNotificationConfiguration : IEntityTypeConfiguration<MotorcycleNotification>
{
    public void Configure(EntityTypeBuilder<MotorcycleNotification> b)
    {
        b.ToTable("motorcycle_notifications", "motonow");

        b.HasKey(x => x.Identifier);

        b.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
        b.Property(x => x.Plate).IsRequired().HasMaxLength(10);
        b.Property(x => x.Model).IsRequired().HasMaxLength(120);
        b.Property(x => x.Year).IsRequired();

        b.Property(x => x.OccurredAt).HasColumnType("timestamp with time zone");
        b.Property(x => x.ReceivedAt).HasColumnType("timestamp with time zone");

        b.HasIndex(x => x.Year);
        b.HasIndex(x => x.Plate);
    }
}
