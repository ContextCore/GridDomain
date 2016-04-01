using System.Data.Entity.Migrations;

namespace GridDomain.Balance.ReadModel.Migrations
{
    public partial class BalanceMoney : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessCurrentBalances", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BusinessCurrentBalances", "Currency", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessCurrentBalances", "Currency");
            DropColumn("dbo.BusinessCurrentBalances", "Amount");
        }
    }
}
