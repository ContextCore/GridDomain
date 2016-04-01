using System.Data.Entity.Migrations;

namespace GridDomain.EventStore.MSSQL.Migrations
{
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Logged = c.DateTime(nullable: false),
                        Message = c.String(),
                        Logger = c.String(),
                        StackTrace = c.String(),
                        ThreadId = c.Int(nullable: false),
                        CallSite = c.String(),
                        Exception = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LogRecords");
        }
    }
}
