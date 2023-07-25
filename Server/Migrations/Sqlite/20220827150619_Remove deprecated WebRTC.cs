using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Remotely.Server.Migrations.Sqlite;

public partial class RemovedeprecatedWebRTC : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "WebRtcSetting",
            table: "Devices");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "WebRtcSetting",
            table: "Devices",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);
    }
}
