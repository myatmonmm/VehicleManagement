using Microsoft.EntityFrameworkCore;
using VehicleManagement.Web.Models;

namespace VehicleManagement.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<VehicleCategory> VehicleCategories => Set<VehicleCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // precision settings
        modelBuilder.Entity<Vehicle>()
            .Property(v => v.WeightKg)
            .HasPrecision(10, 2);

        modelBuilder.Entity<VehicleCategory>()
            .Property(c => c.MinWeight)
            .HasPrecision(10, 2);

        modelBuilder.Entity<VehicleCategory>()
            .Property(c => c.MaxWeight)
            .HasPrecision(10, 2);

        // category names must be unique
        modelBuilder.Entity<VehicleCategory>()
            .HasIndex(c => c.Name)
            .IsUnique();

        //relationship
        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Manufacturer)
            .WithMany()
            .HasForeignKey(v => v.ManufacturerId)
            .OnDelete(DeleteBehavior.Restrict);

        // manufacturer seed
        modelBuilder.Entity<Manufacturer>().HasData(
new Manufacturer { Id = 1, Name = "Mazda" },
new Manufacturer { Id = 2, Name = "Mercedes" },
new Manufacturer { Id = 3, Name = "Honda" },
new Manufacturer { Id = 4, Name = "Ferrari" },
new Manufacturer { Id = 5, Name = "Toyota" }
);

        // Category seed
        modelBuilder.Entity<VehicleCategory>().HasData(
            new VehicleCategory
            {
                Id = 1,
                Name = "Light",
                MinWeight = 0m,
                MaxWeight = 500m,
                Icon = "bi-car-front-fill"
            },
            new VehicleCategory
            {
                Id = 2,
                Name = "Medium",
                MinWeight = 500m,
                MaxWeight = 2500m,
                Icon = "bi-taxi-front-fill"
            },
            new VehicleCategory
            {
                Id = 3,
                Name = "Heavy",
                MinWeight = 2500m,
                MaxWeight = null,
                Icon = "bi-truck-front-fill"
            }
        );
    }



}