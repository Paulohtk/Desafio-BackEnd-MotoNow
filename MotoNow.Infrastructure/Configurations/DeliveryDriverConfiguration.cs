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

    public class DeliveryDriverConfiguration : IEntityTypeConfiguration<DeliveryDriver>
    {
        public void Configure(EntityTypeBuilder<DeliveryDriver> b)
        {
            b.ToTable("delivery_drivers", "motonow");

            b.HasKey(x => x.Identifier);

            b.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Cnpj).IsRequired().HasMaxLength(14);
            b.Property(x => x.BirthDate).HasColumnType("date").IsRequired();

            b.Property(x => x.DriverLicenseNumber).IsRequired().HasMaxLength(11);
            b.Property(x => x.DriverLicenseClass).IsRequired().HasMaxLength(3);

            b.HasIndex(x => x.Identifier).IsUnique();
            b.HasIndex(x => x.Cnpj).IsUnique();
            b.HasIndex(x => x.DriverLicenseNumber).IsUnique();
            b.Property(x => x.DriverLicenseImageUrl).HasColumnName("driver_license_image_url");


            b.ToTable(t =>
            {
                t.HasCheckConstraint("ck_delivery_driver_cnpj_digits", "cnpj ~ '^[0-9]{14}$'");
                t.HasCheckConstraint("ck_delivery_driver_cnh_digits", "driver_license_number ~ '^[0-9]{11}$'");
                t.HasCheckConstraint("ck_delivery_driver_cnh_class", "driver_license_class IN ('A','B','AB')");
            });
        }
    }
}