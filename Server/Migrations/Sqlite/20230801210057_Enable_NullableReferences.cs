using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Remotely.Server.Migrations.Sqlite;

/// <inheritdoc />
public partial class Enable_NullableReferences : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // These relationships weren't enforced previously.
        migrationBuilder.Sql("delete from \"ScriptResults\" where \"DeviceID\" is null;");
        migrationBuilder.Sql("delete from \"ScriptResults\" where \"DeviceID\" not in (select \"ID\" from \"Devices\");");
        migrationBuilder.Sql("delete from \"ScriptResults\" where \"OrganizationID\" is null;");
        migrationBuilder.Sql("delete from \"ScriptResults\" where \"OrganizationID\" not in (select \"ID\" from \"Organizations\");");
        migrationBuilder.Sql("delete from \"ScriptResults\" where \"SavedScriptId\" not in (select \"Id\" from \"SavedScripts\");");
        migrationBuilder.Sql("delete from \"ScriptRuns\" where \"OrganizationID\" is null;");
        migrationBuilder.Sql("delete from \"ScriptRuns\" where \"OrganizationID\" not in (select \"ID\" from \"Organizations\");");
        migrationBuilder.Sql("delete from \"ScriptRuns\" where \"SavedScriptId\" not in (select \"Id\" from \"SavedScripts\");");

        migrationBuilder.DropForeignKey(
            name: "FK_Alerts_Organizations_OrganizationID",
            table: "Alerts");

        migrationBuilder.DropForeignKey(
            name: "FK_ApiTokens_Organizations_OrganizationID",
            table: "ApiTokens");

        migrationBuilder.DropForeignKey(
            name: "FK_Devices_Organizations_OrganizationID",
            table: "Devices");

        migrationBuilder.DropForeignKey(
            name: "FK_InviteLinks_Organizations_OrganizationID",
            table: "InviteLinks");

        migrationBuilder.DropForeignKey(
            name: "FK_RemotelyUsers_Organizations_OrganizationID",
            table: "RemotelyUsers");

        migrationBuilder.DropForeignKey(
            name: "FK_SavedScripts_Organizations_OrganizationID",
            table: "SavedScripts");

        migrationBuilder.DropForeignKey(
            name: "FK_ScriptResults_Organizations_OrganizationID",
            table: "ScriptResults");

        migrationBuilder.DropForeignKey(
            name: "FK_ScriptRuns_ScriptSchedules_ScriptScheduleId",
            table: "ScriptRuns");

        migrationBuilder.DropTable(
            name: "DeviceScriptRun1");

        migrationBuilder.DropIndex(
            name: "IX_ScriptRuns_ScriptScheduleId",
            table: "ScriptRuns");

        migrationBuilder.DropColumn(
            name: "ScriptScheduleId",
            table: "ScriptRuns");

        migrationBuilder.AlterColumn<byte[]>(
            name: "FileContents",
            table: "SharedFiles",
            type: "BLOB",
            nullable: false,
            defaultValue: new byte[0],
            oldClrType: typeof(byte[]),
            oldType: "BLOB",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "ScriptSchedules",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "ScriptSchedules",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreatorId",
            table: "ScriptSchedules",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "ScriptRuns",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ScriptInput",
            table: "ScriptResults",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "ScriptResults",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DeviceID",
            table: "ScriptResults",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "SavedScripts",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreatorId",
            table: "SavedScripts",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationName",
            table: "Organizations",
            type: "TEXT",
            maxLength: 25,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 25,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "BrandingInfoId",
            table: "Organizations",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "InviteLinks",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "Devices",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "MacAddresses",
            table: "Devices",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "DeviceGroups",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "DeviceGroups",
            type: "TEXT",
            maxLength: 200,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 200,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "ApiTokens",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserID",
            table: "Alerts",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "Alerts",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DeviceID",
            table: "Alerts",
            type: "TEXT",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_ScriptRuns_SavedScriptId",
            table: "ScriptRuns",
            column: "SavedScriptId");

        migrationBuilder.CreateIndex(
            name: "IX_ScriptRuns_ScheduleId",
            table: "ScriptRuns",
            column: "ScheduleId");

        migrationBuilder.CreateIndex(
            name: "IX_ScriptResults_SavedScriptId",
            table: "ScriptResults",
            column: "SavedScriptId");

        migrationBuilder.AddForeignKey(
            name: "FK_Alerts_Organizations_OrganizationID",
            table: "Alerts",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_ApiTokens_Organizations_OrganizationID",
            table: "ApiTokens",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Devices_Organizations_OrganizationID",
            table: "Devices",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_InviteLinks_Organizations_OrganizationID",
            table: "InviteLinks",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_RemotelyUsers_Organizations_OrganizationID",
            table: "RemotelyUsers",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_SavedScripts_Organizations_OrganizationID",
            table: "SavedScripts",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_ScriptResults_Organizations_OrganizationID",
            table: "ScriptResults",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_ScriptResults_SavedScripts_SavedScriptId",
            table: "ScriptResults",
            column: "SavedScriptId",
            principalTable: "SavedScripts",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ScriptRuns_SavedScripts_SavedScriptId",
            table: "ScriptRuns",
            column: "SavedScriptId",
            principalTable: "SavedScripts",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ScriptRuns_ScriptSchedules_ScheduleId",
            table: "ScriptRuns",
            column: "ScheduleId",
            principalTable: "ScriptSchedules",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Alerts_Organizations_OrganizationID",
            table: "Alerts");

        migrationBuilder.DropForeignKey(
            name: "FK_ApiTokens_Organizations_OrganizationID",
            table: "ApiTokens");

        migrationBuilder.DropForeignKey(
            name: "FK_Devices_Organizations_OrganizationID",
            table: "Devices");

        migrationBuilder.DropForeignKey(
            name: "FK_InviteLinks_Organizations_OrganizationID",
            table: "InviteLinks");

        migrationBuilder.DropForeignKey(
            name: "FK_RemotelyUsers_Organizations_OrganizationID",
            table: "RemotelyUsers");

        migrationBuilder.DropForeignKey(
            name: "FK_SavedScripts_Organizations_OrganizationID",
            table: "SavedScripts");

        migrationBuilder.DropForeignKey(
            name: "FK_ScriptResults_Organizations_OrganizationID",
            table: "ScriptResults");

        migrationBuilder.DropForeignKey(
            name: "FK_ScriptResults_SavedScripts_SavedScriptId",
            table: "ScriptResults");

        migrationBuilder.DropForeignKey(
            name: "FK_ScriptRuns_SavedScripts_SavedScriptId",
            table: "ScriptRuns");

        migrationBuilder.DropForeignKey(
            name: "FK_ScriptRuns_ScriptSchedules_ScheduleId",
            table: "ScriptRuns");

        migrationBuilder.DropIndex(
            name: "IX_ScriptRuns_SavedScriptId",
            table: "ScriptRuns");

        migrationBuilder.DropIndex(
            name: "IX_ScriptRuns_ScheduleId",
            table: "ScriptRuns");

        migrationBuilder.DropIndex(
            name: "IX_ScriptResults_SavedScriptId",
            table: "ScriptResults");

        migrationBuilder.DropColumn(
            name: "BrandingInfoId",
            table: "Organizations");

        migrationBuilder.AlterColumn<byte[]>(
            name: "FileContents",
            table: "SharedFiles",
            type: "BLOB",
            nullable: true,
            oldClrType: typeof(byte[]),
            oldType: "BLOB");

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "ScriptSchedules",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "ScriptSchedules",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "CreatorId",
            table: "ScriptSchedules",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "ScriptRuns",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AddColumn<int>(
            name: "ScriptScheduleId",
            table: "ScriptRuns",
            type: "INTEGER",
            nullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ScriptInput",
            table: "ScriptResults",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "ScriptResults",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "DeviceID",
            table: "ScriptResults",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "SavedScripts",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "CreatorId",
            table: "SavedScripts",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationName",
            table: "Organizations",
            type: "TEXT",
            maxLength: 25,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 25);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "InviteLinks",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "Devices",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "MacAddresses",
            table: "Devices",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "DeviceGroups",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "DeviceGroups",
            type: "TEXT",
            maxLength: 200,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 200);

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "ApiTokens",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "UserID",
            table: "Alerts",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "OrganizationID",
            table: "Alerts",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "DeviceID",
            table: "Alerts",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.CreateTable(
            name: "DeviceScriptRun1",
            columns: table => new
            {
                DevicesCompletedID = table.Column<string>(type: "TEXT", nullable: false),
                ScriptRunsCompletedId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeviceScriptRun1", x => new { x.DevicesCompletedID, x.ScriptRunsCompletedId });
                table.ForeignKey(
                    name: "FK_DeviceScriptRun1_Devices_DevicesCompletedID",
                    column: x => x.DevicesCompletedID,
                    principalTable: "Devices",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_DeviceScriptRun1_ScriptRuns_ScriptRunsCompletedId",
                    column: x => x.ScriptRunsCompletedId,
                    principalTable: "ScriptRuns",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ScriptRuns_ScriptScheduleId",
            table: "ScriptRuns",
            column: "ScriptScheduleId");

        migrationBuilder.CreateIndex(
            name: "IX_DeviceScriptRun1_ScriptRunsCompletedId",
            table: "DeviceScriptRun1",
            column: "ScriptRunsCompletedId");

        migrationBuilder.AddForeignKey(
            name: "FK_Alerts_Organizations_OrganizationID",
            table: "Alerts",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID");

        migrationBuilder.AddForeignKey(
            name: "FK_ApiTokens_Organizations_OrganizationID",
            table: "ApiTokens",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID");

        migrationBuilder.AddForeignKey(
            name: "FK_Devices_Organizations_OrganizationID",
            table: "Devices",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID");

        migrationBuilder.AddForeignKey(
            name: "FK_InviteLinks_Organizations_OrganizationID",
            table: "InviteLinks",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID");

        migrationBuilder.AddForeignKey(
            name: "FK_RemotelyUsers_Organizations_OrganizationID",
            table: "RemotelyUsers",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID");

        migrationBuilder.AddForeignKey(
            name: "FK_SavedScripts_Organizations_OrganizationID",
            table: "SavedScripts",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID");

        migrationBuilder.AddForeignKey(
            name: "FK_ScriptResults_Organizations_OrganizationID",
            table: "ScriptResults",
            column: "OrganizationID",
            principalTable: "Organizations",
            principalColumn: "ID");

        migrationBuilder.AddForeignKey(
            name: "FK_ScriptRuns_ScriptSchedules_ScriptScheduleId",
            table: "ScriptRuns",
            column: "ScriptScheduleId",
            principalTable: "ScriptSchedules",
            principalColumn: "Id");
    }
}
