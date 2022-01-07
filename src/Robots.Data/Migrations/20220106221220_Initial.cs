using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Robots.Data.Migrations
{
  public partial class Initial : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Jobs",
          columns: table => new
          {
            JobId = table.Column<Guid>(type: "uuid", nullable: false),
            PayloadId = table.Column<int>(type: "integer", nullable: false),
            RobotId = table.Column<int>(type: "integer", nullable: false),
            DistanceToGoal = table.Column<decimal>(type: "numeric(38,17)", nullable: false),
            BatteryLevel = table.Column<int>(type: "integer", nullable: false),
            Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Jobs", x => x.JobId);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Jobs_Created",
          table: "Jobs",
          column: "Created");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Jobs");
    }
  }
}
