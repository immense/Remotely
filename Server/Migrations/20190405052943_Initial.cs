using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Remotely.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    OrganizationName = table.Column<string>(maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommandContexts",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    CommandMode = table.Column<string>(nullable: true),
                    CommandText = table.Column<string>(nullable: true),
                    SenderUserID = table.Column<string>(nullable: true),
                    SenderConnectionID = table.Column<string>(nullable: true),
                    TargetDeviceIDs = table.Column<string>(nullable: true),
                    PSCoreResults = table.Column<string>(nullable: true),
                    CommandResults = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    OrganizationID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandContexts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CommandContexts_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    CurrentUser = table.Column<string>(nullable: true),
                    Drives = table.Column<string>(nullable: true),
                    FreeMemory = table.Column<double>(nullable: false),
                    FreeStorage = table.Column<double>(nullable: false),
                    Is64Bit = table.Column<bool>(nullable: false),
                    IsOnline = table.Column<bool>(nullable: false),
                    LastOnline = table.Column<DateTime>(nullable: false),
                    DeviceName = table.Column<string>(nullable: true),
                    OrganizationID = table.Column<string>(nullable: true),
                    OSArchitecture = table.Column<int>(nullable: false),
                    OSDescription = table.Column<string>(nullable: true),
                    Platform = table.Column<string>(nullable: true),
                    ProcessorCount = table.Column<int>(nullable: false),
                    ServerVerificationToken = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(maxLength: 200, nullable: true),
                    TotalMemory = table.Column<double>(nullable: false),
                    TotalStorage = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Devices_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventLogs",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    EventType = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true),
                    OrganizationID = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EventLogs_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InviteLinks",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    InvitedUser = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    DateSent = table.Column<DateTime>(nullable: false),
                    OrganizationID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InviteLinks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InviteLinks_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PermissionGroups",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    OrganizationID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PermissionGroups_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RemotelyUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    UserOptions = table.Column<string>(nullable: true),
                    OrganizationID = table.Column<string>(nullable: true),
                    IsAdministrator = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemotelyUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemotelyUsers_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SharedFiles",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    FileContents = table.Column<byte[]>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    OrganizationID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedFiles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SharedFiles_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DevicePermissionLinks",
                columns: table => new
                {
                    DeviceID = table.Column<string>(nullable: false),
                    PermissionGroupID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicePermissionLinks", x => new { x.PermissionGroupID, x.DeviceID });
                    table.ForeignKey(
                        name: "FK_DevicePermissionLinks_Devices_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DevicePermissionLinks_PermissionGroups_PermissionGroupID",
                        column: x => x.PermissionGroupID,
                        principalTable: "PermissionGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_RemotelyUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "RemotelyUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_RemotelyUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "RemotelyUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_RemotelyUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "RemotelyUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_RemotelyUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "RemotelyUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissionLinks",
                columns: table => new
                {
                    RemotelyUserID = table.Column<string>(nullable: false),
                    PermissionGroupID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionLinks", x => new { x.PermissionGroupID, x.RemotelyUserID });
                    table.ForeignKey(
                        name: "FK_UserPermissionLinks_PermissionGroups_PermissionGroupID",
                        column: x => x.PermissionGroupID,
                        principalTable: "PermissionGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissionLinks_RemotelyUsers_RemotelyUserID",
                        column: x => x.RemotelyUserID,
                        principalTable: "RemotelyUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CommandContexts_OrganizationID",
                table: "CommandContexts",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_DevicePermissionLinks_DeviceID",
                table: "DevicePermissionLinks",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_OrganizationID",
                table: "Devices",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_OrganizationID",
                table: "EventLogs",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_InviteLinks_OrganizationID",
                table: "InviteLinks",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionGroups_OrganizationID",
                table: "PermissionGroups",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "RemotelyUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "RemotelyUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RemotelyUsers_OrganizationID",
                table: "RemotelyUsers",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_SharedFiles_OrganizationID",
                table: "SharedFiles",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionLinks_RemotelyUserID",
                table: "UserPermissionLinks",
                column: "RemotelyUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CommandContexts");

            migrationBuilder.DropTable(
                name: "DevicePermissionLinks");

            migrationBuilder.DropTable(
                name: "EventLogs");

            migrationBuilder.DropTable(
                name: "InviteLinks");

            migrationBuilder.DropTable(
                name: "SharedFiles");

            migrationBuilder.DropTable(
                name: "UserPermissionLinks");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "PermissionGroups");

            migrationBuilder.DropTable(
                name: "RemotelyUsers");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
