using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations.Sqlite
{
    public partial class AddAlertDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Alerts",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "Alerts");
        }
    }
}
