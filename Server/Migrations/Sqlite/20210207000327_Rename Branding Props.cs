using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations.Sqlite
{
    public partial class RenameBrandingProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "BrandingTitleForegroundRed",
                table: "BrandingInfo",
                newName: "TitleForegroundRed");

            migrationBuilder.RenameColumn(
                name: "BrandingTitleForegroundGreen",
                table: "BrandingInfo",
                newName: "TitleForegroundGreen");

            migrationBuilder.RenameColumn(
                name: "BrandingTitleForegroundBlue",
                table: "BrandingInfo",
                newName: "TitleForegroundBlue");

            migrationBuilder.RenameColumn(
                name: "BrandingTitleBackgroundRed",
                table: "BrandingInfo",
                newName: "TitleBackgroundRed");

            migrationBuilder.RenameColumn(
                name: "BrandingTitleBackgroundGreen",
                table: "BrandingInfo",
                newName: "TitleBackgroundGreen");

            migrationBuilder.RenameColumn(
                name: "BrandingTitleBackgroundBlue",
                table: "BrandingInfo",
                newName: "TitleBackgroundBlue");

            migrationBuilder.RenameColumn(
                name: "BrandingProduct",
                table: "BrandingInfo",
                newName: "Product");

            migrationBuilder.RenameColumn(
                name: "BrandingIcon",
                table: "BrandingInfo",
                newName: "Icon");

            migrationBuilder.RenameColumn(
                name: "BrandingButtonForegroundRed",
                table: "BrandingInfo",
                newName: "ButtonForegroundRed");

            migrationBuilder.RenameColumn(
                name: "BrandingButtonForegroundGreen",
                table: "BrandingInfo",
                newName: "ButtonForegroundGreen");

            migrationBuilder.RenameColumn(
                name: "BrandingButtonForegroundBlue",
                table: "BrandingInfo",
                newName: "ButtonForegroundBlue");

            migrationBuilder.RenameColumn(
                name: "BrandingInfoId",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "TitleForegroundRed",
                table: "BrandingInfo",
                newName: "BrandingTitleForegroundRed");

            migrationBuilder.RenameColumn(
                name: "TitleForegroundGreen",
                table: "BrandingInfo",
                newName: "BrandingTitleForegroundGreen");

            migrationBuilder.RenameColumn(
                name: "TitleForegroundBlue",
                table: "BrandingInfo",
                newName: "BrandingTitleForegroundBlue");

            migrationBuilder.RenameColumn(
                name: "TitleBackgroundRed",
                table: "BrandingInfo",
                newName: "BrandingTitleBackgroundRed");

            migrationBuilder.RenameColumn(
                name: "TitleBackgroundGreen",
                table: "BrandingInfo",
                newName: "BrandingTitleBackgroundGreen");

            migrationBuilder.RenameColumn(
                name: "TitleBackgroundBlue",
                table: "BrandingInfo",
                newName: "BrandingTitleBackgroundBlue");

            migrationBuilder.RenameColumn(
                name: "Product",
                table: "BrandingInfo",
                newName: "BrandingProduct");

            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "BrandingInfo",
                newName: "BrandingIcon");

            migrationBuilder.RenameColumn(
                name: "ButtonForegroundRed",
                table: "BrandingInfo",
                newName: "BrandingButtonForegroundRed");

            migrationBuilder.RenameColumn(
                name: "ButtonForegroundGreen",
                table: "BrandingInfo",
                newName: "BrandingButtonForegroundGreen");

            migrationBuilder.RenameColumn(
                name: "ButtonForegroundBlue",
                table: "BrandingInfo",
                newName: "BrandingButtonForegroundBlue");

            migrationBuilder.RenameColumn(
                name: "InfoId",
                table: "BrandingInfo",
                newName: "BrandingInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_BrandingInfo_BrandingInfoId",
                table: "Organizations",
                column: "BrandingInfoId",
                principalTable: "BrandingInfo",
                principalColumn: "BrandingInfoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
