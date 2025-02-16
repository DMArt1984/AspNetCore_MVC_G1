using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Models.Control;
using AspNetCore_MVC_Project.Models; // Подключаем модели

namespace AspNetCore_MVC_Project.Data
{
    /// <summary>
    /// Контекст базы данных приложения. Используется для работы с Entity Framework Core.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Таблица компаний (фабрик).
        /// </summary>
        public DbSet<Factory> Factories { get; set; }

        /// <summary>
        /// Таблица покупок (разрешений на модули).
        /// </summary>
        public DbSet<Purchase> Purchases { get; set; }

        /// <summary>
        /// Таблица модулей (опциональных блоков).
        /// </summary>
        public DbSet<OptionBlock> OptionBlocks { get; set; }

        /// <summary>
        /// Конструктор контекста базы данных.
        /// </summary>
        /// <param name="options">Настройки для подключения к БД.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Определяет правила создания таблиц и связей между сущностями.
        /// </summary>
        /// <param name="modelBuilder">Объект построителя моделей.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // ОБЯЗАТЕЛЬНО вызываем базовый метод!

            // === Настройка связей между таблицами ===

            // Связь "Покупка - Компания" (многие-к-одному)
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Factory) // Один Factory может быть у многих Purchase
                .WithMany() // Не определяем навигационное свойство в Factory
                .HasForeignKey(p => p.FactoryId) // Внешний ключ FactoryId в Purchase
                .OnDelete(DeleteBehavior.SetNull); // Если компания удаляется, FactoryId становится NULL

            // Связь "Покупка - OptionBlock" (многие-к-одному)
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.OptionBlock) // Один OptionBlock может быть у многих Purchase
                .WithMany(o => o.Purchases) // В OptionBlock есть навигационное свойство Purchases
                .HasForeignKey(p => p.OptionBlockId) // Внешний ключ OptionBlockId в Purchase
                .OnDelete(DeleteBehavior.Cascade); // Если `OptionBlock` удаляется, все связанные `Purchase` тоже удаляются
        }
    }
}