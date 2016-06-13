namespace BusinessNews.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessAccounts",
                c => new
                    {
                        BalanceId = c.Guid(nullable: false),
                        BusinessId = c.Guid(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.String(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.BalanceId);
            
            CreateTable(
                "dbo.TransactionHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BalanceId = c.Guid(nullable: false),
                        Time = c.DateTime(nullable: false),
                        EventType = c.String(),
                        Event = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TransactionHistories");
            DropTable("dbo.BusinessAccounts");
        }
    }
}
