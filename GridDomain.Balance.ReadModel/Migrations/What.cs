namespace GridDomain.Balance.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class What : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.BusinessBalances", "UpdatesCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BusinessBalances", "UpdatesCount", c => c.Int(nullable: false));
        }
    }
}
