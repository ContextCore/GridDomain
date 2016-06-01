namespace GridDomain.Balance.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrimaryKeyForTransactionHistory : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TransactionHistories", "BusinessId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransactionHistories", "BusinessId", c => c.Guid(nullable: false));
        }
    }
}
