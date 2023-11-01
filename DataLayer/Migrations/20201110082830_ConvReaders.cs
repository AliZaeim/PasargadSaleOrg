using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class ConvReaders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Readers",
                table: "Conversations",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 10, 11, 58, 28, 564, DateTimeKind.Local).AddTicks(3582));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 10, 11, 58, 28, 559, DateTimeKind.Local).AddTicks(4617));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Readers",
                table: "Conversations");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 9, 11, 28, 51, 137, DateTimeKind.Local).AddTicks(340));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 9, 11, 28, 51, 131, DateTimeKind.Local).AddTicks(8078));
        }
    }
}
