using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotoNow.Domain.Entities;

namespace MotoNow.Infrastructure.Configurations
{
    public class MotorCycleConfiguration : IEntityTypeConfiguration<Motorcycle>
    {
        public void Configure(EntityTypeBuilder<Motorcycle> builder)
        {
            builder.ToTable("motorcycles", "motonow");

            builder.HasKey(m => m.Identifier);

            builder.Property(m => m.Identifier)
                 .IsRequired()
                 .HasMaxLength(64)           
                 .ValueGeneratedNever();     

            builder.Property(m => m.Plate)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(m => m.Plate)
                .IsUnique();

            //builder.HasMany(m => m.Rentals)
            //    .WithOne(r => r.Motorcycle)
            //    .HasForeignKey(r => r.MotorcycleId);
        }
    }
}
