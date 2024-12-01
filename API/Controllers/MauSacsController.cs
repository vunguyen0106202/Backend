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
    public class MauSacsController : Controller
    {
        private readonly DPContext _context;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        public MauSacsController(DPContext context, IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        // GET: api/MauSacs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MauSacLoai>>> GetMauSacs(string key, int offset, int limit = 20, string sortField = "", string sortOrder = "asc")
        {
            //var kb = from l in _context.Loais
            //         join s in _context.MauSacs
            //         on l.Id equals s.Id_Loai
            //         select new MauSacLoai()
            //         {
            //             Id = s.Id,
            //             Id_Loai = l.Id,
            //             TenLoai = l.Ten,
            //            MaMau = s.MaMau
            //         };
            //return await kb.ToListAsync();
            var query = from s in _context.MauSacs
                        join c in _context.Loais on s.Id_Loai equals c.Id
                        where s.IsActive == true && s.IsDelete == false && c.IsActive == true && c.IsDelete == false
                        select new MauSacLoai
                        {
                            Id = s.Id,
                            Id_Loai = (int)s.Id_Loai,
                            TenLoai = c.Ten,
                            MaMau = s.MaMau,
                            CreatedDate = s.CreatedDate,
                            IsActive = s.IsActive,
                            IsDelete = s.IsDelete,
                            UpdatedDate = s.UpdatedDate,
                            DeletedDate = s.DeletedDate
                        };

            query = query.Where(s => string.IsNullOrEmpty(key) || s.MaMau.Contains(key) || s.TenLoai.Contains(key));

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField)
                {
                    case "tenSize":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.MaMau) : query.OrderByDescending(s => s.MaMau);
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

            var totalCount = await _context.MauSacs
                .Join(_context.Loais, s => s.Id_Loai, c => c.Id, (s, c) => new { s, c })
                .Where(sc => sc.s.IsActive == true && sc.s.IsDelete == false && sc.c.IsActive == true && sc.c.IsDelete == false)
                .Where(sc => string.IsNullOrEmpty(key) || sc.s.MaMau.Contains(key) || sc.c.Ten.Contains(key))
                .CountAsync();

            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<MauSacLoai>>.Success(results, totalCount, ""));
        }
        [HttpGet("mausac")]
        public async Task<ActionResult> GetMauSac()
        {
            var resuft = await _context.MauSacs.Select(d => new
            {
                tenmau = d.MaMau
            }).Distinct().ToListAsync();
            return Json(resuft);
        }
        [HttpPost("mau")]
        public IActionResult getListMauSac([FromBody] JObject json)
        {
            var id = int.Parse(json.GetValue("id_san_pham").ToString());
            var mausac_id = _context.SanPhamBienThes.Where(d => d.Id_SanPham == id).Select(d => d.Id_Mau).ToList();
            var resuft = _context.MauSacs.Where(d => mausac_id.Contains(d.Id)).
                Select(d => new {
                    mau = d.MaMau
                }).ToList();
            return Json(resuft);
        }
        [HttpGet("GetColorFollowCategory")]
        public async Task<ActionResult<IEnumerable<TenMauLoai>>> GetMauSacLoai()
        {
            var kb = from m in _context.MauSacs
                     join l in _context.Loais
                     on m.Id_Loai equals l.Id
                     select new TenMauLoai()
                     {
                         Id = m.Id,
                        LoaiTenMau = l.Ten+" "+m.MaMau
                     };
            var result = await kb.ToListAsync();
            return Ok(Result<object>.Success(result));
        }
        // GET: api/MauSacs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MauSac>> GetMauSac(int id)
        {
            //var mauSac = await _context.MauSacs.FindAsync(id);
            //if (mauSac == null)
            //{
            //    return NotFound();
            //}
            //return mauSac;
            var result = await _context.MauSacs
          .Include(s => s.Loai)
          .Where(s => s.Id == id)
          .Select(s => new MauSacLoai
          {
              Id = s.Id,
              Id_Loai = s.Id_Loai,
              TenLoai = s.Loai.Ten,
              MaMau = s.MaMau,
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
        // PUT: api/MauSacs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMauSac(int id, [FromForm]UploadMauSac upload)
        {
            MauSac mausac;
            mausac = await _context.MauSacs.FindAsync(id);
            mausac.MaMau = upload.MaMau;
           mausac.Id_Loai = upload.Id_Loai;
            _context.MauSacs.Update(mausac);
            Notification notification = new Notification()
            {
                TenSanPham = upload.MaMau,
                TranType = "Edit"
            };
            _context.Notifications.Add(notification);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MauSacExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            await _hubContext.Clients.All.BroadcastMessage();
            return Ok();
        }
        // POST: api/MauSacs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MauSac>> PostMauSac([FromForm] UploadMauSac upload)
        { 
            MauSac mausac;
            if(upload.Id > 0)
            {
                mausac = await _context.MauSacs.FindAsync(upload.Id);
                if (mausac == null)
                {
                    return NotFound();
                }
                mausac.MaMau = upload.MaMau;
                mausac.Id_Loai = upload.Id_Loai;
                mausac.UpdatedDate = DateTime.Now;
                //_context.MauSacs.Update(mausac);
            }
            else
            {
                mausac = new MauSac()
                {
                    MaMau = upload.MaMau,
                    Id_Loai = upload.Id_Loai,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDelete = false,
                };
                _context.MauSacs.Add(mausac);
            }

            await _context.SaveChangesAsync(); 
            return Ok(Result<MauSac>.Success(mausac));
            //Notification notification = new Notification()
            //{
            //    TenSanPham = upload.MaMau,
            //    TranType = "Add"
            //};
            //_context.Notifications.Add(notification);
            //MauSac mausac = new MauSac()
            //{
            //    MaMau = upload.MaMau,
            //    Id_Loai = upload.Id_Loai,
            //};
            //_context.MauSacs.Add(mausac);
            //await _context.SaveChangesAsync();
            //await _hubContext.Clients.All.BroadcastMessage();
            //return Ok();
        }
        // DELETE: api/MauSacs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMauSac(int id)
        {
            var mauSac = await _context.MauSacs.FindAsync(id);
            mauSac.IsActive = false;
            mauSac.IsDelete = true;
            mauSac.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(Result<object>.Success(mauSac));
            //Notification notification = new Notification()
            //{
            //    TenSanPham = mauSac.MaMau,
            //    TranType = "Delete"
            //};
            //_context.Notifications.Add(notification);
            //if (mauSac == null)
            //{
            //    return NotFound();
            //}
            //_context.MauSacs.Remove(mauSac);
            //await _context.SaveChangesAsync();
            //await _hubContext.Clients.All.BroadcastMessage();
            //return Ok();
        }
        private bool MauSacExists(int id)
        {
            return _context.MauSacs.Any(e => e.Id == id);
        }
    }
}
