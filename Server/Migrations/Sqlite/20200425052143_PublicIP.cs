using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations.Sqlite
{
    public partial class PublicIP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicIP",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicIP",
                table: "Devices");
        }
    }
}
