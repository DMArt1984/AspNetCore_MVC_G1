using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Models;

namespace AspNetCore_MVC_Project.Data
{
    /// <summary>
    /// Контекст базы данных компании.
    /// </summary>
    public class CompanyDbContext : DbContext
    {
        private readonly string _connectionString;

        /// <summary>
        /// Конструктор с передачей строки подключения (используется в коде).
        /// </summary>
        public CompanyDbContext(string connectionString)
            : base(new DbContextOptionsBuilder<CompanyDbContext>().UseSqlServer(connectionString).Options)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Пустой конструктор для работы с `Add-Migration` (используется при генерации миграций).
        /// </summary>
        public CompanyDbContext(DbContextOptions<CompanyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<Mark> Marks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Добавляем проверку, чтобы EF не создавал дубликаты таблиц
            //modelBuilder.HasDefaultSchema("dbo");

            //if (!Database.CanConnect())
            {
                // Отключаем управление EF Core для Jobs и Statistics
                //modelBuilder.Entity<Job>().ToTable("Jobs").HasNoKey();
                //modelBuilder.Entity<Statistic>().ToTable("Statistics").HasNoKey();

                // Добавляем `Marks` в управление EF Core
                //modelBuilder.Entity<Mark>().ToTable("Marks");


                // Указываем существующие таблицы, чтобы EF Core их НЕ пересоздавал
                modelBuilder.Entity<Job>().ToTable("Jobs").HasKey(j => j.Id);
                modelBuilder.Entity<Statistic>().ToTable("Statistics").HasKey(s => s.Id);
                modelBuilder.Entity<Mark>().ToTable("Marks").HasKey(m => m.Id);
            }
        }
    }
}
