using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations
{
    public partial class Devicealias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "Devices",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alias",
                table: "Devices");
        }
    }
}
