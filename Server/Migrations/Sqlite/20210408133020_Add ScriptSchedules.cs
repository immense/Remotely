using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations.Sqlite
{
    public partial class AddScriptSchedules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandResults");

            migrationBuilder.DropIndex(
                name: "IX_ApiTokens_Token",
                table: "ApiTokens");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "RemotelyUsers");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "ApiTokens");

            migrationBuilder.CreateTable(
                name: "SavedScripts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<string>(type: "TEXT", nullable: true),
                    FolderPath = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    GenerateAlertOnError = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsQuickScript = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OrganizationID = table.Column<string>(type: "TEXT", nullable: true),
                    SendEmailOnError = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendErrorEmailTo = table.Column<string>(type: "TEXT", nullable: true),
                    Shell = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedScripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedScripts_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SavedScripts_RemotelyUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "RemotelyUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScriptSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<string>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<string>(type: "TEXT", nullable: true),
                    Interval = table.Column<int>(type: "INTEGER", nullable: false),
                    LastRun = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    NextRun = table.Column<string>(type: "TEXT", nullable: false),
                    StartAt = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationID = table.Column<string>(type: "TEXT", nullable: true),
                    RunOnNextConnect = table.Column<bool>(type: "INTEGER", nullable: false),
                    SavedScriptId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptSchedules_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScriptSchedules_RemotelyUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "RemotelyUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceGroupScriptSchedule",
                columns: table => new
                {
                    DeviceGroupsID = table.Column<string>(type: "TEXT", nullable: false),
                    ScriptSchedulesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceGroupScriptSchedule", x => new { x.DeviceGroupsID, x.ScriptSchedulesId });
                    table.ForeignKey(
                        name: "FK_DeviceGroupScriptSchedule_DeviceGroups_DeviceGroupsID",
                        column: x => x.DeviceGroupsID,
                        principalTable: "DeviceGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceGroupScriptSchedule_ScriptSchedules_ScriptSchedulesId",
                        column: x => x.ScriptSchedulesId,
                        principalTable: "ScriptSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceScriptSchedule",
                columns: table => new
                {
                    DevicesID = table.Column<string>(type: "TEXT", nullable: false),
                    ScriptSchedulesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceScriptSchedule", x => new { x.DevicesID, x.ScriptSchedulesId });
                    table.ForeignKey(
                        name: "FK_DeviceScriptSchedule_Devices_DevicesID",
                        column: x => x.DevicesID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceScriptSchedule_ScriptSchedules_ScriptSchedulesId",
                        column: x => x.ScriptSchedulesId,
                        principalTable: "ScriptSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScriptRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Initiator = table.Column<string>(type: "TEXT", nullable: true),
                    InputType = table.Column<int>(type: "INTEGER", nullable: false),
                    OrganizationID = table.Column<string>(type: "TEXT", nullable: true),
                    RunAt = table.Column<string>(type: "TEXT", nullable: false),
                    RunOnNextConnect = table.Column<bool>(type: "INTEGER", nullable: false),
                    SavedScriptId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ScheduleId = table.Column<int>(type: "INTEGER", nullable: true),
                    ScriptScheduleId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptRuns_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScriptRuns_ScriptSchedules_ScriptScheduleId",
                        column: x => x.ScriptScheduleId,
                        principalTable: "ScriptSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceScriptRun",
                columns: table => new
                {
                    DevicesID = table.Column<string>(type: "TEXT", nullable: false),
                    ScriptRunsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceScriptRun", x => new { x.DevicesID, x.ScriptRunsId });
                    table.ForeignKey(
                        name: "FK_DeviceScriptRun_Devices_DevicesID",
                        column: x => x.DevicesID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceScriptRun_ScriptRuns_ScriptRunsId",
                        column: x => x.ScriptRunsId,
                        principalTable: "ScriptRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "ScriptResults",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceID = table.Column<string>(type: "TEXT", nullable: true),
                    ErrorOutput = table.Column<string>(type: "TEXT", nullable: true),
                    HadErrors = table.Column<bool>(type: "INTEGER", nullable: false),
                    InputType = table.Column<int>(type: "INTEGER", nullable: false),
                    OrganizationID = table.Column<string>(type: "TEXT", nullable: true),
                    RunTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    ScriptInput = table.Column<string>(type: "TEXT", nullable: true),
                    ScheduleId = table.Column<int>(type: "INTEGER", nullable: true),
                    SavedScriptId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ScriptRunId = table.Column<int>(type: "INTEGER", nullable: true),
                    SenderConnectionID = table.Column<string>(type: "TEXT", nullable: true),
                    SenderUserName = table.Column<string>(type: "TEXT", nullable: true),
                    Shell = table.Column<int>(type: "INTEGER", nullable: false),
                    StandardOutput = table.Column<string>(type: "TEXT", nullable: true),
                    TimeStamp = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ScriptResults_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScriptResults_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScriptResults_ScriptRuns_ScriptRunId",
                        column: x => x.ScriptRunId,
                        principalTable: "ScriptRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScriptResults_ScriptSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "ScriptSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceGroupScriptSchedule_ScriptSchedulesId",
                table: "DeviceGroupScriptSchedule",
                column: "ScriptSchedulesId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceScriptRun_ScriptRunsId",
                table: "DeviceScriptRun",
                column: "ScriptRunsId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceScriptRun1_ScriptRunsCompletedId",
                table: "DeviceScriptRun1",
                column: "ScriptRunsCompletedId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceScriptSchedule_ScriptSchedulesId",
                table: "DeviceScriptSchedule",
                column: "ScriptSchedulesId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedScripts_CreatorId",
                table: "SavedScripts",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedScripts_OrganizationID",
                table: "SavedScripts",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptResults_DeviceID",
                table: "ScriptResults",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptResults_OrganizationID",
                table: "ScriptResults",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptResults_ScheduleId",
                table: "ScriptResults",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptResults_ScriptRunId",
                table: "ScriptResults",
                column: "ScriptRunId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptRuns_OrganizationID",
                table: "ScriptRuns",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptRuns_ScriptScheduleId",
                table: "ScriptRuns",
                column: "ScriptScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptSchedules_CreatorId",
                table: "ScriptSchedules",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptSchedules_OrganizationID",
                table: "ScriptSchedules",
                column: "OrganizationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceGroupScriptSchedule");

            migrationBuilder.DropTable(
                name: "DeviceScriptRun");

            migrationBuilder.DropTable(
                name: "DeviceScriptRun1");

            migrationBuilder.DropTable(
                name: "DeviceScriptSchedule");

            migrationBuilder.DropTable(
                name: "SavedScripts");

            migrationBuilder.DropTable(
                name: "ScriptResults");

            migrationBuilder.DropTable(
                name: "ScriptRuns");

            migrationBuilder.DropTable(
                name: "ScriptSchedules");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "RemotelyUsers",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "ApiTokens",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommandResults",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false),
                    CommandMode = table.Column<string>(type: "TEXT", nullable: true),
                    CommandResults = table.Column<string>(type: "TEXT", nullable: true),
                    CommandText = table.Column<string>(type: "TEXT", nullable: true),
                    OrganizationID = table.Column<string>(type: "TEXT", nullable: true),
                    PSCoreResults = table.Column<string>(type: "TEXT", nullable: true),
                    SenderConnectionID = table.Column<string>(type: "TEXT", nullable: true),
                    SenderUserID = table.Column<string>(type: "TEXT", nullable: true),
                    TargetDeviceIDs = table.Column<string>(type: "TEXT", nullable: true),
                    TimeStamp = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CommandResults_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiTokens_Token",
                table: "ApiTokens",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_CommandResults_OrganizationID",
                table: "CommandResults",
                column: "OrganizationID");
        }
    }
}
