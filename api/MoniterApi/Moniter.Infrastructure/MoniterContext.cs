using Microsoft.EntityFrameworkCore;
using Moniter.Domain;

namespace Moniter.Infrastructure
{
    public class MoniterContext : DbContext
    {
        private readonly string _databaseName = "moniter.db";

        public MoniterContext(DbContextOptions options) 
            : base(options)
        {
        }

        public MoniterContext(string databaseName)
        {
            _databaseName = databaseName;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_databaseName}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}