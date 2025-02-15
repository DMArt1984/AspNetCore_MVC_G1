using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Models;
using Microsoft.Data.SqlClient;

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

        /// <summary>
        /// Проверяет и создаёт таблицы, которых ещё нет.
        /// </summary>
        public void EnsureTablesCreated()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                // Проверяем и создаём таблицу Jobs
                var checkJobs = new SqlCommand("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Jobs') CREATE TABLE Jobs (Id INT IDENTITY(1,1) PRIMARY KEY, Title NVARCHAR(MAX) NOT NULL, AnalogValue DECIMAL(18,2) NOT NULL)", connection);
                checkJobs.ExecuteNonQuery();

                // Проверяем и создаём таблицу Statistics
                var checkStatistics = new SqlCommand("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Statistics') CREATE TABLE Statistics (Id INT IDENTITY(1,1) PRIMARY KEY, Title NVARCHAR(MAX) NOT NULL, Kf1 FLOAT NOT NULL, Kf2 FLOAT NOT NULL, Kf3 FLOAT NOT NULL)", connection);
                checkStatistics.ExecuteNonQuery();

                // Проверяем и создаём таблицу Marks
                var checkMarks = new SqlCommand("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Marks') CREATE TABLE Marks (Id INT IDENTITY(1,1) PRIMARY KEY, Title NVARCHAR(MAX) NOT NULL, Value NVARCHAR(MAX) NOT NULL)", connection);
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

            // Добавляем проверку, чтобы EF не создавал дубликаты таблиц
            //modelBuilder.HasDefaultSchema("dbo");

            //if (!Database.CanConnect())
            {
                // v1
                // Отключаем управление EF Core для Jobs и Statistics
                //modelBuilder.Entity<Job>().ToTable("Jobs").HasNoKey();
                //modelBuilder.Entity<Statistic>().ToTable("Statistics").HasNoKey();

                // Добавляем `Marks` в управление EF Core
                //modelBuilder.Entity<Mark>().ToTable("Marks");

                // v2
                // Указываем существующие таблицы, чтобы EF Core их НЕ пересоздавал
                modelBuilder.Entity<Job>().ToTable("Jobs").HasKey(j => j.Id);
                modelBuilder.Entity<Statistic>().ToTable("Statistics").HasKey(s => s.Id);
                modelBuilder.Entity<Mark>().ToTable("Marks").HasKey(m => m.Id);

                // v3
                // Проверяем существование таблиц перед добавлением в модель
                //if (!TableExists("Jobs"))
                //    modelBuilder.Entity<Job>().ToTable("Jobs");

                //if (!TableExists("Statistics"))
                //    modelBuilder.Entity<Statistic>().ToTable("Statistics");

                //if (!TableExists("Marks"))
                //    modelBuilder.Entity<Mark>().ToTable("Marks");
            }
        }

        /// <summary>
        /// Проверяет, существует ли таблица в базе данных.
        /// </summary>
        private bool TableExists(string tableName)
        {
            var query = $@"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = '{tableName}'";

            var count = Database.ExecuteSqlRaw(query);

            return count > 0;
        }

    }
}
