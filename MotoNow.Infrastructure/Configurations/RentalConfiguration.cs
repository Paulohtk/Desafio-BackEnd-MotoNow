using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MotoNow.Domain.Entities;

namespace MotoNow.Infrastructure.Configurations
{

    public class RentalConfiguration : IEntityTypeConfiguration<Rental>
    {
        public void Configure(EntityTypeBuilder<Rental> b)
        {
            b.ToTable("rentals", "motonow");

            b.HasKey(x => x.Identifier);

            b.Property(x => x.DeliveryDriverId).IsRequired();
            b.Property(x => x.MotorcycleId).IsRequired();

            b.Property(x => x.StartAt).IsRequired().HasColumnType("timestamp with time zone");
            b.Property(x => x.EndAt).HasColumnType("timestamp with time zone");
            b.Property(x => x.ExpectedEndAt).IsRequired().HasColumnType("timestamp with time zone");
            b.Property(x => x.ReturnDate).HasColumnType("timestamp with time zone");


            b.Property(x => x.PlanDays).IsRequired();

            b.HasOne<DeliveryDriver>()
                .WithMany()
                .HasForeignKey(x => x.DeliveryDriverId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Motorcycle>()
                .WithMany()
                .HasForeignKey(x => x.MotorcycleId)
                .OnDelete(DeleteBehavior.Restrict);

            
            b.HasIndex(x => new { x.DeliveryDriverId, x.MotorcycleId, x.StartAt, x.ReturnDate });

            b.ToTable(t =>
            {
                t.HasCheckConstraint("ck_rental_plan_days", "plan_days > 0");
            });
        }
    }
}