using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Models;
using API.Helper.SignalR;
using API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using API.Helper.Result;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        // Vũ commit
        //API 
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly DPContext _context;
        public BannersController(DPContext context, IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            this._context = context;
            this._hubContext = hubContext;
        }
        [HttpGet]
        public async Task<ActionResult> GetllBanners(string key, int idBanner = -1, int offset = 0, int limit = 20, string sortField = "", string sortOrder = "asc")
        {

            var query = _context.Banners.AsQueryable();

            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(s => s.Name.Contains(key) || s.Description.Contains(key));
            }


            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField)
                {
                    case "name":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name);
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

            if (idBanner > 0)
            {
                query = query.Where(q => q.Id == idBanner).Skip(offset).Take(limit);
            }
            else
            {
                query = query.Skip(offset).Take(limit);
            }


            var banners = await query.ToListAsync();

            return Ok(Result<object>.Success(banners, banners.Count(), "lấy data thành công"));
        }
        [HttpGet("get")]
        public async Task<ActionResult>getBannerUser(int type)
        {
            var currentTime = DateTime.Now;
            
            var query = _context.Banners
                .Where(s => s.BannerType == type && currentTime >= s.StartDate
                    && currentTime <= s.EndDate);
            var r=await query.ToListAsync();

            return Ok(Result<object>.Success(r, 1, "lấy data thành công"));
        }

        [HttpPost]
        public async Task<ActionResult<Banner>> PutBanner([FromForm] UploadBanner uploadBanner)
        {
            Banner banner;
            if (uploadBanner.Id > 0)
            {
                banner = await _context.Banners.FindAsync(uploadBanner.Id);
                if (banner == null)
                {
                    return NotFound(Result<string>.Success("không tồn tại."));
                }
                banner.Name = uploadBanner.Name;
                banner.Description = uploadBanner.Description;
                banner.BannerType = uploadBanner.BannerType;
                banner.Link = uploadBanner.Link;
                banner.StartDate = uploadBanner.StartDate;
                banner.EndDate = uploadBanner.EndDate;  
                banner.UpdatedDate = DateTime.Now;
                if (uploadBanner.files!= null)
                {
                    FileHelper.DeleteFileOnTypeAndNameAsync1("banner", banner.Image);
                }
             
            }
            else
            {
                banner = new Banner
                {
                    Name = uploadBanner.Name,
                    Description = uploadBanner.Description,
                    BannerType = uploadBanner.BannerType,
                    Link=uploadBanner.Link,
                    StartDate = uploadBanner.StartDate, 
                    EndDate = uploadBanner.EndDate,
                    CreatedDate = DateTime.Now
                };
                _context.Banners.Add(banner);
            }
           
            if (uploadBanner.files != null)
            {
                var file = uploadBanner.files;
                
                    if (file.Length > 0 && file.Length < 2 * 1024 * 1024)
                    {
                        // Kiểm tra xem ảnh đã tồn tại chưa
                        banner.Image = await FileHelper.UploadImageAndReturnFileNameAsync4(uploadBanner, "banner", uploadBanner.files);
                       
                    }
                
            }
            await _context.SaveChangesAsync();
            return Ok(Result<object>.Success(banner, 1, "Thêm/Cập nhật thành công !!!"));
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Banner>> GetDetail(int id)
        {
            var banner = await _context.Banners.FirstOrDefaultAsync(sp => sp.Id == id);

            if (banner == null)
            {
                return NotFound();
            }

            return Ok(Result<object>.Success(banner, 1, ""));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var banner = await _context.Banners.FindAsync(id);

            if (banner == null)
            {
                return NotFound(Result<string>.Success("Banner không tồn tại."));
            }

            // Xóa ảnh nếu có trong database và thư mục wwwroot
            if (!string.IsNullOrEmpty(banner.Image))
            {
                FileHelper.DeleteFileOnTypeAndNameAsync1("banner", banner.Image);
            }

            _context.Banners.Remove(banner); // Xóa banner khỏi database
            await _context.SaveChangesAsync(); // Lưu thay đổi

            return Ok(Result<object>.Success(banner));
        }
    }
}
