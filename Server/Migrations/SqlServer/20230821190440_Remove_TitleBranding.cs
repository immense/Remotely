using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Remotely.Server.Migrations.SqlServer;

/// <inheritdoc />
public partial class Remove_TitleBranding : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ButtonForegroundBlue",
            table: "BrandingInfos");

        migrationBuilder.DropColumn(
            name: "ButtonForegroundGreen",
            table: "BrandingInfos");

        migrationBuilder.DropColumn(
            name: "ButtonForegroundRed",
            table: "BrandingInfos");

        migrationBuilder.DropColumn(
            name: "TitleBackgroundBlue",
            table: "BrandingInfos");

        migrationBuilder.DropColumn(
            name: "TitleBackgroundGreen",
            table: "BrandingInfos");

        migrationBuilder.DropColumn(
            name: "TitleBackgroundRed",
            table: "BrandingInfos");

        migrationBuilder.DropColumn(
            name: "TitleForegroundBlue",
            table: "BrandingInfos");

        migrationBuilder.DropColumn(
            name: "TitleForegroundGreen",
            table: "BrandingInfos");

        migrationBuilder.DropColumn(
            name: "TitleForegroundRed",
            table: "BrandingInfos");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<byte>(
            name: "ButtonForegroundBlue",
            table: "BrandingInfos",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);

        migrationBuilder.AddColumn<byte>(
            name: "ButtonForegroundGreen",
            table: "BrandingInfos",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);

        migrationBuilder.AddColumn<byte>(
            name: "ButtonForegroundRed",
            table: "BrandingInfos",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);

        migrationBuilder.AddColumn<byte>(
            name: "TitleBackgroundBlue",
            table: "BrandingInfos",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);

        migrationBuilder.AddColumn<byte>(
            name: "TitleBackgroundGreen",
            table: "BrandingInfos",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);

        migrationBuilder.AddColumn<byte>(
            name: "TitleBackgroundRed",
            table: "BrandingInfos",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);

        migrationBuilder.AddColumn<byte>(
            name: "TitleForegroundBlue",
            table: "BrandingInfos",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);

        migrationBuilder.AddColumn<byte>(
            name: "TitleForegroundGreen",
            table: "BrandingInfos",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);

        migrationBuilder.AddColumn<byte>(
            name: "TitleForegroundRed",
            table: "BrandingInfos",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);
    }
}
