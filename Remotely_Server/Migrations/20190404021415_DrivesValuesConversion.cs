using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely_Server.Migrations
{
    public partial class DrivesValuesConversion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Drives");

            migrationBuilder.AddColumn<string>(
                name: "Drives",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Drives",
                table: "Devices");

            migrationBuilder.CreateTable(
                name: "Drives",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    DeviceID = table.Column<string>(nullable: true),
                    DriveFormat = table.Column<string>(nullable: true),
                    DriveType = table.Column<int>(nullable: false),
                    FreeSpace = table.Column<double>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RootDirectory = table.Column<string>(nullable: true),
                    TotalSize = table.Column<double>(nullable: false),
                    VolumeLabel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drives", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Drives_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drives_DeviceID",
                table: "Drives",
                column: "DeviceID");
        }
    }
}
