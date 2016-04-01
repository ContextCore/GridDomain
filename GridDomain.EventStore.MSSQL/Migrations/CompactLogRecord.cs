using System.Data.Entity.Migrations;

namespace GridDomain.EventStore.MSSQL.Migrations
{
    public partial class CompactLogRecord : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.LogRecords", "StackTrace");
            DropColumn("dbo.LogRecords", "CallSite");
            DropColumn("dbo.LogRecords", "Exception");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LogRecords", "Exception", c => c.String());
            AddColumn("dbo.LogRecords", "CallSite", c => c.String());
            AddColumn("dbo.LogRecords", "StackTrace", c => c.String());
        }
    }
}
