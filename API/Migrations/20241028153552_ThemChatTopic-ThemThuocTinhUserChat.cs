using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class ThemChatTopicThemThuocTinhUserChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatTopicId",
                table: "UserChats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "UserChats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdOrder",
                table: "UserChats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "UserChats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "UserChats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatTopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    IdOrder = table.Column<int>(type: "int", nullable: false),
                    TimeChat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatTopics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserChats_ChatTopicId",
                table: "UserChats",
                column: "ChatTopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChats_ChatTopics_ChatTopicId",
                table: "UserChats",
                column: "ChatTopicId",
                principalTable: "ChatTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChats_ChatTopics_ChatTopicId",
                table: "UserChats");

            migrationBuilder.DropTable(
                name: "ChatTopics");

            migrationBuilder.DropIndex(
                name: "IX_UserChats_ChatTopicId",
                table: "UserChats");

            migrationBuilder.DropColumn(
                name: "ChatTopicId",
                table: "UserChats");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "UserChats");

            migrationBuilder.DropColumn(
                name: "IdOrder",
                table: "UserChats");

            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "UserChats");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "UserChats");
        }
    }
}
