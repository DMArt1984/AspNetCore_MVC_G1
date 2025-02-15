using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Models;

namespace AspNetCore_MVC_Project.Data
{
    public class CompanyDbContext : DbContext
    {
        private readonly string _connectionString;

        public CompanyDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<Mark> Marks { get; set; } // Новая таблица Mark
    }
}
