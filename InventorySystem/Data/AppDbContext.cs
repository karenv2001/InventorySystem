using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InventorySystem.Data
{
    public class AppDbContext : DbContext
    {
        private readonly string _connection;
        public AppDbContext()
        {
            var config = Program.Configuration;
            _connection = config.GetConnectionString("DefaultConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connection, ServerVersion.AutoDetect(_connection));
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Entry> Entries { get; set; }
    }
}
