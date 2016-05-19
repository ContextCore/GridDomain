namespace GridDomain.Balance.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BalanceReadModelConcurrency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessBalances", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessBalances", "RowVersion");
        }
    }
}
