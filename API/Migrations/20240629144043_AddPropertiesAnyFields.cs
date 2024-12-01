using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddPropertiesAnyFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "UserLikes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "UserLikes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserLikes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "UserLikes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "UserLikes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "UserComments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "UserComments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserComments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "UserComments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "UserComments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "UserChats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "UserChats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserChats",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "UserChats",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "UserChats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Sizes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Sizes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Sizes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Sizes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Sizes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SanPhamBienThes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "SanPhamBienThes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SanPhamBienThes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "SanPhamBienThes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "SanPhamBienThes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PhieuNhapHangs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "PhieuNhapHangs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PhieuNhapHangs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "PhieuNhapHangs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "PhieuNhapHangs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "MauSacs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "MauSacs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MauSacs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "MauSacs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "MauSacs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "MaGiamGias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "MaGiamGias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MaGiamGias",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "MaGiamGias",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "MaGiamGias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "JobSeekers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "JobSeekers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "JobSeekers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "JobSeekers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "JobSeekers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ImageSanPhams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ImageSanPhams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ImageSanPhams",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "ImageSanPhams",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ImageSanPhams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ImageBlogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ImageBlogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ImageBlogs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "ImageBlogs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ImageBlogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "HoaDons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "HoaDons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "HoaDons",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "HoaDons",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "HoaDons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ChiTietPhieuNhapHangs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ChiTietPhieuNhapHangs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ChiTietPhieuNhapHangs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "ChiTietPhieuNhapHangs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ChiTietPhieuNhapHangs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ChiTietHoaDons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ChiTietHoaDons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ChiTietHoaDons",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "ChiTietHoaDons",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ChiTietHoaDons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Carts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Carts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Carts",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Carts",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Carts",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "UserComments");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "UserComments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserComments");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "UserComments");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "UserComments");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "UserChats");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "UserChats");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserChats");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "UserChats");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "UserChats");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SanPhamBienThes");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "SanPhamBienThes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SanPhamBienThes");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "SanPhamBienThes");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "SanPhamBienThes");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PhieuNhapHangs");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "PhieuNhapHangs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PhieuNhapHangs");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "PhieuNhapHangs");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "PhieuNhapHangs");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "MauSacs");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "MauSacs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MauSacs");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "MauSacs");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "MauSacs");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "MaGiamGias");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "MaGiamGias");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MaGiamGias");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "MaGiamGias");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "MaGiamGias");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ImageSanPhams");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ImageSanPhams");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ImageSanPhams");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "ImageSanPhams");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ImageSanPhams");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ImageBlogs");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ImageBlogs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ImageBlogs");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "ImageBlogs");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ImageBlogs");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ChiTietPhieuNhapHangs");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ChiTietPhieuNhapHangs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ChiTietPhieuNhapHangs");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "ChiTietPhieuNhapHangs");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ChiTietPhieuNhapHangs");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ChiTietHoaDons");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ChiTietHoaDons");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ChiTietHoaDons");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "ChiTietHoaDons");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ChiTietHoaDons");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Carts");
        }
    }
}
