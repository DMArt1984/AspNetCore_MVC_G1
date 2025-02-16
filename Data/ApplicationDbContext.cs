using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Models.Control;

namespace AspNetCore_MVC_Project.Data
{
    public class ApplicationDbContext : DbContext
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
            base.OnModelCreating(modelBuilder);

            // ����� "������� - ��������" (������-�-������)
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Factory)
                .WithMany()
                .HasForeignKey(p => p.FactoryId)
                .OnDelete(DeleteBehavior.SetNull); // ���� �������� ���������, FactoryId = NULL

            // ����� "������� - OptionBlock" (������-�-������)
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.OptionBlock)
                .WithMany(o => o.Purchases)
                .HasForeignKey(p => p.OptionBlockId)
                .OnDelete(DeleteBehavior.Cascade); // ���� `OptionBlock` ���������, ��� ��������� `Purchase` ���� ���������
        }
    }
}
