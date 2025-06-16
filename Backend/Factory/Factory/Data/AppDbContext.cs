using Factory.Models;
using Microsoft.EntityFrameworkCore;
using Factory.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Factory.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<QuoteRequest> QuoteRequests { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            // Seed initial categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "أقنعة الوقاية" },
                new Category { Id = 2, Name = "مرشحات الأقنعة" },
                new Category { Id = 3, Name = "البدل الوقائية" },
                new Category { Id = 4, Name = "ملحقات الحماية" }
            );
            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.ToTable("ContactMessages"); // Ensure correct table name
                entity.HasKey(e => e.Id); // Ensure primary key
            });
        }
    }
}