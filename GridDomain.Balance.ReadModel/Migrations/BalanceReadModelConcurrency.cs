using System.Data.Entity.Migrations;

namespace GridDomain.Balance.ReadModel.Migrations
{
    public partial class BalanceReadModelConcurrency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessBalances", "RowVersion",
                c => c.Binary(false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }

        public override void Down()
        {
            DropColumn("dbo.BusinessBalances", "RowVersion");
        }
    }
}