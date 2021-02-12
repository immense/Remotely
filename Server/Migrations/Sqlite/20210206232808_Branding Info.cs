using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations.Sqlite
{
    public partial class BrandingInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrandingInfoId",
                table: "Organizations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BrandingInfo",
                columns: table => new
                {
                    BrandingInfoId = table.Column<string>(type: "TEXT", nullable: false),
                    BrandingProduct = table.Column<string>(type: "TEXT", maxLength: 25, nullable: true),
                    BrandingIcon = table.Column<byte[]>(type: "BLOB", nullable: true),
                    BrandingTitleForegroundRed = table.Column<byte>(type: "INTEGER", nullable: false),
                    BrandingTitleForegroundGreen = table.Column<byte>(type: "INTEGER", nullable: false),
                    BrandingTitleForegroundBlue = table.Column<byte>(type: "INTEGER", nullable: false),
                    BrandingTitleBackgroundRed = table.Column<byte>(type: "INTEGER", nullable: false),
                    BrandingTitleBackgroundGreen = table.Column<byte>(type: "INTEGER", nullable: false),
                    BrandingTitleBackgroundBlue = table.Column<byte>(type: "INTEGER", nullable: false),
                    BrandingButtonForegroundRed = table.Column<byte>(type: "INTEGER", nullable: false),
                    BrandingButtonForegroundGreen = table.Column<byte>(type: "INTEGER", nullable: false),
                    BrandingButtonForegroundBlue = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandingInfo", x => x.BrandingInfoId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_BrandingInfoId",
                table: "Organizations",
                column: "BrandingInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_BrandingInfo_BrandingInfoId",
                table: "Organizations",
                column: "BrandingInfoId",
                principalTable: "BrandingInfo",
                principalColumn: "BrandingInfoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_BrandingInfo_BrandingInfoId",
                table: "Organizations");

            migrationBuilder.DropTable(
                name: "BrandingInfo");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_BrandingInfoId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "BrandingInfoId",
                table: "Organizations");
        }
    }
}
