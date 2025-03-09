using Microsoft.EntityFrameworkCore;

namespace AspNetCore_MVC_Project.Areas.BOX.Data
{
    public class BoxDbContext : DbContext
    {
        public BoxDbContext(DbContextOptions<BoxDbContext> options)
            : base(options)
        {
        }

        public DbSet<MarketItem> MarketItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Дополнительная настройка моделей, если требуется.
        }

        // Метод для создания базы и таблиц, если их нет
        public void EnsureDatabaseCreated()
        {
            Database.EnsureCreated();
        }
    }
}

