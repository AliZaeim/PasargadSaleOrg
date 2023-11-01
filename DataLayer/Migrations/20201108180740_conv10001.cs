using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class conv10001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersConversations_UsersConversations_ParentId",
                table: "UsersConversations");

            migrationBuilder.DropTable(
                name: "UserConversation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersConversations",
                table: "UsersConversations");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "UsersConversations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UsersConversations");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "UsersConversations");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "UsersConversations");

            migrationBuilder.RenameTable(
                name: "UsersConversations",
                newName: "Conversations");

            migrationBuilder.RenameIndex(
                name: "IX_UsersConversations_ParentId",
                table: "Conversations",
                newName: "IX_Conversations_ParentId");

            migrationBuilder.AlterColumn<string>(
                name: "SenderFullPro",
                table: "Conversations",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SenderCode",
                table: "Conversations",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Conversations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RecepiesInfo",
                table: "Conversations",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 8, 21, 37, 38, 271, DateTimeKind.Local).AddTicks(5188));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 8, 21, 37, 38, 266, DateTimeKind.Local).AddTicks(1849));

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Conversations_ParentId",
                table: "Conversations",
                column: "ParentId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Conversations_ParentId",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "RecepiesInfo",
                table: "Conversations");

            migrationBuilder.RenameTable(
                name: "Conversations",
                newName: "UsersConversations");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_ParentId",
                table: "UsersConversations",
                newName: "IX_UsersConversations_ParentId");

            migrationBuilder.AlterColumn<string>(
                name: "SenderFullPro",
                table: "UsersConversations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SenderCode",
                table: "UsersConversations",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "UsersConversations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UsersConversations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "UsersConversations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "UsersConversations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersConversations",
                table: "UsersConversations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserConversation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConversation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConversation_UsersConversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "UsersConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserConversation_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "URId",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 11, 8, 20, 38, 35, 554, DateTimeKind.Local).AddTicks(2381));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegDate",
                value: new DateTime(2020, 11, 8, 20, 38, 35, 549, DateTimeKind.Local).AddTicks(4037));

            migrationBuilder.CreateIndex(
                name: "IX_UserConversation_ConversationId",
                table: "UserConversation",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConversation_UserId",
                table: "UserConversation",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersConversations_UsersConversations_ParentId",
                table: "UsersConversations",
                column: "ParentId",
                principalTable: "UsersConversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
