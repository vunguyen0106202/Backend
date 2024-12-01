using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddMoTaToNhaCungCap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "NhaCungCaps",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "NhaCungCaps",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "NhaCungCaps",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "NhaCungCaps",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "NhaCungCaps",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "NhaCungCaps");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "NhaCungCaps");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "NhaCungCaps");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "NhaCungCaps");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "NhaCungCaps");
        }
    }
}
