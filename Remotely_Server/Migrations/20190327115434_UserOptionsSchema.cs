using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely_Server.Migrations
{
    public partial class UserOptionsSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommandModeShortcutRemotely",
                table: "RemotelyUserOptions",
                newName: "CommandModeShortcutWeb");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommandModeShortcutWeb",
                table: "RemotelyUserOptions",
                newName: "CommandModeShortcutRemotely");
        }
    }
}
