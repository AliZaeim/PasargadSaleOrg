using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class usersConv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersConversationWithAdmins");

            migrationBuilder.CreateTable(
                name: "UsersConversations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    UserFullPro = table.Column<string>(maxLength: 100, nullable: false),
                    Subject = table.Column<string>(maxLength: 100, nullable: false),
                    Message = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ParentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersConversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersConversations_UsersConversations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "UsersConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersConversations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 3, 17, 8, 36, 725, DateTimeKind.Local).AddTicks(5699));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 3, 17, 8, 36, 720, DateTimeKind.Local).AddTicks(5629));

            migrationBuilder.CreateIndex(
                name: "IX_UsersConversations_ParentId",
                table: "UsersConversations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersConversations_UserId",
                table: "UsersConversations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersConversations");

            migrationBuilder.CreateTable(
                name: "UsersConversationWithAdmins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    UserFullPro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersConversationWithAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersConversationWithAdmins_UsersConversationWithAdmins_ParentId",
                        column: x => x.ParentId,
                        principalTable: "UsersConversationWithAdmins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersConversationWithAdmins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 3, 17, 2, 59, 882, DateTimeKind.Local).AddTicks(4798));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 3, 17, 2, 59, 877, DateTimeKind.Local).AddTicks(6445));

            migrationBuilder.CreateIndex(
                name: "IX_UsersConversationWithAdmins_ParentId",
                table: "UsersConversationWithAdmins",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersConversationWithAdmins_UserId",
                table: "UsersConversationWithAdmins",
                column: "UserId");
        }
    }
}
