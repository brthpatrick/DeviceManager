using Microsoft.EntityFrameworkCore;
using DeviceManager.API.Models;

namespace DeviceManager.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Devices");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Manufacturer).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Type).IsRequired().HasMaxLength(20);
                entity.Property(d => d.OperatingSystem).HasMaxLength(50);
                entity.Property(d => d.OSVersion).HasMaxLength(30);
                entity.Property(d => d.Processor).HasMaxLength(100);
                entity.Property(d => d.RAM).HasMaxLength(20);
                entity.Property(d => d.Description).HasMaxLength(500);

                entity.HasOne(d => d.User)
                      .WithMany(u => u.Devices)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Role).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Location).IsRequired().HasMaxLength(100);    
                entity.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}
