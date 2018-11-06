using System;
using Microsoft.EntityFrameworkCore;
using Moniter.Models;

namespace Moniter.Infrastructure
{
    public class MoniterContext : DbContext
    {
        public MoniterContext(DbContextOptions options) 
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Binding> Bindings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Floor>().HasOne(x => x.Building).WithMany(b => b.Floors).HasForeignKey(p => p.BuildingId);
            modelBuilder.Entity<Floor>().HasOne(x => x.Binding).WithOne(b => b.Floor)
                .HasForeignKey<Binding>(p => p.FloorId);
            modelBuilder.Entity<Room>().HasOne(x => x.Floor).WithMany(b => b.Rooms).HasForeignKey(p => p.FloorId);
            modelBuilder.Entity<Bed>().HasOne(x => x.Room).WithMany(b => b.Beds).HasForeignKey(p => p.RoomId);
        }
    }
}