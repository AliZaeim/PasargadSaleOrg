using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class usersConv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersConversationWithAdmins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    UserFullPro = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ParentId = table.Column<int>(nullable: true)
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
                value: new DateTime(2020, 11, 3, 17, 1, 38, 562, DateTimeKind.Local).AddTicks(3765));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 3, 17, 1, 38, 556, DateTimeKind.Local).AddTicks(8931));

            migrationBuilder.CreateIndex(
                name: "IX_UsersConversationWithAdmins_ParentId",
                table: "UsersConversationWithAdmins",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersConversationWithAdmins_UserId",
                table: "UsersConversationWithAdmins",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersConversationWithAdmins");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 10, 22, 16, 8, 28, 509, DateTimeKind.Local).AddTicks(345));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 10, 22, 16, 8, 28, 503, DateTimeKind.Local).AddTicks(9218));
        }
    }
}
