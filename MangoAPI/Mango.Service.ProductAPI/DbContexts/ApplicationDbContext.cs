using Mango.Service.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Service.ProductAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 1,
                Name = "Samosa",
                Price = 16,
                Description = "Praesent scelerisque, mi sed ultrices condimentum",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/c/cb/Samosachutney.jpg",
                CategoryName = "Apptizer"
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 2,
                Name = "Paneer Tikka",
                Price = 13.99,
                Description = "Praesent scelerisque, mi sed ultrices condimentum",
                ImageUrl = "https://en.wikipedia.org/wiki/Paneer_tikka#/media/File:Paneer_Tikka_Masala.jpg",
                CategoryName = "Apptizer"
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 3,
                Name = "Apple Pie",
                Price = 10.99,
                Description = "Praesent scelerisque, mi sed ultrices condimentum",
                ImageUrl = "https://en.wikipedia.org/wiki/Apple_pie#/media/File:Apple_pie_14.jpg",
                CategoryName = "Dessert"
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 4,
                Name = "Pav Bhaji",
                Price = 15,
                Description = "Praesent scelerisque, mi sed ultrices condimentum",
                ImageUrl = "https://en.wikipedia.org/wiki/Pav_bhaji#/media/File:Pav_Bhaji.jpg",
                CategoryName = "Entree"
            });
        }
    }
}
