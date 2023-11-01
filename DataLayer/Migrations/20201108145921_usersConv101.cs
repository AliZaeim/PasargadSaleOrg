using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class usersConv101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientCode",
                table: "UsersConversations");

            migrationBuilder.DropColumn(
                name: "RecipientFullPro",
                table: "UsersConversations");

            migrationBuilder.AddColumn<string>(
                name: "RecipientsCodeInfo",
                table: "UsersConversations",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 8, 18, 29, 19, 473, DateTimeKind.Local).AddTicks(9463));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 8, 18, 29, 19, 468, DateTimeKind.Local).AddTicks(5815));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientsCodeInfo",
                table: "UsersConversations");

            migrationBuilder.AddColumn<string>(
                name: "RecipientCode",
                table: "UsersConversations",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientFullPro",
                table: "UsersConversations",
                type: "nvarchar(50)",
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
    }
}
