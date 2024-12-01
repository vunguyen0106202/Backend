using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class themMaHoaDon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaHoaDon",
                table: "HoaDons",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaHoaDon",
                table: "HoaDons");
        }
    }
}
