using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using API.Data;
using API.Dtos;
using API.Models;
using API.Helper.SignalR;
using API.Helper.Result;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : Controller
    {
        private readonly DPContext _context;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        public SizesController(DPContext context, IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        [HttpPost("sizetheomau")]
        public IActionResult getListSizeTheoMau([FromBody] JObject json)
        {
            try
            {
                var id = int.Parse(json.GetValue("id_san_pham").ToString());
                var mamau = json.GetValue("mamau").ToString();
                var id_loai_sp = _context.SanPhams.Where(d => d.Id == id).Select(d => d.Id_Loai).SingleOrDefault();
                var mauloai = mamau + id_loai_sp;
                var id_mau = _context.MauSacs.Where(d => d.MaMau+d.Id_Loai == mauloai && d.Id_Loai == id_loai_sp).Select(d => d.Id).SingleOrDefault();
                var list_idsize = _context.SanPhamBienThes.Where(d => d.Id_Mau == id_mau && d.Id_SanPham == id).Select(d => d.SizeId.ToString()).ToList();
                var resuft = _context.Sizes.Where(d => list_idsize.Contains(d.Id.ToString())).Select(
                    d => new
                    {
                        size = d.TenSize
                    }).ToList();
                return Json(resuft);
            }
            catch (Exception ex)
            {
                var a = ex;
                return Json("");
            }
        }
        // GET: api/Sizes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SizeLoai>>> GetSizes(string key, int offset, int limit = 20, string sortField = "", string sortOrder = "asc")
        {
            var query = from s in _context.Sizes
                        join c in _context.Loais on s.Id_Loai equals c.Id
                        where s.IsActive == true && s.IsDelete == false && c.IsActive == true && c.IsDelete == false
                        select new SizeLoai
                        {
                            Id = s.Id,
                            Id_Loai = (int)s.Id_Loai,
                            TenLoai = c.Ten,
                            TenSize = s.TenSize,
                            CreatedDate = s.CreatedDate,
                            IsActive = s.IsActive,
                            IsDelete = s.IsDelete,
                            UpdatedDate = s.UpdatedDate,
                            DeletedDate = s.DeletedDate
                        };

            query = query.Where(s => string.IsNullOrEmpty(key) || s.TenSize.Contains(key) || s.TenLoai.Contains(key));

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField)
                {
                    case "tenSize":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.TenSize) : query.OrderByDescending(s => s.TenSize);
                        break;
                    case "tenLoai":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.TenLoai) : query.OrderByDescending(s => s.TenLoai);
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

            var totalCount = await _context.Sizes
                .Join(_context.Loais, s => s.Id_Loai, c => c.Id, (s, c) => new { s, c })
                .Where(sc => sc.s.IsActive == true && sc.s.IsDelete == false && sc.c.IsActive == true && sc.c.IsDelete == false)
                .Where(sc => string.IsNullOrEmpty(key) || sc.s.TenSize.Contains(key) || sc.c.Ten.Contains(key))
                .CountAsync();

            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<SizeLoai>>.Success(results, totalCount, ""));
        }

        [HttpGet("GetSizeFollowCategory")]
        public async Task<ActionResult<IEnumerable<TenSizeLoai>>> GetTenSizeLoais()
        {
            var kb = from m in _context.Sizes
                     join l in _context.Loais
                     on m.Id_Loai equals l.Id
                     select new TenSizeLoai()
                     {
                         Id = m.Id,
                         SizeLoaiTen = l.Ten+" _Size_ "+m.TenSize
                     };
            var result = await kb.ToListAsync();
            return Ok(Result<object>.Success(result));
        }
        // GET: api/Sizes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Size>> GetSize(int id)
        {
            //var size = await _context.Sizes.FindAsync(id);
            //if (size == null)
            //{
            //    return NotFound();
            //}
            var result = await _context.Sizes
             .Include(s => s.Loai)
             .Where(s => s.Id == id)
             .Select(s => new SizeLoai
             {
                 Id = s.Id,
                 Id_Loai = s.Id_Loai,
                 TenLoai = s.Loai.Ten,
                 TenSize = s.TenSize,
                 CreatedDate = s.CreatedDate,
                 UpdatedDate = s.UpdatedDate,
                 IsActive = s.IsActive,
                 IsDelete = s.IsDelete,
                 DeletedDate = s.DeletedDate
             }).FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }
            return Ok(Result<object>.Success(result));
        }
        // PUT: api/Sizes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSize(int id, [FromForm] UploadSize upload)
        {
            Size size;
            size = await _context.Sizes.FindAsync(id);
            size.TenSize = upload.TenSize;
            size.Id_Loai = upload.Id_Loai;
            _context.Sizes.Update(size);
            Notification notification = new Notification()
            {
                TenSanPham = upload.TenSize,
                TranType = "Edit"
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.BroadcastMessage();
            return NoContent();
        }
        // POST: api/Sizes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SizeLoai>> PostSize([FromForm]  UploadSize upload)
        {
            Size size;

            if (upload.Id > 0)
            {
                size = await _context.Sizes.FindAsync(upload.Id);
                if (size == null)
                {
                    return NotFound();
                }
                size.TenSize = upload.TenSize;
                size.Id_Loai = upload.Id_Loai;
                size.UpdatedDate = DateTime.Now;
                //_context.Sizes.Update(size);
            }
            else
            {
                size = new Size()
                {
                    TenSize = upload.TenSize,
                    Id_Loai = upload.Id_Loai,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDelete = false,
                };
                _context.Sizes.Add(size);
            }

            await _context.SaveChangesAsync();

            var result = await _context.Sizes
                .Include(s => s.Loai)
                .Where(s => s.Id == size.Id)
                .Select(s => new SizeLoai
                {
                    Id = s.Id,
                    Id_Loai = s.Id_Loai,
                    TenLoai = s.Loai.Ten,
                    TenSize = s.TenSize,
                    CreatedDate = s.CreatedDate,
                    UpdatedDate = s.UpdatedDate,
                    IsActive = s.IsActive,
                    IsDelete = s.IsDelete,
                    DeletedDate = s.DeletedDate
                }).FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(Result<SizeLoai>.Success(result));
        }

        // DELETE: api/Sizes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSize(int id)
        {
            var size = await _context.Sizes.FindAsync(id);
            size.IsActive = false;
            size.IsDelete = true;
            size.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(Result<object>.Success(size));
            //if (size == null)
            //{
            //    return NotFound();
            //}
            //_context.Sizes.Remove(size);
            //Notification notification = new Notification()
            //{
            //    TenSanPham = size.TenSize,
            //    TranType = "Delete"
            //};
            //_context.Notifications.Add(notification);
            //await _context.SaveChangesAsync();
            //await _hubContext.Clients.All.BroadcastMessage();
            //return NoContent();
        }
        private bool SizeExists(int id)
        {
            return _context.Sizes.Any(e => e.Id == id);
        }
    }
}
