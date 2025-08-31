using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MotoNow.Domain.Entities;

namespace MotoNow.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DeliveryDriver> DeliveryDrivers => Set<DeliveryDriver>();
        public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
        public DbSet<Rental> Rentals => Set<Rental>();
        public DbSet<MotorcycleNotification> MotorcycleNotifications => Set<MotorcycleNotification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
