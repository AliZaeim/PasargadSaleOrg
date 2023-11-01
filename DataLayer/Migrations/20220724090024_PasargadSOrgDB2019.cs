using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class PasargadSOrgDB2019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2022, 7, 24, 13, 30, 23, 450, DateTimeKind.Local).AddTicks(1870));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2022, 7, 24, 13, 30, 23, 448, DateTimeKind.Local).AddTicks(2079));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2022, 7, 24, 13, 28, 58, 309, DateTimeKind.Local).AddTicks(3876));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2022, 7, 24, 13, 28, 58, 306, DateTimeKind.Local).AddTicks(8439));
        }
    }
}
