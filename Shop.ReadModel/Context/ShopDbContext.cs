using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Shop.ReadModel.Context
{
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

            modelBuilder.Entity<Good>().HasKey(o =>  o.Id);
            modelBuilder.Entity<Good>().HasIndex(o =>  o.Name);

            modelBuilder.Entity<Account>().HasKey(o =>  o.Id);
            modelBuilder.Entity<Account>().HasIndex(o => o.Number);

            modelBuilder.Entity<AccountTransaction>().HasKey(o => o.TransactionId);
            modelBuilder.Entity<AccountTransaction>().HasIndex(o => o.AccountId);
            modelBuilder.Entity<AccountTransaction>().HasIndex(o => o.TransactionNumber);
            modelBuilder.Entity<AccountTransaction>().Property(o => o.TransactionNumber)
                                                     .UseSqlServerIdentityColumn();

            modelBuilder.Entity<Good>().HasIndex(o =>  o.Name);

            modelBuilder.Entity<Sku>().HasKey(o =>  o.Id);
            modelBuilder.Entity<Sku>().HasIndex(o =>  o.Number);
            modelBuilder.Entity<Sku>().HasIndex(o =>  o.Article);
            modelBuilder.Entity<Sku>().HasIndex(o =>  o.Name);

            modelBuilder.Entity<SkuStock>().HasKey(o =>  o.Id);
            modelBuilder.Entity<SkuStock>().HasIndex(o =>  o.SkuId);

            modelBuilder.Entity<SkuReserve>().HasKey(o =>  new { o.StockId, o.CustomerId});
            modelBuilder.Entity<SkuReserve>().HasIndex(o =>  new {o.SkuId});

            modelBuilder.Entity<SkuStockHistory>().HasKey(o => new {o.StockId, o.Number});
            modelBuilder.Entity<SkuStockHistory>().Property(o => o.Number)
                                                  .UseSqlServerIdentityColumn();
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Good> Goods { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountTransaction> TransactionHistory { get; set; }
        public DbSet<Sku> Skus { get; set; }
        public DbSet<SkuStock> SkuStocks { get; set; }
        public DbSet<SkuReserve> SkuReserves { get; set; }
        public DbSet<SkuStockHistory> StockHistory { get;set; }
    }
}