using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Models;
using AspNetCore_MVC_Project.Models.Control;

namespace AspNetCore_MVC_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Factory> Factories { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<OptionBlock> OptionBlocks { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // ОБЯЗАТЕЛЬНО вызываем базовый метод!

            // Связь "Покупка - Компания" (многие-к-одному)
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Factory)
                .WithMany()
                .HasForeignKey(p => p.FactoryId)
                .OnDelete(DeleteBehavior.SetNull); // Если компания удаляется, FactoryId = NULL

            // Связь "Покупка - OptionBlock" (многие-к-одному)
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.OptionBlock)
                .WithMany(o => o.Purchases)
                .HasForeignKey(p => p.OptionBlockId)
                .OnDelete(DeleteBehavior.Cascade); // Если `OptionBlock` удаляется, все связанные `Purchase` тоже удаляются
        }
    }
}

