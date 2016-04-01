using System.Data.Entity.Migrations;

namespace GridDomain.EventStore.MSSQL.Migrations
{
    public partial class PresiceExecutionTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LogRecords", "TicksFromAppStart", c => c.Long(false));
        }

        public override void Down()
        {
            DropColumn("dbo.LogRecords", "TicksFromAppStart");
        }
    }
}