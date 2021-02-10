using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations.SqlServer
{
    public partial class WebRtcSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WebRtcSetting",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebRtcSetting",
                table: "Devices");
        }
    }
}
