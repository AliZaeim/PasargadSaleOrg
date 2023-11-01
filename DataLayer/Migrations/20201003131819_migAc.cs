using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class migAc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "OP_Create", "OP_Remove", "OP_Update", "ParentId", "PermissionTitle" },
                values: new object[] { 99, null, null, null, 12, "فعال/غیرفعال کردن" });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 10, 3, 16, 48, 16, 867, DateTimeKind.Local).AddTicks(4732));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 10, 3, 16, 48, 16, 861, DateTimeKind.Local).AddTicks(2256));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 99);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 9, 25, 12, 30, 28, 781, DateTimeKind.Local).AddTicks(6804));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 9, 25, 12, 30, 28, 775, DateTimeKind.Local).AddTicks(5428));
        }
    }
}
