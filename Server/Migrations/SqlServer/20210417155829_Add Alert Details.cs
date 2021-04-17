using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations.SqlServer
{
    public partial class AddAlertDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Alerts",
                type: "nvarchar(max)",
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
