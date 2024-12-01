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
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace API.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : Controller
    {
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly DPContext _context;
        public BlogsController(DPContext context, IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            this._context = context;
            this._hubContext = hubContext;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogAndImage>>> GetllBlogs(string key, int offset, int limit = 20, string sortField = "", string sortOrder = "asc")
        {
            var query = _context.Blogs
                 .Where(s => s.IsActive == true && s.IsDelete == false)
                 .Select(s => new BlogAndImage
                 {
                     Id = s.Id,
                     TieuDe = s.TieuDe,
                     NoiDung = s.NoiDung,
                     image = _context.ImageBlogs.Where(q => q.FkBlogId == s.Id).Select(q => q.ImageName).FirstOrDefault(),
                 })
                 .Where(s => string.IsNullOrEmpty(key) || s.TieuDe.Contains(key));

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField)
                {
                    case "tieuDe":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.TieuDe) : query.OrderByDescending(s => s.TieuDe);
                        break;
                    default:
                        query = query.OrderByDescending(s => s.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(s => s.Id);
            }

            query = query.Skip(offset).Take(limit);

            var totalCount = await _context.Blogs
                .Where(s => s.IsActive == true && s.IsDelete == false)
                .Where(s => string.IsNullOrEmpty(key) || s.TieuDe.Contains(key))
                .CountAsync();

            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<object>>.Success(results, totalCount, ""));
        }
        [HttpPost("getBlog")]
        public async Task<ActionResult> GetBlog(string key, int offset, int limit = 20, string sortField = "", string sortOrder = "asc")
        {
              var query = _context.Blogs
                .Where(s => s.IsActive == true && s.IsDelete == false)
                .Select(s => new
                {
                    Id = s.Id,
                    TieuDe = s.TieuDe,
                    NoiDung = s.NoiDung,
                    Image = _context.ImageBlogs.Where(q => q.Id == s.Id).Select(q => q.ImageName).FirstOrDefault(),
                })
                .Where(s => string.IsNullOrEmpty(key) || s.TieuDe.Contains(key));

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField)
                {
                    case "tieuDe":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.TieuDe) : query.OrderByDescending(s => s.TieuDe);
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

            var totalCount = await _context.Blogs
                .Where(s => s.IsActive == true && s.IsDelete == false)
                .Where(s => string.IsNullOrEmpty(key) || s.TieuDe.Contains(key))
                .CountAsync();

            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<object>>.Success(results, totalCount, ""));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlog(int id, [FromForm] UploadBlog upload)
        {
            var listImage = new List<ImageBlog>();
            Blog blog = new Blog();
            blog = await _context.Blogs.FindAsync(id);
            blog.TieuDe = upload.TieuDe;
            blog.NoiDung = upload.NoiDung;
            blog.FkAppUser_NguoiThem = upload.FkUserId;
            Notification notification = new Notification()
            {
                TenSanPham = upload.TieuDe,
                TranType = "Edit"
            };
            _context.Notifications.Add(notification);
            ImageBlog[] images = _context.ImageBlogs.Where(s => s.FkBlogId == id).ToArray();
            _context.ImageBlogs.RemoveRange(images);
            ImageBlog image = new ImageBlog();
            var file = upload.files.ToArray();
            var imageBlogs = _context.ImageBlogs.ToArray().Where(s => s.FkBlogId == id);
            foreach (var i in imageBlogs)
            {
                FileHelper.DeleteFileOnTypeAndNameAsync("blog", i.ImageName);
            }
            if (upload.files != null)
            {
                for (int i = 0; i < file.Length; i++)
                {
                    if (file[i].Length > 0 && file[i].Length < 5120)
                    {
                        listImage.Add(new ImageBlog()
                        {
                            ImageName = await FileHelper.UploadImageAndReturnFileNameAsync(null, upload, "blog", (IFormFile[])upload.files, i),
                            FkBlogId = blog.Id,
                        });
                    }
                }
            }
            else // xu li khi khong cap nhat hinh
            {
                List<ImageBlog> List;
                List = _context.ImageBlogs.Where(s => s.FkBlogId == id).ToList();
                foreach (ImageBlog img in List)
                    listImage.Add(new ImageBlog()
                    {
                        ImageName = img.ImageName,
                        FkBlogId = blog.Id,
                    }); ;
            };
            blog.ImageBlogs = listImage;
            //_context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.BroadcastMessage();
            return Ok();
        }
        // POST: api/Blogs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Blog>> PostBlog([FromForm] UploadBlog upload)
        {
            
            Blog blog;
            if (upload.Id > 0)
            {
                blog = await _context.Blogs.FindAsync(upload.Id);
                if (blog == null)
                {
                    return NotFound(Result<string>.Success("không tồn tại."));
                }
                blog.TieuDe = upload.TieuDe;
                blog.NoiDung = upload.NoiDung;
                blog.UpdatedDate = DateTime.Now;

                // Xử lý xóa ảnh
                if (upload.DeletedImages != null && upload.DeletedImages.Count > 0)
                {
                    var imagesToDelete = _context.ImageBlogs
                        .Where(img => img.FkBlogId == blog.Id && upload.DeletedImages.Contains(img.ImageName))
                        .ToList();
                    _context.ImageBlogs.RemoveRange(imagesToDelete);
                    foreach (var img in imagesToDelete)
                    {
                        FileHelper.DeleteFileOnTypeAndNameAsync("", img.ImageName);
                    }
                }

                _context.Blogs.Update(blog);
            }
            else
            {
                blog = new Blog()
                {
                    TieuDe = upload.TieuDe,
                    NoiDung = upload.NoiDung,
                    IsActive = true,
                    IsDelete = false,
                    CreatedDate = DateTime.Now,
                };

                _context.Blogs.Add(blog);
            }

            Notification notification = new Notification()
            {
                TenSanPham = upload.TieuDe,
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
                        var existingImage = await _context.ImageBlogs
                            .FirstOrDefaultAsync(img => img.ImageName == file[i].FileName && img.Id == blog.Id);

                        if (existingImage == null)
                        {
                            var image = new ImageBlog();
                            image.ImageName = await FileHelper.UploadImageAndReturnFileNameAsync3(null, upload, "blog", upload.files.ToArray(), i);
                            image.FkBlogId = blog.Id;
                           _context.ImageBlogs.Add(image);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.BroadcastMessage();
            return Ok(Result<Blog>.Success(blog));

        }
        // DELETE: api/Blogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            //var imageBlogs = _context.ImageBlogs.ToArray().Where(s => s.FkBlogId == id);
            //foreach (var i in imageBlogs)
            //{
            //    FileHelper.DeleteFileOnTypeAndNameAsync("blog", i.ImageName);
            //}
            //_context.ImageBlogs.RemoveRange(imageBlogs);
            //var blog = await _context.Blogs.FindAsync(id);
            //_context.Blogs.Remove(blog);
            //await _context.SaveChangesAsync();
            //return Ok();
            var blog = await _context.Blogs.FindAsync(id);
            blog.IsActive = false;
            blog.IsDelete = true;
            blog.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(Result<object>.Success(blog));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetDetail(int id)
        {
            var blog = await _context.Blogs.Include(sp => sp.ImageBlogs).FirstOrDefaultAsync(sp => sp.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            return Ok(Result<object>.Success(blog));
        }

        [HttpGet("GetBlogEdit/{id}")]
        public async Task<ActionResult<IEnumerable<object>>> GetBlogEdit(int id)
        { 
            var query = _context.Blogs
             .Where(s => s.IsActive == true && s.IsDelete == false && s.Id == id)
             .Select(s => new
             {
                 Id = s.Id,
                 TieuDe = s.TieuDe,
                 NoiDung = s.NoiDung,
                 Image = _context.ImageBlogs.Where(q => q.FkBlogId == s.Id).Select(q => q.ImageName).FirstOrDefault(),
             });
            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<object>>.Success(results));
        }
         
    }
}
