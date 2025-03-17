using Microsoft.EntityFrameworkCore;
using OKbkm.Models;
using System.Collections.Generic;

namespace OKbkm.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // 🌟 Veritabanı tablolarını burada tanımlıyoruz
        public DbSet<Register> Users { get; set; }
        public DbSet<AccountCreate> Accounts { get; set; }
        public DbSet<TransactionHistory> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=okbkm;Username=admin;Password=admin;");
            }
        }
    }
}
