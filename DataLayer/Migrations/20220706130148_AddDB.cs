using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class AddDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2022, 7, 6, 17, 31, 47, 150, DateTimeKind.Local).AddTicks(9880));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2022, 7, 6, 17, 31, 47, 149, DateTimeKind.Local).AddTicks(1504));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 16, 12, 6, 54, 803, DateTimeKind.Local).AddTicks(6698));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 16, 12, 6, 54, 798, DateTimeKind.Local).AddTicks(3037));
        }
    }
}
