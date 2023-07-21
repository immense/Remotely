using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Remotely.Server.Migrations.SqlServer;

/// <inheritdoc />
public partial class ReplaceDbLoggerwithSerilog : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "EventLogs");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EventLogs",
            columns: table => new
            {
                ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                OrganizationID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                EventType = table.Column<int>(type: "int", nullable: false),
                Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TimeStamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EventLogs", x => x.ID);
                table.ForeignKey(
                    name: "FK_EventLogs_Organizations_OrganizationID",
                    column: x => x.OrganizationID,
                    principalTable: "Organizations",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateIndex(
            name: "IX_EventLogs_OrganizationID",
            table: "EventLogs",
            column: "OrganizationID");
    }
}
