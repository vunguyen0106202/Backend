﻿using API.Dtos;
using API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace API.Data
{
    public interface IDataConnector
    {
        Task<List<TenSPSoLanXuatHienTrongDonHang>> GetSoLanXuatHienTrongDonHang();
        Task<List<TenSanPhamDoanhSo>> Top10SanPhamLoiNhats();
        Task<List<SanPhamTonKho>>Top10SanPhamTonNhats();
        Task<List<NhanHieuBanChayNhatTrongNam>> GetNhanHieuBanChayNhatTrongNam();
        Task<List<DataSetBanRaTonKho>> DataDataSetBanRaTonKho();
        Task<List<NhaCungCapSoLuong>> GetNhaCungCapSoLuongs();
        Task<List<NhaCungCapTongTien>> GetDoanhSoBans();
        Task<NamSoTongTien> GetNamTongTien();
        Task<MotHoaDon> HoaDonDetailAsync(int id);
        Task<MotHoaDon> GetOneOrder(int id);
    }
    /// <summary>
    /// this is class has some custom data connect
    /// </summary>
    public class DataConnector: IDataConnector
    {
        private readonly DPContext _context;
        public DataConnector(DPContext context)
        {
            _context = context;
        }
        public async Task<List<TenSPSoLanXuatHienTrongDonHang>> GetSoLanXuatHienTrongDonHang()
        {
            var sql = @"select top(10) SanPhams.Ten+' '+Sizes.TenSize+' '+MauSacs.MaMau as 'Ten', sum(ChiTietHoaDons.Soluong) as 'Số lần tồn tại trong đơn hàng'
                        from SanPhams
                        inner join SanPhamBienThes
                        on SanPhams.Id = SanPhamBienThes.Id_SanPham
						inner join MauSacs
                        on SanPhamBienThes.Id_Mau = MauSacs.Id
                        inner join Sizes
                        on SanPhamBienThes.SizeId = Sizes.Id
                        inner join ChiTietHoaDons
                        on SanPhamBienThes.Id = ChiTietHoaDons.Id_SanPhamBienThe
                        inner join HoaDons
					    on ChiTietHoaDons.Id_HoaDon = HoaDons.Id
                         where HoaDons.TrangThai = 2 
                        group by SanPhams.Ten+' '+Sizes.TenSize+' '+MauSacs.MaMau ,SanPhamBienThes.SoLuongTon
                        order by sum(ChiTietHoaDons.Soluong) desc
                       ";
            var solanxuathiens = new List<TenSPSoLanXuatHienTrongDonHang>();
            SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString());
            await conn.OpenAsync();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader;
            reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    solanxuathiens.Add(new TenSPSoLanXuatHienTrongDonHang()
                    {
                        TenSP = (string)reader["Ten"],
                        SoLanXuatHienTrongDonHang = (int)reader["Số lần tồn tại trong đơn hàng"]
                    });
                }
            }
            await conn.CloseAsync();
            return solanxuathiens.ToList();
        }
        public async Task<List<SanPhamTonKho>> Top10SanPhamTonNhats()
        {
            var sql = @"select top(10) SanPhams.Ten+' '+Sizes.TenSize+' '+MauSacs.MaMau as 'Ten',SanPhamBienThes.SoLuongTon
                        from SanPhams
                        inner join SanPhamBienThes
                        on SanPhams.Id = SanPhamBienThes.Id_SanPham
                        inner join MauSacs
                        on SanPhamBienThes.Id_Mau = MauSacs.Id
                        inner join Sizes
                        on SanPhamBienThes.SizeId = Sizes.Id
                        order by SanPhamBienThes.SoLuongTon desc
                       ";
            var soluongton = new List<SanPhamTonKho>();
            SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString());
            await conn.OpenAsync();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader;
            reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    soluongton.Add(new SanPhamTonKho()
                    {
                        Ten = (string)reader["Ten"],
                        SoLuongTon = (int)reader["SoLuongTon"],
                    });
                }
            }
            await conn.CloseAsync();
            return soluongton.ToList();
        }
        public async Task<List<TenSanPhamDoanhSo>> Top10SanPhamLoiNhats()
        {
            string sql = @"
            SELECT TOP(10) 
                SanPhams.Ten + ' ' + Sizes.TenSize + ' ' + MauSacs.MaMau AS Ten, 
                CAST(SUM(ChiTietHoaDons.ThanhTien) AS DECIMAL(18, 2)) AS ThanhTien
            FROM SanPhams
            INNER JOIN SanPhamBienThes ON SanPhams.Id = SanPhamBienThes.Id_SanPham
            INNER JOIN MauSacs ON SanPhamBienThes.Id_Mau = MauSacs.Id
            INNER JOIN Sizes ON SanPhamBienThes.SizeId = Sizes.Id
            INNER JOIN ChiTietHoaDons ON SanPhamBienThes.Id = ChiTietHoaDons.Id_SanPhamBienThe
            INNER JOIN HoaDons ON ChiTietHoaDons.Id_HoaDon = HoaDons.Id
            WHERE HoaDons.TrangThai = 2
            GROUP BY SanPhams.Ten, Sizes.TenSize, MauSacs.MaMau
            ORDER BY CAST(SUM(ChiTietHoaDons.ThanhTien) AS DECIMAL(18, 2)) DESC;
        ";
            SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString());
            List<TenSanPhamDoanhSo> tenspdss = new List<TenSanPhamDoanhSo>();
            SqlCommand cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    tenspdss.Add(
                        new TenSanPhamDoanhSo()
                        {
                            TenSP = (string)reader["Ten"],
                            DoanhSoCaoNhat = (decimal)reader["ThanhTien"]
                        });
                }
            }
            return tenspdss;
        }
        public async Task<List<NhanHieuBanChayNhatTrongNam>> GetNhanHieuBanChayNhatTrongNam()
        {
            string sql = @"select top(5) NhanHieus.Ten, sum(ChiTietHoaDons.Soluong) as 'soluong'
                                from NhanHieus
                                inner join SanPhams
                                    on NhanHieus.Id = SanPhams.Id_NhanHieu
                                inner join SanPhamBienThes
                                    on SanPhamBienThes.Id_SanPham = SanPhams.Id
                                inner join ChiTietHoaDons 
                                    on ChiTietHoaDons.Id_SanPhamBienThe = SanPhamBienThes.Id
                                inner join HoaDons
                                    on HoaDons.Id = ChiTietHoaDons.Id_HoaDon
                                where DATEPART(YYYY, HoaDons.NgayTao) = YEAR(GETDATE()) and HoaDons.TrangThai = 2
                                group by NhanHieus.Ten
                        ";
            SqlConnection cnn;
            cnn = new SqlConnection(_context.Database.GetConnectionString());
            SqlDataReader reader;
            SqlCommand cmd;
            List<NhanHieuBanChayNhatTrongNam> listNH = new List<NhanHieuBanChayNhatTrongNam>();
            await cnn.OpenAsync();
            cmd = new SqlCommand(sql, cnn);
            reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    listNH.Add(new NhanHieuBanChayNhatTrongNam()
                    {
                        Ten = (string)reader["Ten"],
                        SoLuong = (int)reader["soluong"]
                    });
                }
            }
            cnn.Close();
            return listNH;
        }
        public async Task<List<NhanHieuBanChayNhatTrongNam>> GetNhanHieuBanChayNhat(int? year = null)
        {
            // Lấy năm hiện tại nếu không truyền tham số year
            int selectedYear = year ?? DateTime.Now.Year;

            string sql = @"
                select top(10) NhanHieus.Ten, sum(ChiTietHoaDons.Soluong) as 'soluong'
                from NhanHieus
                inner join SanPhams on NhanHieus.Id = SanPhams.Id_NhanHieu
                inner join SanPhamBienThes on SanPhamBienThes.Id_SanPham = SanPhams.Id
                inner join ChiTietHoaDons on ChiTietHoaDons.Id_SanPhamBienThe = SanPhamBienThes.Id
                inner join HoaDons on HoaDons.Id = ChiTietHoaDons.Id_HoaDon
                where DATEPART(YYYY, HoaDons.NgayTao) = @Year and HoaDons.TrangThai = 2
                group by NhanHieus.Ten
            ";

            using SqlConnection cnn = new SqlConnection(_context.Database.GetConnectionString());
            using SqlCommand cmd = new SqlCommand(sql, cnn);

            // Truyền tham số năm vào câu lệnh SQL
            cmd.Parameters.AddWithValue("@Year", selectedYear);

            List<NhanHieuBanChayNhatTrongNam> listNH = new List<NhanHieuBanChayNhatTrongNam>();
            await cnn.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    listNH.Add(new NhanHieuBanChayNhatTrongNam()
                    {
                        Ten = reader["Ten"].ToString(),
                        SoLuong = Convert.ToInt32(reader["soluong"])
                    });
                }
            }

            return listNH;
        }

        public async Task<List<DataSetBanRaTonKho>> DataDataSetBanRaTonKho()
        {
            string sql = @"
                        select top(16) SanPhams.Ten+' '+ChiTietHoaDons.Mau+' '+ChiTietHoaDons.Size as 'Ten',cast(sum(SanPhams.GiaNhap*SanPhamBienThes.SoLuongTon) as decimal(18,2)) as'GiaTriTonKho' ,sum(ChiTietHoaDons.ThanhTien) as'GiaTriBanRa'
                        from ChiTietHoaDons
                        inner join SanPhamBienThes
                        on ChiTietHoaDons.Id_SanPhamBienThe = SanPhamBienThes.Id
                        inner join SanPhams
                        on ChiTietHoaDons.Id_SanPham = SanPhams.Id
                        inner join HoaDons
                        on ChiTietHoaDons.Id_HoaDon = HoaDons.Id
                        where HoaDons.TrangThai = 2
                        group by(SanPhams.Ten+' '+ChiTietHoaDons.Mau+' '+ChiTietHoaDons.Size)
                        order by sum(ChiTietHoaDons.ThanhTien) desc;
                        ";
            SqlConnection cnn;
            cnn = new SqlConnection(_context.Database.GetConnectionString());
            SqlDataReader reader;
            SqlCommand cmd;
            var list = new List<DataSetBanRaTonKho>();
            await cnn.OpenAsync();
            cmd = new SqlCommand(sql, cnn);
            reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new DataSetBanRaTonKho()
                    {
                        Ten = (string)reader["Ten"],
                        GiaTriTonKho = (decimal)reader["GiaTriTonKho"],
                        GiaTriBanRa = (decimal)reader["GiaTriBanRa"]
                    });
                }
            }
            await cnn.CloseAsync();
            return list;
        }
        public async Task<List<NhaCungCapSoLuong>> GetNhaCungCapSoLuongs()
        {
            string sql = @"	select NhaCungCaps.Ten,sum(ChiTietPhieuNhapHangs.SoluongNhap) as 'So luong da nhap'
	                        from NhaCungCaps
	                        inner join PhieuNhapHangs
	                        on NhaCungCaps.Id = PhieuNhapHangs.Id_NhaCungCap
	                        inner join ChiTietPhieuNhapHangs
	                        on NhaCungCaps.Id = ChiTietPhieuNhapHangs.Id_PhieuNhapHang
	                        inner join SanPhamBienThes
	                        on SanPhamBienThes.Id = ChiTietPhieuNhapHangs.Id_PhieuNhapHang
	                        inner join SanPhams
	                        on SanPhams.Id = SanPhamBienThes.Id_SanPham
	                        group by NhaCungCaps.Ten ,SanPhams.GiaNhap
	                        order by sum(ChiTietPhieuNhapHangs.SoluongNhap) desc
";
            SqlConnection cnn;
            cnn = new SqlConnection(_context.Database.GetConnectionString());
            SqlDataReader reader;
            SqlCommand cmd;
            var list = new List<NhaCungCapSoLuong>();
            await cnn.OpenAsync();
            cmd = new SqlCommand(sql, cnn);
            reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new NhaCungCapSoLuong()
                    {
                        Ten = (string)reader["Ten"],
                        SoLuong = (int)reader["So luong da nhap"]
                    });
                }
            }
            await cnn.CloseAsync();
            return list;
        }
        public async Task<List<NhaCungCapTongTien>> GetDoanhSoBans()
        {
            string sql = @"select NhaCungCaps.Ten, sum(PhieuNhapHangs.TongTien) as 'TongTien' 
	                      from PhieuNhapHangs, NhaCungCaps
	                      where PhieuNhapHangs.Id_NhaCungCap = NhaCungCaps.Id
	                      group by NhaCungCaps.Ten
                          order by sum(PhieuNhapHangs.TongTien) desc";
            SqlConnection cnn;
            cnn = new SqlConnection(_context.Database.GetConnectionString());
            SqlDataReader reader;
            SqlCommand cmd;
            var list = new List<NhaCungCapTongTien>();
            await cnn.OpenAsync();
            cmd = new SqlCommand(sql, cnn);
            reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new NhaCungCapTongTien()
                    {
                        Ten = (string)reader["Ten"],
                        TongTien = (decimal)reader["TongTien"]
                    });
                }
            }
            await cnn.CloseAsync();
            return list;
        }
        public async Task<NamSoTongTien> GetNamTongTien()
        {
            int year=DateTime.Now.Year;
            string sql = @"select DATEPART( YYYY,HoaDons.NgayTao) as 'Nam', sum(HoaDons.TongTien) as'Tong tien trong nam'
                            from HoaDons
                            where DATEPART( YYYY,HoaDons.NgayTao)=YEAR(GETDATE()) and HoaDons.TrangThai = 2
                            group by DATEPART( YYYY,HoaDons.NgayTao)
                        ";
            SqlConnection cnn;
            cnn = new SqlConnection(_context.Database.GetConnectionString());
            SqlDataReader reader;
            SqlCommand cmd;
            var nam2021 = new NamSoTongTien();
            await cnn.OpenAsync();
            cmd = new SqlCommand(sql, cnn);
            reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    nam2021.Nam = (int)reader["Nam"];
                    nam2021.TongTien = (decimal)reader["Tong tien trong nam"];
                }
            }
            cnn.Close();
            return nam2021;
        }
        public async Task<NamSoTongTien> GetTongTienTheoNam(int? year = null)
        {
            // Lấy năm hiện tại nếu không truyền tham số
            int selectedYear = year ?? DateTime.Now.Year;

            string sql = @"
        select DATEPART(YYYY, HoaDons.NgayTao) as 'Nam', 
               sum(HoaDons.TongTien) as 'TongTienTrongNam'
        from HoaDons
        where DATEPART(YYYY, HoaDons.NgayTao) = @Year 
              and HoaDons.TrangThai = 2
        group by DATEPART(YYYY, HoaDons.NgayTao)
    ";

            using SqlConnection cnn = new SqlConnection(_context.Database.GetConnectionString());
            using SqlCommand cmd = new SqlCommand(sql, cnn);

            // Truyền tham số năm vào câu lệnh SQL
            cmd.Parameters.AddWithValue("@Year", selectedYear);

            var result = new NamSoTongTien();
            await cnn.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    result.Nam = (int)reader["Nam"];
                    result.TongTien = (decimal)reader["TongTienTrongNam"];
                }
            }

            return result;
        }

        public async Task<MotHoaDon> HoaDonDetailAsync(int id)
        {
            string sql = @"		;with ProductImageTable
	                            as (
		                        SELECT ChiTietHoaDons.Id,SanPhams.Ten,ImageSanPhams.ImageName,Sizes.TenSize,MauSacs.MaMau,ChiTietHoaDons.Soluong,cast(SanPhams.GiaBan as decimal(18,2)) as'GiaBan',ChiTietHoaDons.ThanhTien,
		                        ROW_NUMBER() OVER (PARTITION BY ChiTietHoaDons.Id ORDER BY  ImageSanPhams.Id)  RowNum
		                        FROM SanPhams 
								LEFT JOIN ImageSanPhams 
								ON SanPhams.Id=ImageSanPhams.IdSanPham 
								inner join SanPhamBienThes
								on SanPhamBienThes.Id_SanPham = SanPhams.Id
								inner join Sizes
								on SanPhamBienThes.SizeId = Sizes.Id
								inner join MauSacs
								on SanPhamBienThes.Id_Mau = MauSacs.Id
								inner join ChiTietHoaDons
								on ChiTietHoaDons.Id_SanPhamBienThe = SanPhamBienThes.Id
								inner join HoaDons
								on HoaDons.Id = ChiTietHoaDons.Id_HoaDon
								where ChiTietHoaDons.Id_HoaDon = @value
		                          )
		                        SELECT Id,Ten,ImageName,TenSize,MaMau,Soluong,GiaBan,ThanhTien
		                        from ProductImageTable
	                            where
                                ProductImageTable.RowNum = 1
";
            SqlConnection cnn;
            cnn = new SqlConnection(_context.Database.GetConnectionString());
            SqlDataReader reader;
            SqlCommand cmd;
            var list = new List<NhieuChiTietHoaDon>();
            await cnn.OpenAsync();
            SqlParameter param = new SqlParameter();
            cmd = new SqlCommand(sql, cnn);
            param.ParameterName = "@value";
            param.Value = id;
            cmd.Parameters.Add(param);
            reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new NhieuChiTietHoaDon()
                    {
                        Id = (int)reader["Id"],
                        Ten = (string)reader["Ten"],
                        Hinh = (string)reader["ImageName"],
                        GiaBan = (decimal)reader["GiaBan"],
                        MauSac = (string)reader["MaMau"],
                        Size = (string)reader["TenSize"],
                        SoLuong = (int)reader["SoLuong"],
                        ThanhTien = (decimal)reader["ThanhTien"]
                    });
                }
            }
            await cnn.CloseAsync();
            var hd = from h in _context.HoaDons
                     join us in _context.AppUsers
                     on h.Id_User equals us.Id
                     select new MotHoaDon()
                     {
                         Id = h.Id,
                         FullName = us.LastName + ' ' + us.FirstName,
                         DiaChi = us.DiaChi,
                         Email = us.Email,
                         SDT = us.SDT,
                         hoaDon = new HoaDon()
                         {
                             Id_User = h.Id_User,
                             TongTien = h.TongTien,
                             GhiChu = h.GhiChu,
                             NgayTao = h.NgayTao,
                             TrangThai = h.TrangThai,
                             LoaiThanhToan = h.LoaiThanhToan,
                         },
                         chiTietHoaDons = list,
                     };
            return await hd.FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<MotHoaDon> GetOneOrder(int id)
        {
            string sql = @"		;with ProductImageTable
	                            as (
		                        SELECT ChiTietHoaDons.Id,SanPhams.Ten,ImageSanPhams.ImageName,Sizes.TenSize,MauSacs.MaMau,ChiTietHoaDons.Soluong,cast(SanPhams.GiaBan as decimal(18,2)) as'GiaBan',ChiTietHoaDons.ThanhTien,
		                        ROW_NUMBER() OVER (PARTITION BY ChiTietHoaDons.Id ORDER BY  ImageSanPhams.Id)  RowNum
		                        FROM SanPhams 
								LEFT JOIN ImageSanPhams 
								ON SanPhams.Id=ImageSanPhams.IdSanPham 
								inner join SanPhamBienThes
								on SanPhamBienThes.Id_SanPham = SanPhams.Id
								inner join Sizes
								on SanPhamBienThes.SizeId = Sizes.Id
								inner join MauSacs
								on SanPhamBienThes.Id_Mau = MauSacs.Id
								inner join ChiTietHoaDons
								on ChiTietHoaDons.Id_SanPhamBienThe = SanPhamBienThes.Id
								inner join HoaDons
								on HoaDons.Id = ChiTietHoaDons.Id_HoaDon
								where ChiTietHoaDons.Id_HoaDon = @value
		                          )
		                        SELECT Id,Ten,ImageName,TenSize,MaMau,Soluong,GiaBan,ThanhTien
		                        from ProductImageTable
	                            where
                                ProductImageTable.RowNum = 1
";
            SqlConnection cnn;
            cnn = new SqlConnection(_context.Database.GetConnectionString());
            SqlDataReader reader;
            SqlCommand cmd;
            var list = new List<NhieuChiTietHoaDon>();
            await cnn.OpenAsync();
            SqlParameter param = new SqlParameter();
            cmd = new SqlCommand(sql, cnn);
            param.ParameterName = "@value";
            param.Value = id;
            cmd.Parameters.Add(param);
            reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new NhieuChiTietHoaDon()
                    {
                        Id = (int)reader["Id"],
                        Ten = (string)reader["Ten"],
                        Hinh = (string)reader["ImageName"],
                        GiaBan = (decimal)reader["GiaBan"],
                        MauSac = (string)reader["MaMau"],
                        Size = (string)reader["TenSize"],
                        SoLuong = (int)reader["SoLuong"],
                        ThanhTien = (decimal)reader["ThanhTien"]
                    });
                }
            }
            await cnn.CloseAsync();
            var hd = from h in _context.HoaDons
                     join us in _context.AppUsers
                     on h.Id_User equals us.Id
                     select new MotHoaDon()
                     {
                         Id = h.Id,
                         FullName = us.LastName + ' ' + us.FirstName,
                         DiaChi = us.DiaChi,
                         Email = us.Email,
                         SDT = us.SDT,
                         hoaDon = new HoaDon()
                         {
                             Id_User = h.Id_User,
                             TongTien = h.TongTien,
                             GhiChu = h.GhiChu,
                             NgayTao = h.NgayTao,
                             TrangThai = h.TrangThai
                         },
                         chiTietHoaDons = list,
                     };
            var result = await hd.FirstOrDefaultAsync(s => s.Id == id);
            return result;
        }
    }
}
