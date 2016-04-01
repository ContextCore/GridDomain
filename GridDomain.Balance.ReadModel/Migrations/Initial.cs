using System.Data.Entity.Migrations;

namespace GridDomain.Balance.ReadModel.Migrations
{
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessCurrentBalances",
                c => new
                    {
                        BusinessId = c.Guid(nullable: false),
                        BalanceId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.BusinessId);
            
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TransactoinHistories");
            DropTable("dbo.BusinessCurrentBalances");
        }
    }
}
