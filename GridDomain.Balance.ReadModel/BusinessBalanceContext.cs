using System.Data.Entity;

namespace GridDomain.Balance.ReadModel
{
    public class BusinessBalanceContext : DbContext
    {
        public static string DefaultConnectionString;
            // = @"Data Source=(localdb)\v11.0;Initial Catalog=AutoTestGridDomainRead;Integrated Security = true";

        public BusinessBalanceContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<BusinessBalanceContext>());
        }

        public BusinessBalanceContext() : this(DefaultConnectionString)
        {
        }

        public DbSet<BusinessBalance> Balances { get; set; }
        public DbSet<TransactoinHistory> TransactoinHistory { get; set; }
    }
}