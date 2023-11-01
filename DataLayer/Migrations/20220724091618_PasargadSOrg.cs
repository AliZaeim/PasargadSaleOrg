using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class PasargadSOrg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2022, 7, 24, 13, 46, 17, 35, DateTimeKind.Local).AddTicks(132));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2022, 7, 24, 13, 46, 17, 32, DateTimeKind.Local).AddTicks(9967));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
