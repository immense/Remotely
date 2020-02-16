using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations
{
    public partial class ApiTokenandDeviceindexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUsed",
                table: "ApiTokens",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceName",
                table: "Devices",
                column: "DeviceName");

            migrationBuilder.CreateIndex(
                name: "IX_ApiTokens_Token",
                table: "ApiTokens",
                column: "Token");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceName",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_ApiTokens_Token",
                table: "ApiTokens");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUsed",
                table: "ApiTokens",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
