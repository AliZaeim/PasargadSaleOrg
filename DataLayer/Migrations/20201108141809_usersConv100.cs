using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class usersConv100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersConversations_Users_UserId",
                table: "UsersConversations");

            migrationBuilder.DropIndex(
                name: "IX_UsersConversations_UserId",
                table: "UsersConversations");

            migrationBuilder.DropColumn(
                name: "UserFullPro",
                table: "UsersConversations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UsersConversations");

            migrationBuilder.AddColumn<string>(
                name: "RecipientCode",
                table: "UsersConversations",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientFullPro",
                table: "UsersConversations",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderCode",
                table: "UsersConversations",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderFullPro",
                table: "UsersConversations",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 8, 17, 48, 6, 774, DateTimeKind.Local).AddTicks(5356));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 8, 17, 48, 6, 769, DateTimeKind.Local).AddTicks(2677));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientCode",
                table: "UsersConversations");

            migrationBuilder.DropColumn(
                name: "RecipientFullPro",
                table: "UsersConversations");

            migrationBuilder.DropColumn(
                name: "SenderCode",
                table: "UsersConversations");

            migrationBuilder.DropColumn(
                name: "SenderFullPro",
                table: "UsersConversations");

            migrationBuilder.AddColumn<string>(
                name: "UserFullPro",
                table: "UsersConversations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UsersConversations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 7, 17, 30, 38, 152, DateTimeKind.Local).AddTicks(1997));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 7, 17, 30, 38, 146, DateTimeKind.Local).AddTicks(9383));

            migrationBuilder.CreateIndex(
                name: "IX_UsersConversations_UserId",
                table: "UsersConversations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersConversations_Users_UserId",
                table: "UsersConversations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
