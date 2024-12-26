using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class ThemKieuThanhToanChoDonHang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPayed",
                table: "HoaDons",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiThanhToan",
                table: "HoaDons",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPayed",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "LoaiThanhToan",
                table: "HoaDons");
        }
    }
}
