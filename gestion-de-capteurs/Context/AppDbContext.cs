using gestion_de_capteurs.Entity;
using gestion_de_capteurs.Models;
using Microsoft.EntityFrameworkCore;

namespace gestion_de_capteurs.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Authentification> Authentifications { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Optional override if you don't want to pass options externally
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=sensors.db");
            }
        }
    }
}
