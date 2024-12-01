using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddToNhanHieu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "NhanHieus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "NhanHieus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "NhanHieus",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "NhanHieus",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "NhanHieus",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "NhanHieus");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "NhanHieus");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "NhanHieus");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "NhanHieus");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "NhanHieus");
        }
    }
}
