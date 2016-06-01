namespace GridDomain.Balance.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TimeForTransactionHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionHistories", "Time", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransactionHistories", "Time");
        }
    }
}
