using System.Data.Entity.Migrations;

namespace GridDomain.Balance.ReadModel.Migrations
{
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessCurrentBalances",
                c => new
                {
                    BusinessId = c.Guid(false),
                    BalanceId = c.Guid(false)
                })
                .PrimaryKey(t => t.BusinessId);

            CreateTable(
                "dbo.TransactoinHistories",
                c => new
                {
                    Id = c.Guid(false),
                    BusinessId = c.Guid(false),
                    BalanceId = c.Guid(false),
                    TransactionSource = c.Guid(false),
                    TransactionType = c.String(),
                    TransactionDescription = c.String()
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.TransactoinHistories");
            DropTable("dbo.BusinessCurrentBalances");
        }
    }
}