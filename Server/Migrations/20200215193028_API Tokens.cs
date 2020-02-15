using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations
{
    public partial class APITokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiTokens",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    LastUsed = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    OrganizationID = table.Column<string>(nullable: true),
                    Secret = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiTokens", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ApiTokens_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemotelyUsers_UserName",
                table: "RemotelyUsers",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_ApiTokens_OrganizationID",
                table: "ApiTokens",
                column: "OrganizationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiTokens");

            migrationBuilder.DropIndex(
                name: "IX_RemotelyUsers_UserName",
                table: "RemotelyUsers");
        }
    }
}
