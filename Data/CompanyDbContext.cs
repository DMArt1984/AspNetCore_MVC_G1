using Microsoft.EntityFrameworkCore;
using Npgsql;
using AspNetCore_MVC_Project.Models.Factory;

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
            : base(new DbContextOptionsBuilder<CompanyDbContext>().UseNpgsql(connectionString).Options)
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

        /// <summary>
        /// Проверяет и создает таблицы, которых еще нет.
        /// </summary>
        public void EnsureTablesCreated()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                // Проверяем и создаем таблицу Jobs
                var checkJobs = new NpgsqlCommand(@"
                    CREATE TABLE IF NOT EXISTS Jobs (
                        Id SERIAL PRIMARY KEY, 
                        Title TEXT NOT NULL, 
                        AnalogValue DECIMAL(18,2) NOT NULL
                    )", connection);
                checkJobs.ExecuteNonQuery();

                // Проверяем и создаем таблицу Statistics
                var checkStatistics = new NpgsqlCommand(@"
                    CREATE TABLE IF NOT EXISTS Statistics (
                        Id SERIAL PRIMARY KEY, 
                        Title TEXT NOT NULL, 
                        Kf1 DOUBLE PRECISION NOT NULL, 
                        Kf2 DOUBLE PRECISION NOT NULL, 
                        Kf3 DOUBLE PRECISION NOT NULL
                    )", connection);
                checkStatistics.ExecuteNonQuery();

                // Проверяем и создаем таблицу Marks
                var checkMarks = new NpgsqlCommand(@"
                    CREATE TABLE IF NOT EXISTS Marks (
                        Id SERIAL PRIMARY KEY, 
                        Title TEXT NOT NULL, 
                        Value TEXT NOT NULL
                    )", connection);
                checkMarks.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке таблиц: {ex.Message}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Job>().ToTable("Jobs").HasKey(j => j.Id);
            modelBuilder.Entity<Statistic>().ToTable("Statistics").HasKey(s => s.Id);
            modelBuilder.Entity<Mark>().ToTable("Marks").HasKey(m => m.Id);
        }

        /// <summary>
        /// Проверяет, существует ли таблица в БД.
        /// </summary>
        private bool TableExists(string tableName)
        {
            var query = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_name = '{tableName}';";
            var count = Database.ExecuteSqlRaw(query);
            return count > 0;
        }
    }
}

