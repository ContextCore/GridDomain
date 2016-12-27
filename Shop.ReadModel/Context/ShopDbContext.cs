using System;
using Microsoft.EntityFrameworkCore;

namespace Shop.ReadModel.Context
{

    public class Account
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public Guid UserId { get; set; }
        public string Login { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
    }

    public class AccountHistory
    {
        public Guid AccountId { get; set; }
        public AccountOperations Operation { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal NewAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
    }

    public enum AccountOperations
    {
        Replenish,
        Withdrawal
    }
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options)
            :base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Order>().HasIndex(o => o.Number);

            modelBuilder.Entity<OrderItem>().HasKey(o => new { o.OrderId, o.NumberInOrder});

            modelBuilder.Entity<User>().HasKey(o =>  o.Id);
            modelBuilder.Entity<User>().HasIndex(o =>  o.Login);

            modelBuilder.Entity<Good>().HasIndex(o =>  o.Id);
            modelBuilder.Entity<Good>().HasIndex(o =>  o.Name);

            modelBuilder.Entity<Account>().HasKey(o =>  o.Id);
            modelBuilder.Entity<Account>().HasIndex(o => o.Number);

            modelBuilder.Entity<Good>().HasIndex(o =>  o.Name);
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Good> Goods { get; set; }
    }
}