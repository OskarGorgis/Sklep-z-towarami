using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.EntityFrameworkCore;
using Sklep_z_towarami.Models;


namespace Sklep_z_towarami.Data
{
    public class MyDbContext:IdentityDbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options): base(options) { }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
            modelBuilder.Entity<Article>().HasOne(a => a.Category).WithMany(c => c.Articles).HasForeignKey(a => a.CategoryId);
        }
    }

    public static class ModelBuilderExtention
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category()
                {
                    Id = 1,
                    Name = "Food"
                },
                new Category()
                {
                    Id = 2,
                    Name = "Drink"
                },
                new Category()
                {
                    Id = 3,
                    Name = "Other"
                },
                new Category()
                {
                    Id = 4,
                    Name = "Tools"
                },
                new Category()
                {
                    Id = 5,
                    Name = "Toys"
                }
                );

            modelBuilder.Entity<Article>().HasData(
                new Article()
                {
                    Id = 1,
                    Name = "Bread",
                    Price = 2.5f,
                    ImagePath = "./attachments/basic_question_mark.jpg",
                    CategoryId = 1,
                    Promo = true
                },
                new Article()
                {
                    Id= 2,
                    Name = "Water",
                    Price = 1.5f,
                    ImagePath = "./attachments/basic_question_mark.jpg",
                    CategoryId = 2,
                    Promo = false
                }
                ); ; 
        }
    }
}

