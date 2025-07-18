using Microsoft.EntityFrameworkCore;
using OKbkm.Models;

namespace OKbkm
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) 
        {
        }

        public DbSet<Login> Logins { get; set; }
        public DbSet<Register> Registers { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<AccountCreate> AccountCreates { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<TransactionHistory> TransactionHistories { get; set; }
    }
}
