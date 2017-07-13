using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GridDomain.Tests.Acceptance.Migrations
{
    public partial class BalloonInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BalloonCatalog",
                columns: table => new
                {
                    BalloonId = table.Column<Guid>(nullable: false),
                    LastChanged = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    TitleVersion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalloonCatalog", x => x.BalloonId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalloonCatalog");
        }
    }
}
