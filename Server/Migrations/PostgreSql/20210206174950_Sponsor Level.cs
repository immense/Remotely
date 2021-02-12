using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations.PostgreSql
{
    public partial class SponsorLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSponsor",
                table: "Organizations",
                newName: "IsDefaultOrganization");

            migrationBuilder.AddColumn<double>(
                name: "SponsorAmount",
                table: "Organizations",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SponsorAmount",
                table: "Organizations");

            migrationBuilder.RenameColumn(
                name: "IsDefaultOrganization",
                table: "Organizations",
                newName: "IsSponsor");
        }
    }
}
