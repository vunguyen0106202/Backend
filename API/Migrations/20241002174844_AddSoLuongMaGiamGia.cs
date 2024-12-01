using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddSoLuongMaGiamGia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoLuong",
                table: "MaGiamGias",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoLuong",
                table: "MaGiamGias");
        }
    }
}
