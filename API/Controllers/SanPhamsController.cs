using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using API.Data;
using API.Dtos;
using API.Models;
using API.Helper.SignalR;
using API.Helper;
using API.Helper.Result;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SanPhamsController : Controller
    {
        private readonly DPContext _context;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        public SanPhamsController(DPContext context, IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        [HttpPost("size/{id}")]
        public async Task<ActionResult> Size(int idLoai)
        {
            var resuft = _context.Sizes.Where(d => d.Id_Loai == idLoai).Select(
                d => new TenSizeLoai
                {
                    SizeLoaiTen = d.TenSize
                });
            return Json(await resuft.FirstOrDefaultAsync());
        }
        [HttpPost("mau/{id}")]
        public async Task<ActionResult<IEnumerable<MauSac>>> Mau(int idLoai)
        {
            return await _context.MauSacs.Where(d => d.Id_Loai == idLoai).ToListAsync();
        }
        [HttpPost("like")]
        public async Task<ActionResult> LikeSanPham(UserLike userlike)
        {
            var resuft = await _context.UserLikes.Where(d => d.IdSanPham == userlike.IdSanPham && d.IdUser == userlike.IdUser).FirstOrDefaultAsync();
            if (resuft == null)
            {
                resuft = new UserLike
                {
                    IdSanPham = userlike.IdSanPham,
                    IdUser = userlike.IdUser,
                };
                _context.Add(resuft);
                _context.SaveChanges();
                return Json(1);
            }
            else
            {
                _context.Remove(resuft);
                _context.SaveChanges();
                return Json(2);
            }
        }
        [HttpPost("dslike")]
        public async Task<ActionResult> ListLikeSanPham(UserLike userlike)
        {
            var resuft = _context.UserLikes.Where(d => d.IdUser == userlike.IdUser).Select(
                d => new SanPhamLike
                {
                    id = d.Id,
                    idSanPham = d.IdSanPham,
                    ten = _context.SanPhams.Where(s => s.Id == d.IdSanPham).Select(s => s.Ten).FirstOrDefault(),
                    gia = (decimal)_context.SanPhams.Where(s => s.Id == d.IdSanPham).Select(s => s.GiaBan).FirstOrDefault(),
                    anh=_context.ImageSanPhams.Where(s=>s.IdSanPham==d.IdSanPham).Select(s=>s.ImageName).FirstOrDefault(),
                });
            var r= await resuft.ToListAsync();
            return  Ok(Result<object>.Success(r, 1, "lấy data thành công")) ;
        }
        [HttpPost("deletelike/{id}")]
        public async Task<ActionResult> DeleteLike(int id)
        {
            var card = _context.UserLikes.Where(d => d.Id == id).SingleOrDefault();
            _context.UserLikes.Remove(card);
            await _context.SaveChangesAsync();
            return Json("1");
        }
        [HttpPost("review")]
        public async Task<ActionResult> Review(UserComment usercomment)
        {
            var resuft = new UserComment
            {
                NgayComment = DateTime.Now,
                IdSanPham = usercomment.IdSanPham,
                Content = usercomment.Content,
                IdUser = usercomment.IdUser,
            };
            _context.Add(resuft);
            _context.SaveChanges();
            var listcomment = _context.UserComments.Where(d => d.IdSanPham == usercomment.IdSanPham).Select(
                d => new Review
                {
                    Content = d.Content,
                    tenUser = _context.AppUsers.Where(s => s.Id == d.IdUser).Select(s => s.FirstName + " " + s.LastName).SingleOrDefault(),
                    NgayComment = d.NgayComment
                }
                );
            return Json(await listcomment.ToListAsync());
        }
        [HttpPost("listreview")]
        public async Task<ActionResult> ListReview(UserComment usercomment)
        {
            var listcomment = _context.UserComments.Where(d => d.IdSanPham == usercomment.IdSanPham).Select(
                d => new Review
                {
                    Content = d.Content,
                    tenUser = _context.AppUsers.Where(s => s.Id == d.IdUser).Select(s => s.FirstName + " " + s.LastName).SingleOrDefault(),
                    NgayComment = d.NgayComment
                }
                );
            return Json(await listcomment.ToListAsync());
        }
        [HttpPost("checklike")]
        public async Task<ActionResult> checkLikeSanPham(UserLike userlike)
        {
            var resuft = await _context.UserLikes.Where(d => d.IdSanPham == userlike.IdSanPham && d.IdUser == userlike.IdUser).FirstOrDefaultAsync();
            if (resuft == null)
            {
                return Json(1);
            }
            else
            {
                return Json(2);
            }
        }
        // GET: api/SanPhams
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<SanPhamLoaiThuongHieu>>> GetSanPhams()
        //{
        //    var listIdSanPhamliked = await _context.UserLikes.Select(s => s.IdSanPham).ToListAsync();
        //    var list = await _context.SanPhams.Select(
        //           s => new SanPhamLoaiThuongHieu()
        //           {
        //               Id = s.Id,
        //               Ten = s.Ten,
        //               GiaBan = s.GiaBan,
        //               Tag = s.Tag,
        //               KhuyenMai = s.KhuyenMai,
        //               MoTa = s.MoTa,
        //               HuongDan = s.HuongDan,
        //               GioiTinh = s.GioiTinh,
        //               ThanhPhan = s.ThanhPhan,
        //               IsLike = listIdSanPhamliked.Contains(s.Id),
        //               TrangThaiSanPham = s.TrangThaiSanPham,
        //               TrangThaiHoatDong = s.TrangThaiHoatDong,
        //               Id_Loai = s.Id_Loai,
        //               Id_NhanHieu = s.Id_NhanHieu,
        //               Id_NhaCungCap = s.Id_NhaCungCap,
        //               SoLuongComment = _context.UserComments.Where(x => x.IdSanPham == s.Id).Count(),
        //               SoLuongLike = _context.UserComments.Where(x => x.IdSanPham == s.Id).Count(),
        //               TenLoai = _context.Loais.Where(d => d.Id == s.Id_Loai).Select(d => d.Ten).FirstOrDefault(),
        //               TenNhanHieu = _context.NhanHieus.Where(d => d.Id == s.Id_NhanHieu).Select(d => d.Ten).FirstOrDefault(),
        //               Image = _context.ImageSanPhams.Where(q => q.IdSanPham == s.Id).Select(q => q.ImageName).FirstOrDefault(),
        //           }).Take(50).ToListAsync();
        //    //return Ok(Result<object>.Success(list, list.Count(), ""));  
        //    return list;
        //}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPhamLoaiThuongHieu>>> GetSanPhams(string key, int offset, int limit = 20, string sortField = "", string sortOrder = "asc")
        {
            var listIdSanPhamLiked = await _context.UserLikes.Select(s => s.IdSanPham).ToListAsync();

            var query = _context.SanPhams
                .Where(s => s.IsActive == true && s.IsDelete == false)
                .Select(s => new SanPhamLoaiThuongHieu
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    GiaBan = s.GiaBan,
                    GiaNhap = s.GiaNhap,
                    Tag = s.Tag,
                    KhuyenMai = s.KhuyenMai,
                    MoTa = s.MoTa,
                    HuongDan = s.HuongDan,
                    GioiTinh = s.GioiTinh,
                    ThanhPhan = s.ThanhPhan,
                    IsLike = listIdSanPhamLiked.Contains(s.Id),
                    TrangThaiSanPham = s.TrangThaiSanPham,
                    TrangThaiHoatDong = s.TrangThaiHoatDong,
                    Id_Loai = s.Id_Loai,
                    Id_NhanHieu = s.Id_NhanHieu,
                    Id_NhaCungCap = s.Id_NhaCungCap,
                    SoLuongComment = _context.UserComments.Count(x => x.IdSanPham == s.Id),
                    SoLuongLike = _context.UserLikes.Count(x => x.IdSanPham == s.Id),
                    TenLoai = _context.Loais.Where(d => d.Id == s.Id_Loai).Select(d => d.Ten).FirstOrDefault(),
                    TenNhanHieu = _context.NhanHieus.Where(d => d.Id == s.Id_NhanHieu).Select(d => d.Ten).FirstOrDefault(),
                    ImageRepresent = s.ImageRepresent,
                    Image = _context.ImageSanPhams.Where(q => q.IdSanPham == s.Id).Select(q => q.ImageName).FirstOrDefault(),
                })
                .Where(s => string.IsNullOrEmpty(key) || s.Ten.Contains(key));

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField)
                {
                    case "ten":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.Ten) : query.OrderByDescending(s => s.Ten);
                        break;
                    case "soLuongLike":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.SoLuongLike) : query.OrderByDescending(s => s.SoLuongLike);
                        break;
                    case "soLuongComment":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.SoLuongComment) : query.OrderByDescending(s => s.SoLuongComment);
                        break;
                    case "giaBan":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.GiaBan) : query.OrderByDescending(s => s.GiaBan);
                        break;
                    case "giaNhap":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.GiaNhap) : query.OrderByDescending(s => s.GiaNhap);
                        break;
                    case "trangThaiSanPham":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.TrangThaiSanPham) : query.OrderByDescending(s => s.TrangThaiSanPham);
                        break;
                    case "trangThaiHoatDong":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.TrangThaiHoatDong) : query.OrderByDescending(s => s.TrangThaiHoatDong);
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

            var totalCount = await _context.SanPhams
                .Where(s => s.IsActive == true && s.IsDelete == false)
                .Where(s => string.IsNullOrEmpty(key) || s.Ten.Contains(key))
                .CountAsync();

            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<SanPhamLoaiThuongHieu>>.Success(results, totalCount, ""));
        }

        [HttpGet("GetAutoCompleteProduct")]
        public async Task<ActionResult> GetAutoCompleteProduct(string key, int offset, int limit = 20)
        {
            var query = _context.SanPhams
                .Where(s => s.IsActive == true && s.IsDelete == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Id_Loai = s.Id_Loai,
                    Ten = s.Ten,
                    Image = _context.ImageSanPhams.Where(q => q.IdSanPham == s.Id).Select(q => q.ImageName).FirstOrDefault(),

                })
                .Where(s => string.IsNullOrEmpty(key) || s.Ten.Contains(key));

            query = query.OrderBy(s => s.Ten).Skip(offset).Take(limit);

            var totalCount = await _context.SanPhams
                .Where(s => s.IsActive == true && s.IsDelete == false)
                .Where(s => string.IsNullOrEmpty(key) || s.Ten.Contains(key))
                .CountAsync();

            var results = await query.ToListAsync();

            return Ok(Result<object>.Success(results, totalCount, ""));
        }


        [HttpGet("GetProductById/{id}")]
        public async Task<ActionResult<IEnumerable<SanPhamLoaiThuongHieu>>> GetProductById(int id)
        {
            var listIdSanPhamLiked = await _context.UserLikes.Select(s => s.IdSanPham).ToListAsync();

            var query = _context.SanPhams
                .Where(s => s.IsActive == true && s.IsDelete == false && s.Id == id)
                .Select(s => new SanPhamLoaiThuongHieu
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    GiaBan = s.GiaBan,
                    GiaNhap = s.GiaNhap,
                    Tag = s.Tag,
                    KhuyenMai = s.KhuyenMai,
                    MoTa = s.MoTa,
                    HuongDan = s.HuongDan,
                    GioiTinh = s.GioiTinh,
                    ThanhPhan = s.ThanhPhan,
                    IsLike = listIdSanPhamLiked.Contains(s.Id),
                    TrangThaiSanPham = s.TrangThaiSanPham,
                    TrangThaiHoatDong = s.TrangThaiHoatDong,
                    Id_Loai = s.Id_Loai,
                    Id_NhanHieu = s.Id_NhanHieu,
                    Id_NhaCungCap = s.Id_NhaCungCap,
                    SoLuongComment = _context.UserComments.Count(x => x.IdSanPham == s.Id),
                    SoLuongLike = _context.UserLikes.Count(x => x.IdSanPham == s.Id),
                    TenLoai = _context.Loais.Where(d => d.Id == s.Id_Loai).Select(d => d.Ten).FirstOrDefault(),
                    TenNhanHieu = _context.NhanHieus.Where(d => d.Id == s.Id_NhanHieu).Select(d => d.Ten).FirstOrDefault(),
                    ImageRepresent = s.ImageRepresent,
                    Image = _context.ImageSanPhams.Where(q => q.IdSanPham == s.Id).Select(q => q.ImageName).FirstOrDefault(),
                });
            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<SanPhamLoaiThuongHieu>>.Success(results));
        }

        // GET: api/SanPhams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPham>> GetSanPham(int id)
        {
            var sanPham = await _context.SanPhams.Include(sp => sp.ImageSanPhams).FirstOrDefaultAsync(sp => sp.Id == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return Ok(Result<object>.Success(sanPham));
        }
        [HttpPost("UpdateState")]
        public async Task<ActionResult> UpdateState(int id)
        { 
            var sanpham = await _context.SanPhams.FirstOrDefaultAsync(s => s.Id == id);
            sanpham.TrangThaiHoatDong = !sanpham.TrangThaiHoatDong;
            await _context.SaveChangesAsync();
            //await _hubContext.Clients.All.BroadcastMessage();
            return Ok(Result<object>.Success(sanpham));
        }
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutSanPham(int id, [FromForm] UploadSanpham upload)
        //{
        //    var listImage = new List<ImageSanPham>();
        //    SanPham sanpham = new SanPham();
        //    sanpham = await _context.SanPhams.FirstOrDefaultAsync(s => s.Id == id);
        //    sanpham.Ten = upload.Ten;
        //    sanpham.NgayCapNhat = DateTime.Now;
        //    sanpham.HuongDan = upload.HuongDan;
        //    sanpham.MoTa = upload.MoTa;
        //    sanpham.GiaBan = upload.GiaBan;
        //    sanpham.Tag = upload.Tag;
        //    sanpham.GioiTinh = upload.GioiTinh;
        //    sanpham.GiaNhap = upload.GiaNhap;
        //    sanpham.KhuyenMai = upload.KhuyenMai;
        //    sanpham.ThanhPhan = upload.ThanhPhan;
        //    sanpham.TrangThaiHoatDong = upload.TrangThaiHoatDong;
        //    sanpham.TrangThaiSanPham = upload.TrangThaiSanPham;
        //    if (upload.Id_NhanHieu == null)
        //    {
        //        sanpham.Id_NhanHieu = sanpham.Id_NhanHieu;
        //    }
        //    else
        //    {
        //        sanpham.Id_NhanHieu = upload.Id_NhanHieu;
        //    }
        //    if (upload.Id_Loai == null)
        //    {
        //        sanpham.Id_Loai = sanpham.Id_Loai;
        //    }
        //    else
        //    {
        //        sanpham.Id_Loai = upload.Id_Loai;
        //    }
        //    if (upload.Id_NhaCungCap == null)
        //    {
        //        sanpham.Id_NhaCungCap = sanpham.Id_NhaCungCap;
        //    }
        //    Notification notification = new Notification()
        //    {
        //        TenSanPham = upload.Ten,
        //        TranType = "Edit"
        //    };
        //    _context.Notifications.Add(notification);
        //    ImageSanPham[] images = _context.ImageSanPhams.Where(s => s.IdSanPham == id).ToArray();
        //    _context.ImageSanPhams.RemoveRange(images);
        //    ImageSanPham image = new ImageSanPham();
        //    var file = upload.files.ToArray();
        //    var imageSanPhams = _context.ImageSanPhams.ToArray().Where(s => s.IdSanPham == id);
        //    foreach (var i in imageSanPhams)
        //    {
        //        FileHelper.DeleteFileOnTypeAndNameAsync("product", i.ImageName);
        //    }
        //    if (upload.files != null)
        //    {
        //        for (int i = 0; i < file.Length; i++)
        //        {
        //            if (file[i].Length > 0 && file[i].Length < 5120)
        //            {
        //                listImage.Add(new ImageSanPham()
        //                {
        //                    ImageName = await FileHelper.UploadImageAndReturnFileNameAsync(upload, null, "product", (IFormFile[])upload.files, i),
        //                    IdSanPham = sanpham.Id,
        //                });
        //            }
        //        }
        //    }
        //    else // xu li khi khong cap nhat hinh
        //    {
        //        List<ImageSanPham> List;
        //        List = _context.ImageSanPhams.Where(s => s.IdSanPham == id).ToList();
        //        foreach (ImageSanPham img in List)
        //            listImage.Add(new ImageSanPham()
        //            {
        //                ImageName = img.ImageName,
        //                IdSanPham = sanpham.Id,
        //            }); ;
        //    };
        //    sanpham.ImageSanPhams = listImage;
        //    _context.SanPhams.Update(sanpham);
        //    await _context.SaveChangesAsync();
        //    await _hubContext.Clients.All.BroadcastMessage();
        //    return Ok();
        //}
        // POST: api/SanPhams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SanPham>> PostSanPham([FromForm] UploadSanpham upload)
        {
            SanPham sanpham;
            if (upload.Id > 0)
            {
                sanpham = await _context.SanPhams.FindAsync(upload.Id);
                if (sanpham == null)
                {
                    return NotFound(Result<string>.Success("Sản phẩm không tồn tại."));
                }
                sanpham.Ten = upload.Ten;
                sanpham.HuongDan = upload.HuongDan;
                sanpham.MoTa = upload.MoTa;
                sanpham.ThanhPhan = upload.ThanhPhan;
                sanpham.TrangThaiHoatDong = upload.TrangThaiHoatDong;
                sanpham.TrangThaiSanPham = upload.TrangThaiSanPham;
                sanpham.GiaBan = upload.GiaBan;
                sanpham.GioiTinh = upload.GioiTinh;
                sanpham.GiaNhap = upload.GiaNhap;
                sanpham.Tag = upload.Tag;
                sanpham.KhuyenMai = upload.KhuyenMai;
                sanpham.Id_Loai = upload.Id_Loai;
                sanpham.Id_NhanHieu = upload.Id_NhanHieu;
                sanpham.Id_NhaCungCap = upload.Id_NhaCungCap;
                sanpham.UpdatedDate = DateTime.Now;
                sanpham.ImageRepresent = upload.ImageRepresent;

                // Xử lý xóa ảnh
                if (upload.DeletedImages != null && upload.DeletedImages.Count > 0)
                {
                    var imagesToDelete = _context.ImageSanPhams
                        .Where(img => img.IdSanPham == sanpham.Id && upload.DeletedImages.Contains(img.ImageName))
                        .ToList();
                    _context.ImageSanPhams.RemoveRange(imagesToDelete);
                    foreach (var img in imagesToDelete)
                    {
                        FileHelper.DeleteFileOnTypeAndNameAsync("product", img.ImageName);
                    }
                }

                _context.SanPhams.Update(sanpham);
            }
            else
            {
                sanpham = new SanPham()
                {
                    Ten = upload.Ten,
                    NgayTao = DateTime.Now,
                    HuongDan = upload.HuongDan,
                    MoTa = upload.MoTa,
                    ThanhPhan = upload.ThanhPhan,
                    TrangThaiHoatDong = upload.TrangThaiHoatDong,
                    TrangThaiSanPham = upload.TrangThaiSanPham,
                    GiaBan = upload.GiaBan,
                    GioiTinh = upload.GioiTinh,
                    GiaNhap = upload.GiaNhap,
                    Tag = upload.Tag,
                    KhuyenMai = upload.KhuyenMai,
                    Id_Loai = upload.Id_Loai,
                    Id_NhanHieu = upload.Id_NhanHieu,
                    Id_NhaCungCap = upload.Id_NhaCungCap,
                    ImageRepresent = upload.ImageRepresent,
                    IsActive = true,
                    IsDelete = false,
                    CreatedDate = DateTime.Now,
                };

                _context.SanPhams.Add(sanpham);
               // await _context.SaveChangesAsync();
            }

            Notification notification = new Notification()
            {
                TenSanPham = upload.Ten,
                TranType = upload.Id != 0 ? "Edit" : "Add"
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            if (upload.files != null)
            {
                var file = upload.files.ToArray();
                for (int i = 0; i < file.Length; i++)
                {
                    if (file[i].Length > 0 && file[i].Length < 2 * 1024 * 1024)
                    {
                        // Kiểm tra xem ảnh đã tồn tại chưa
                        var existingImage = await _context.ImageSanPhams
                            .FirstOrDefaultAsync(img => img.ImageName == file[i].FileName && img.IdSanPham == sanpham.Id);

                        if (existingImage == null)
                        {
                            var imageSanPham = new ImageSanPham();
                            imageSanPham.ImageName = await FileHelper.UploadImageAndReturnFileNameAsync3(upload, null, "product", upload.files.ToArray(), i);
                            imageSanPham.IdSanPham = sanpham.Id;
                            _context.ImageSanPhams.Add(imageSanPham);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            await _hubContext.Clients.All.BroadcastMessage();
            return Ok(Result<SanPham>.Success(sanpham));
        }
        //public async Task<ActionResult<SanPham>> PostSanPham([FromForm] UploadSanpham upload)
        //{
        //    var listImage = new List<ImageSanPham>();
        //    SanPham sanpham = new SanPham()
        //    {
        //        Ten = upload.Ten,
        //        NgayTao = DateTime.Now,
        //        HuongDan = upload.HuongDan,
        //        MoTa = upload.MoTa,
        //        ThanhPhan = upload.ThanhPhan,
        //        TrangThaiHoatDong = upload.TrangThaiHoatDong,
        //        TrangThaiSanPham = upload.TrangThaiSanPham,
        //        GiaBan = upload.GiaBan,
        //        GioiTinh = upload.GioiTinh,
        //        GiaNhap = upload.GiaNhap,
        //        Tag = upload.Tag,
        //        KhuyenMai = upload.KhuyenMai,
        //        Id_Loai = upload.Id_Loai,
        //        Id_NhanHieu = upload.Id_NhanHieu,
        //        Id_NhaCungCap = upload.Id_NhaCungCap,
        //        IsActive = true,
        //        IsDelete = false,
        //        CreatedDate = DateTime.Now,
        //    };

        //    Notification notification = new Notification()
        //    {
        //        TenSanPham = upload.Ten,
        //        TranType = "Add"
        //    };
        //    _context.Notifications.Add(notification);
        //    _context.SanPhams.Add(sanpham);
        //    await _context.SaveChangesAsync();
        //    var file = upload.files.ToArray();
        //    if (upload.files != null)
        //    {
        //        for (int i = 0; i < file.Length; i++)
        //        {
        //            if (file[i].Length > 0 && file[i].Length < 5120)
        //            {
        //                var imageSanPham = new ImageSanPham();
        //                imageSanPham.ImageName = await FileHelper.UploadImageAndReturnFileNameAsync(upload, null, "product", (IFormFile[])upload.files, i);
        //                imageSanPham.IdSanPham = sanpham.Id;
        //                _context.ImageSanPhams.Update(imageSanPham);
        //                await _context.SaveChangesAsync();
        //            }
        //        }
        //    }
        //    await _hubContext.Clients.All.BroadcastMessage();
        //    return Ok(Result<SanPham>.Success(sanpham));
        //    //return Ok(Result<IEnumerable<SanPhamLoaiThuongHieu>>.Success(results, totalCount, ""));
        //}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSanPham(int id)
        {
            //var imageSanPhams = _context.ImageSanPhams.ToArray().Where(s => s.IdSanPham == id);
            //foreach (var i in imageSanPhams)
            //{
            //    FileHelper.DeleteFileOnTypeAndNameAsync("product", i.ImageName);
            //}
            //Models.SanPhamBienThe[] spbts;
            //spbts = _context.SanPhamBienThes.Where(s => s.Id_SanPham == id).ToArray();
            //_context.SanPhamBienThes.RemoveRange(spbts);
            //ImageSanPham[] images;
            //images = _context.ImageSanPhams.Where(s => s.IdSanPham == id).ToArray();
            //_context.ImageSanPhams.RemoveRange(images);
            //await _context.SaveChangesAsync();
            //var sanPham = await _context.SanPhams.FindAsync(id);
            //if (sanPham == null)
            //{
            //    return NotFound();
            //}
            //var CategoryConstraint = _context.Loais.Where(s => s.Id == id);
            //var BrandConstraint = _context.NhanHieus.SingleOrDefaultAsync(s => s.Id == id);
            //if (CategoryConstraint != null)
            //{
            //    _context.SanPhams.Remove(sanPham);
            //}
            //if (BrandConstraint != null)
            //{
            //    _context.SanPhams.Remove(sanPham);
            //}

            var sanPham = await _context.SanPhams.FindAsync(id);
            sanPham.IsActive = false;
            sanPham.IsDelete = true;
            sanPham.DeletedDate = DateTime.Now;

            Notification notification = new Notification()
            {
                TenSanPham = sanPham.Ten,
                TranType = "Delete"
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.BroadcastMessage();
            return Ok(Result<SanPham>.Success(sanPham));
        }
        [HttpGet("loai/{id}")]
        public async Task<ActionResult<IEnumerable<SanPham>>> GetCategory(int id)
        {
            return await _context.SanPhams.Where(s => s.Id_Loai == id || s.Id_NhanHieu == id).ToListAsync();
        }
        [HttpGet("nhanhieu/{id}")]
        public async Task<ActionResult<IEnumerable<SanPham>>> GetBrand(int id)
        {
            return await _context.SanPhams.Where(s => s.Id_NhanHieu == id).ToListAsync();
        }
        [HttpGet("loainhanhieu/{id}")]
        public async Task<ActionResult<IEnumerable<SanPham>>> GetBrandCate(int id)
        {
            var get = _context.SanPhams.Where(s => s.Id_Loai == id);
            if (get != null)
            {
                return await _context.SanPhams.Where(s => s.Id_Loai == id).ToListAsync();
            }
            else
            {
                return await _context.SanPhams.Where(s => s.Id_NhanHieu == id).ToListAsync();
            }
        }
        [HttpGet("chitietsanpham/{id}")]
        public async Task<ActionResult<ProductDetail>> Chitiet(int id)
        {
            ProductDetail pr;
            List<ImageSanPham> listImage;
            listImage = await _context.ImageSanPhams.Where(s => s.IdSanPham == id).ToListAsync();
            List<SanPhamBienTheMauSize> listSPBT;
            var temp = from s in _context.SanPhamBienThes
                       join z in _context.Sizes
                       on s.SizeId equals z.Id
                       join m in _context.MauSacs
                       on s.Id_Mau equals m.Id
                       select new SanPhamBienTheMauSize()
                       {
                           Id = s.Id,
                           SoLuongTon = s.SoLuongTon,
                           TenMau = m.MaMau,
                           TenSize = z.TenSize,
                           Id_SanPham = s.Id_SanPham,
                       };
            listSPBT = await temp.Where(s => s.Id_SanPham == id).ToListAsync();
            var kb = from s in _context.SanPhams
                     join spbt in _context.SanPhamBienThes
                     on s.Id equals spbt.Id_SanPham
                     join hinh in _context.ImageSanPhams
                     on s.Id equals hinh.IdSanPham
                     join th in _context.NhanHieus
                     on s.Id_NhanHieu equals th.Id
                     join l in _context.Loais
                     on s.Id_Loai equals l.Id
                     join ncc in _context.NhaCungCaps
                     on s.Id_NhaCungCap equals ncc.Id
                     select new ProductDetail()
                     {
                         Id = s.Id,
                         Ten = s.Ten,
                         GiaBan = s.GiaBan,
                         Tag = s.Tag,
                         KhuyenMai = s.KhuyenMai,
                         MoTa = s.MoTa,
                         GioiTinh = s.GioiTinh,
                         HuongDan = s.HuongDan,
                         TenNhaCungCap = ncc.Ten,
                         ThanhPhan = s.ThanhPhan,
                         TrangThaiSanPham = s.TrangThaiSanPham,
                         TrangThaiHoatDong = s.TrangThaiHoatDong,
                         Id_Loai = s.Id_Loai,
                         Id_NhanHieu = s.Id_NhanHieu,
                         TenLoai = l.Ten,
                         TenNhanHieu = th.Ten,
                         ImageSanPhams = listImage,
                         SanPhamBienThes = listSPBT,
                     };
            pr = kb.FirstOrDefault(s => s.Id == id);
            return pr;
        }
        [HttpGet("topsanphammoi")]
        public async Task<ActionResult<IEnumerable<SanPhamLoaiThuongHieu>>> DanhSachHangMoi()
        {
            var kb = _context.SanPhams
                .Where(s=>s.IsActive==true && s.IsDelete==false)
                .Select(
                   s => new SanPhamLoaiThuongHieu()
                   {
                       Id = s.Id,
                       Ten = s.Ten,
                       GiaBan = s.GiaBan,
                       Tag = s.Tag,
                       KhuyenMai = s.KhuyenMai,
                       MoTa = s.MoTa,
                       HuongDan = s.HuongDan,
                       GioiTinh = s.GioiTinh,
                       ThanhPhan = s.ThanhPhan,
                       TrangThaiSanPham = s.TrangThaiSanPham,
                       TrangThaiHoatDong = s.TrangThaiHoatDong,
                       Id_Loai = s.Id_Loai,
                       Id_NhanHieu = s.Id_NhanHieu,
                       TenLoai = _context.Loais.Where(d => d.Id == s.Id_Loai).Select(d => d.Ten).FirstOrDefault(),
                       TenNhanHieu = _context.NhanHieus.Where(d => d.Id == s.Id_NhanHieu).Select(d => d.Ten).FirstOrDefault(),
                       Image = _context.ImageSanPhams.Where(q => q.IdSanPham == s.Id).Select(q => q.ImageName).FirstOrDefault(),
                   }).Take(20).Where(s => s.TrangThaiSanPham == "new" && s.TrangThaiHoatDong == true);

            var results = await kb.ToListAsync();

            return Ok(Result<object>.Success(results, 1, ""));
        }
        [HttpPost("sapxepsanpham")]
        public async Task<ActionResult> SapXepSP(SapXep sx)
        {
            var kb = _context.SanPhams.Where(d => d.IsActive==true&& d.IsDelete==false&&d.GiaBan > sx.Thap && d.GiaBan < sx.Cao).Select(
                   s => new SanPhamLoaiThuongHieu()
                   {
                       Id = s.Id,
                       Ten = s.Ten,
                       GiaBan = s.GiaBan,
                       Tag = s.Tag,
                       KhuyenMai = s.KhuyenMai,
                       MoTa = s.MoTa,
                       HuongDan = s.HuongDan,
                       GioiTinh = s.GioiTinh,
                       ThanhPhan = s.ThanhPhan,
                       TrangThaiSanPham = s.TrangThaiSanPham,
                       TrangThaiHoatDong = s.TrangThaiHoatDong,
                       Id_Loai = s.Id_Loai,
                       Id_NhanHieu = s.Id_NhanHieu,
                       TenLoai = _context.Loais.Where(d => d.Id == s.Id_Loai).Select(d => d.Ten).FirstOrDefault(),
                       TenNhanHieu = _context.NhanHieus.Where(d => d.Id == s.Id_NhanHieu).Select(d => d.Ten).FirstOrDefault(),
                       Image = _context.ImageSanPhams.Where(q => q.IdSanPham == s.Id).Select(q => q.ImageName).FirstOrDefault(),
                   }).Take(20);
            var r = await kb.ToListAsync();
            return Ok(Result<IEnumerable<SanPhamLoaiThuongHieu>>.Success(r));
        }
        [HttpGet("searchtheoloai")]
        public async Task<ActionResult<IEnumerable<SanPhamLoaiThuongHieu>>> searchtheoloai(int i)
        {
            var kb = _context.SanPhams.Where(d => d.IsActive == true && d.IsDelete == false && d.Id_Loai==i).Select(
                   s => new SanPhamLoaiThuongHieu()
                   {
                       Id = s.Id,
                       Ten = s.Ten,
                       GiaBan = s.GiaBan,
                       Tag = s.Tag,
                       KhuyenMai = s.KhuyenMai,
                       MoTa = s.MoTa,
                       HuongDan = s.HuongDan,
                       GioiTinh = s.GioiTinh,
                       ThanhPhan = s.ThanhPhan,
                       TrangThaiSanPham = s.TrangThaiSanPham,
                       TrangThaiHoatDong = s.TrangThaiHoatDong,
                       Id_Loai = s.Id_Loai,
                       Id_NhanHieu = s.Id_NhanHieu,
                       TenLoai = _context.Loais.Where(d => d.Id == s.Id_Loai).Select(d => d.Ten).FirstOrDefault(),
                       TenNhanHieu = _context.NhanHieus.Where(d => d.Id == s.Id_NhanHieu).Select(d => d.Ten).FirstOrDefault(),
                       Image = _context.ImageSanPhams.Where(q => q.IdSanPham == s.Id).Select(q => q.ImageName).FirstOrDefault(),
                   }).Take(20);
            var r= await kb.ToListAsync();
            return Ok(Result<IEnumerable<SanPhamLoaiThuongHieu>>.Success(r, 1, ""));
        }
        [HttpPost("searchtheomau")]
        public async Task<IActionResult> getListTaskCalendar([FromBody] JObject json)
        {
            var mau = json.GetValue("mausac").ToString();
            var list_id_mau = _context.MauSacs.Where(d => d.MaMau == mau).Select(d => d.Id.ToString()).ToList();
            var list_spbienthe_theomau = _context.SanPhamBienThes.Where(d => list_id_mau.Contains((d.Id_Mau.ToString()))).Select(d => d.Id_SanPham).Distinct().ToList();
            var kb = _context.SanPhams.Where(d => list_spbienthe_theomau.Contains(d.Id)).Select(
                   s => new SanPhamLoaiThuongHieu()
                   {
                       Id = s.Id,
                       Ten = s.Ten,
                       GiaBan = s.GiaBan,
                       Tag = s.Tag,
                       KhuyenMai = s.KhuyenMai,
                       MoTa = s.MoTa,
                       HuongDan = s.HuongDan,
                       GioiTinh = s.GioiTinh,
                       ThanhPhan = s.ThanhPhan,
                       TrangThaiSanPham = s.TrangThaiSanPham,
                       TrangThaiHoatDong = s.TrangThaiHoatDong,
                       Id_Loai = s.Id_Loai,
                       Id_NhanHieu = s.Id_NhanHieu,
                       TenLoai = _context.Loais.Where(d => d.Id == s.Id_Loai).Select(d => d.Ten).FirstOrDefault(),
                       TenNhanHieu = _context.NhanHieus.Where(d => d.Id == s.Id_NhanHieu).Select(d => d.Ten).FirstOrDefault(),
                       Image = _context.ImageSanPhams.Where(q => q.IdSanPham == s.Id).Select(q => q.ImageName).FirstOrDefault(),
                   }).Take(20);
            var r= await kb.ToListAsync();
            return Ok(Result<IEnumerable<SanPhamLoaiThuongHieu>>.Success(r));
        }
    }
}