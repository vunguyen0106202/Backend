using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using OfficeOpenXml;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Helpers;
using API.Models;
using API.Helper.SignalR;
using API.Helper.Result;
using System.Reflection;
using API.Helper;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonsController : Controller
    {
        private readonly DPContext _context;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly IDataConnector _connector;
        public HoaDonsController(DPContext context, IHubContext<BroadcastHub, IHubClient> hubContext, IDataConnector connector)
        {
            this._context = context;
            this._hubContext = hubContext;
            _connector = connector;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDonUser>>> AllHoaDons(string key, int offset, int limit = 20, string sortField = "", string sortOrder = "asc")
        {
            var query = _context.HoaDons
                .Where(hd=>hd.IsActive==true && hd.IsDelete==false)
                .Join(_context.AppUsers,
                      hd => hd.Id_User,
                      us => us.Id,
                      (hd, us) => new HoaDonUser
                      {
                          GhiChu = hd.GhiChu,
                          Id = hd.Id,
                          MaHoaDon = hd.MaHoaDon,
                          NgayTao = hd.NgayTao,
                          TrangThai = hd.TrangThai,
                          TongTien = hd.TongTien,
                          FullName = us.FirstName + " " + us.LastName,
                      })
                .Where(hd => string.IsNullOrEmpty(key) || hd.MaHoaDon.Contains(key) || hd.FullName.Contains(key));

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField)
                {
                    case "maHoaDon":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.MaHoaDon) : query.OrderByDescending(s => s.MaHoaDon);
                        break;
                    case "ngayTao":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.NgayTao) : query.OrderByDescending(s => s.NgayTao);
                        break;
                    case "tongTien":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.TongTien) : query.OrderByDescending(s => s.TongTien);
                        break;
                    case "trangThai":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.TrangThai) : query.OrderByDescending(s => s.TrangThai);
                        break;
                    default:
                        query = query.OrderBy(s => s.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(s => s.Id);
            }

            query = query.Skip(offset).Take(limit);

            var totalCount = await _context.HoaDons
                 .Where(hd => hd.IsActive == true && hd.IsDelete == false)
                 .Join(_context.AppUsers,
                       hd => hd.Id_User,
                       us => us.Id,
                       (hd, us) => new { hd, us })
                 .Where(joined => string.IsNullOrEmpty(key) || joined.hd.MaHoaDon.Contains(key) || (joined.us.FirstName + " " + joined.us.LastName).Contains(key))
                 .CountAsync();

            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<HoaDonUser>>.Success(results, totalCount, ""));
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<HoaDonUser>>> AllHoaDons()
        //{
        //    var kb = from hd in _context.HoaDons
        //             join us in _context.AppUsers
        //             on hd.Id_User equals us.Id
        //             select new HoaDonUser()
        //             {
        //                 GhiChu = hd.GhiChu,
        //                 Id = hd.Id,
        //                 NgayTao = hd.NgayTao,
        //                 TrangThai = hd.TrangThai,
        //                 TongTien = hd.TongTien,
        //                 FullName = us.FirstName + ' ' + us.LastName,
        //             };
        //    return await kb.ToListAsync();
        //}

        [HttpGet("admindetailorder/{id}")]
        public async Task<ActionResult<MotHoaDon>> HoaDonDetailAsync(int id)
        {
            return await _connector.HoaDonDetailAsync(id);
        }
        [HttpGet("hoadon/{id}")]
        public async Task<ActionResult> ChitietHoaDon(int id)
        {
            var resuft = await _context.HoaDons.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (resuft == null)
            {
                return Ok(Result<object>.Error("Hóa đơn không tồn tại."));
            }
            resuft.User = await _context.AppUsers.Where(d => d.Id == resuft.Id_User).FirstOrDefaultAsync();


            return Ok(Result<object>.Success(resuft, 1, ""));
        }
        [HttpPost("danhsachhoadon")]
        public async Task<ActionResult> ListHoaDon(UserDto user)
        {
            var resuft = await _context.HoaDons.Where(d => d.Id_User == user.idUser).ToListAsync();
            return Json(resuft);
        }
        [HttpPut("suatrangthai/{id}")]
        public async Task<IActionResult> SuaTrangThai(int id, [FromForm] UploadHoaDon upload)
        {
            HoaDon hoadon = new HoaDon();
            hoadon = await _context.HoaDons.FindAsync(id);
            if (hoadon == null)
            {
                return NotFound();
            }
            hoadon.TrangThai = upload.TrangThai;
            _context.HoaDons.Update(hoadon);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.BroadcastMessage();
            return Ok(Result<object>.Success(hoadon, 1, ""));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ChiTietHoaDonSanPhamBienTheViewModel>>> GetChiTietHoaDonSanPhamBienTheViewModel(int id)
        {
            var kb = from spbt in _context.SanPhamBienThes
                     join sp in _context.SanPhams
                     on spbt.Id_SanPham equals sp.Id
                     join cthd in _context.ChiTietHoaDons
                     on spbt.Id equals cthd.Id_SanPhamBienThe
                     join hd in _context.HoaDons
                     on cthd.Id_HoaDon equals hd.Id
                     join size in _context.Sizes
                     on spbt.SizeId equals size.Id
                     join mau in _context.MauSacs
                     on spbt.Id_Mau equals mau.Id
                     select new ChiTietHoaDonSanPhamBienTheViewModel()
                     {
                         IdCTHD = cthd.Id,
                         TenSanPham = sp.Ten,
                         //HinhAnh = spbt.ImagePath,
                         GiaBan = (decimal)sp.GiaBan,
                         SoLuong = cthd.Soluong,
                         ThanhTien = (decimal)cthd.ThanhTien,
                         Id_HoaDon = (int)cthd.Id_HoaDon,
                         TenMau = mau.MaMau,
                         TenSize = size.TenSize,
                     };
            return await kb.Where(s => s.Id_HoaDon == id).ToListAsync();
        }
        [HttpGet("magiamgia")]
        public async Task<ActionResult<IEnumerable<MaGiamGia>>> MaGiamGia()
        {
            var r= await _context.MaGiamGias.ToListAsync();
            return Ok(Result<object>.Success(r, 1,""));
        }
        [HttpPut("magiamgia")]
        public async Task<ActionResult> SuaMaGiamGia( int id)
        {
            MaGiamGia maGiamGia;
            maGiamGia = await _context.MaGiamGias.FindAsync(id);
            maGiamGia.SoLuong = maGiamGia.SoLuong - 1;
            _context.Update(maGiamGia);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.BroadcastMessage();
            return Ok(Result<object>.Success(maGiamGia, 1, "Cập nhật thành công !!!"));
        }
        [HttpPost]
        public async Task<ActionResult<HoaDon>> TaoHoaDon(HoaDon hd)
        {
            HoaDon hoaDon = new HoaDon()
            {
                MaHoaDon = "HDBH",
                TrangThai = 0,
                GhiChu = hd.GhiChu,
                Id_User = hd.Id_User,
                NgayTao = DateTime.Now,
                Tinh = hd.Tinh,
                Huyen = hd.Huyen,
                Xa = hd.Xa,
                DiaChi = hd.DiaChi,
                TongTien = hd.TongTien,
            };

            hoaDon.IsActive = true;
            hoaDon.IsDelete = false;
            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();
            hoaDon.MaHoaDon = "HDBH" + hoaDon.Id;
            NotificationCheckout notification = new NotificationCheckout()
            {
                ThongBaoMaDonHang = hoaDon.Id,
            };
            _context.NotificationCheckouts.Add(notification);
            var cart = _context.Carts.Where(d => d.UserID == hd.Id_User).ToList();
            List<ChiTietHoaDon> ListCTHD = new List<ChiTietHoaDon>();
            for (int i = 0; i < cart.Count; i++)
            {
                var thisSanPhamBienThe =  _context.SanPhamBienThes.Find(cart[i].Id_SanPhamBienThe);
                ChiTietHoaDon cthd = new ChiTietHoaDon();
                cthd.Id_SanPham = cart[i].SanPhamId;
                cthd.Id_SanPhamBienThe = cart[i].Id_SanPhamBienThe;
                cthd.Id_HoaDon = hoaDon.Id;
                cthd.GiaBan = cart[i].Gia;
                cthd.Soluong = cart[i].SoLuong;
                cthd.ThanhTien = cart[i].Gia * cart[i].SoLuong;
                cthd.Size = cart[i].Size;
                cthd.Mau = cart[i].Mau;
                thisSanPhamBienThe.SoLuongTon = thisSanPhamBienThe.SoLuongTon - cart[i].SoLuong;
                _context.SanPhamBienThes.Update(thisSanPhamBienThe);
                _context.ChiTietHoaDons.Add(cthd);
                _context.Carts.Remove(cart[i]);
                await _context.SaveChangesAsync();
            };
            await _hubContext.Clients.All.BroadcastMessage();
            return Json(1);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoaDons(int id)
        {
            ChiTietHoaDon[] cthd;
            cthd = _context.ChiTietHoaDons.Where(s => s.Id_HoaDon == id).ToArray();
            _context.ChiTietHoaDons.RemoveRange(cthd);
            HoaDon hd;
            hd = await _context.HoaDons.FindAsync(id);
            _context.HoaDons.Remove(hd);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
