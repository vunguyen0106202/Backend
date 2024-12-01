using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddPropertiesSanPham : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SanPhams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "SanPhams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SanPhams",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "SanPhams",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "SanPhams",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "SanPhams");
        }
    }
}
