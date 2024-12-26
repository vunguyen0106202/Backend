using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using API.Helper.SignalR;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using API.Helper.Result;
using Nancy.Json;

namespace API.Services
{
    public interface IHoaDonsService
    {
        Task<IResult<HoaDon>> TaoHoaDon(HoaDon hd);
    }
    public class HoaDonsService : IHoaDonsService
    {
        private readonly DPContext _context;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly IDataConnector _connector;
        public HoaDonsService(DPContext context, IHubContext<BroadcastHub, IHubClient> hubContext, IDataConnector connector)
        {
            this._context = context;
            this._hubContext = hubContext;
            _connector = connector;
        }
        public async Task<IResult<HoaDon>> TaoHoaDon(HoaDon hd)
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
            hoaDon.LoaiThanhToan = hd.LoaiThanhToan ?? hoaDon.LoaiThanhToan;
            hoaDon.IsPayed = hd.IsPayed ?? false;
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
                //if (hd.SpOrder != null && hd.SpOrder.Contains(cart[i].CartID))
                //{
                //}
                var thisSanPhamBienThe = _context.SanPhamBienThes.Find(cart[i].Id_SanPhamBienThe);
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
            return Result<HoaDon>.Success(hoaDon);
        }
    }
}
