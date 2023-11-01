using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class chPas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 15, 22, 16, 23, 818, DateTimeKind.Local).AddTicks(1563));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "RegDate" },
                values: new object[] { "UI4H5HBSVCVB", new DateTime(2020, 11, 15, 22, 16, 23, 812, DateTimeKind.Local).AddTicks(4030) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 10, 13, 43, 28, 977, DateTimeKind.Local).AddTicks(5627));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "RegDate" },
                values: new object[] { "0491579241", new DateTime(2020, 11, 10, 13, 43, 28, 971, DateTimeKind.Local).AddTicks(730) });
        }
    }
}
