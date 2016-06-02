using System.Data.Entity.Migrations;

namespace GridDomain.Balance.ReadModel.Migrations
{
    public partial class TransactionHistoryAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionHistories",
                c => new
                {
                    Id = c.Guid(false),
                    BusinessId = c.Guid(false),
                    BalanceId = c.Guid(false),
                    EventType = c.String(),
                    Event = c.String()
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
                    Id = c.Guid(false),
                    BusinessId = c.Guid(false),
                    BalanceId = c.Guid(false),
                    TransactionSource = c.Guid(false),
                    TransactionType = c.String(),
                    TransactionDescription = c.String()
                })
                .PrimaryKey(t => t.Id);

            DropTable("dbo.TransactionHistories");
        }
    }
}