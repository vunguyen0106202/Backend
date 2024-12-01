using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace API.Migrations
{
    public partial class AddMoTaToLoai : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {  
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Loais",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Loais",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Loais",
                nullable: true,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Loais",
                nullable: true,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Loais",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {  
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Loais");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Loais");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Loais");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Loais");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Loais");
        }
    }
}
