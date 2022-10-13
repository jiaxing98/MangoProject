﻿using Mango.Service.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Service.OrderAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
