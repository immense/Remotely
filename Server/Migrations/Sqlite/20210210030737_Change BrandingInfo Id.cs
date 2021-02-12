using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations.Sqlite
{
    public partial class ChangeBrandingInfoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_BrandingInfo_BrandingInfoInfoId",
                table: "Organizations");

            migrationBuilder.RenameColumn(
                name: "BrandingInfoInfoId",
                table: "Organizations",
                newName: "BrandingInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Organizations_BrandingInfoInfoId",
                table: "Organizations",
                newName: "IX_Organizations_BrandingInfoId");

            migrationBuilder.RenameColumn(
                name: "InfoId",
                table: "BrandingInfo",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_BrandingInfo_BrandingInfoId",
                table: "Organizations",
                column: "BrandingInfoId",
                principalTable: "BrandingInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_BrandingInfo_BrandingInfoId",
                table: "Organizations");

            migrationBuilder.RenameColumn(
                name: "BrandingInfoId",
                table: "Organizations",
                newName: "BrandingInfoInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Organizations_BrandingInfoId",
                table: "Organizations",
                newName: "IX_Organizations_BrandingInfoInfoId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BrandingInfo",
                newName: "InfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_BrandingInfo_BrandingInfoInfoId",
                table: "Organizations",
                column: "BrandingInfoInfoId",
                principalTable: "BrandingInfo",
                principalColumn: "InfoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
