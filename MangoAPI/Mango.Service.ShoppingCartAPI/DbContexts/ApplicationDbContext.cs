using Mango.Service.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Service.ShoppingCartAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
