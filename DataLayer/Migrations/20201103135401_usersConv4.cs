using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class usersConv4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 3, 17, 23, 59, 84, DateTimeKind.Local).AddTicks(6156));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 3, 17, 23, 59, 79, DateTimeKind.Local).AddTicks(5434));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 3, 17, 13, 37, 174, DateTimeKind.Local).AddTicks(6832));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 3, 17, 13, 37, 169, DateTimeKind.Local).AddTicks(494));
        }
    }
}
