using System.Data.Entity.Migrations;

namespace BusinessNews.ReadModel.Migrations
{
    public partial class BalanceMoney : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessCurrentBalances", "Amount", c => c.Decimal(false, 18, 2));
            AddColumn("dbo.BusinessCurrentBalances", "Currency", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.BusinessCurrentBalances", "Currency");
            DropColumn("dbo.BusinessCurrentBalances", "Amount");
        }
    }
}