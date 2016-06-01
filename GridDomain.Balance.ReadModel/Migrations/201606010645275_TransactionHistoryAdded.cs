namespace GridDomain.Balance.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TransactionHistoryAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BusinessId = c.Guid(nullable: false),
                        BalanceId = c.Guid(nullable: false),
                        EventType = c.String(),
                        Event = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.TransactoinHistories");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TransactoinHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BusinessId = c.Guid(nullable: false),
                        BalanceId = c.Guid(nullable: false),
                        TransactionSource = c.Guid(nullable: false),
                        TransactionType = c.String(),
                        TransactionDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.TransactionHistories");
        }
    }
}
