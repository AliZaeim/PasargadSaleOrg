using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class Br2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrName = table.Column<string>(maxLength: 100, nullable: false),
                    BrCode = table.Column<string>(maxLength: 50, nullable: false),
                    BrField = table.Column<string>(maxLength: 100, nullable: false),
                    BrAttribution = table.Column<string>(nullable: true),
                    ManagerName = table.Column<string>(maxLength: 100, nullable: false),
                    DateofStablishment = table.Column<string>(maxLength: 20, nullable: true),
                    Phone = table.Column<string>(maxLength: 50, nullable: false),
                    Fax = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 10, 22, 16, 7, 14, 348, DateTimeKind.Local).AddTicks(1255));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 10, 22, 16, 7, 14, 343, DateTimeKind.Local).AddTicks(3466));
        }
    }
}
