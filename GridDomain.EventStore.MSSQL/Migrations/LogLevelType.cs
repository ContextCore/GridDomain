using System.Data.Entity.Migrations;

namespace GridDomain.EventStore.MSSQL.Migrations
{
    public partial class LogLevelType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LogRecords", "Level", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.LogRecords", "Level");
        }
    }
}