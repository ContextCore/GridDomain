using System.Data.Entity;

namespace BusinessNews.ReadModel
{
    public class BusinessBalanceContext : DbContext
    {
        public static string DefaultConnectionString
            = @"Data Source=(local);Initial Catalog=AutoTestGridDomainRead;Integrated Security = true";

        public BusinessBalanceContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<BusinessBalanceContext>());
        }

        public BusinessBalanceContext() : this(DefaultConnectionString)
        {
        }

        public DbSet<BusinessAccount> Accounts { get; set; }
        public DbSet<TransactionHistory> TransactoinHistory { get; set; }
    }
}