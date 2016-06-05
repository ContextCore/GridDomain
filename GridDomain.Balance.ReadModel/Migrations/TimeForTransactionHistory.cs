using System.Data.Entity.Migrations;

namespace BusinessNews.ReadModel.Migrations
{
    public partial class TimeForTransactionHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionHistories", "Time", c => c.DateTime(false));
        }

        public override void Down()
        {
            DropColumn("dbo.TransactionHistories", "Time");
        }
    }
}