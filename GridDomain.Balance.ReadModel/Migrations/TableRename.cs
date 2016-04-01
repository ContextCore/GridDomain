using System.Data.Entity.Migrations;

namespace GridDomain.Balance.ReadModel.Migrations
{
    public partial class TableRename : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessBalances",
                c => new
                {
                    BalanceId = c.Guid(false),
                    BusinessId = c.Guid(false),
                    Amount = c.Decimal(false, 18, 2),
                    Currency = c.String()
                })
                .PrimaryKey(t => t.BalanceId);

            DropTable("dbo.BusinessCurrentBalances");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.BusinessCurrentBalances",
                c => new
                {
                    BusinessId = c.Guid(false),
                    BalanceId = c.Guid(false),
                    Amount = c.Decimal(false, 18, 2),
                    Currency = c.String()
                })
                .PrimaryKey(t => t.BusinessId);

            DropTable("dbo.BusinessBalances");
        }
    }
}