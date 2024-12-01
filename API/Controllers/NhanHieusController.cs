using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Dtos;
using API.Models;
using API.Helper.SignalR;
using API.Helper.Result;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanHieusController : ControllerBase
    {
        private readonly DPContext _context;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        public NhanHieusController(DPContext context, IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        // GET: api/ThuongHieus
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<NhanHieu>>> GetThuongHieus()
        //{
        //    return await _context.NhanHieus.ToListAsync();
        //}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanHieu>>> Gets(string key, int offset, int limit = 20, string sortField = "", string sortOrder = "asc")
        {
            var query = _context.NhanHieus.Where(s => s.IsActive == true && s.IsDelete == false)
                 .Select(s => new NhanHieu
                 {
                     Id = s.Id,
                     Ten = s.Ten,
                     CreatedDate = s.CreatedDate,
                     IsActive = s.IsActive,
                     IsDelete = s.IsDelete,
                     UpdatedDate = s.UpdatedDate,
                     DeletedDate = s.DeletedDate,
                 })
        .Where(s => string.IsNullOrEmpty(key) || s.Ten.Contains(key));

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField)
                {
                    case "ten":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.Ten) : query.OrderByDescending(s => s.Ten);
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

            var totalCount = await _context.NhanHieus
                .Where(s => s.IsActive == true && s.IsDelete == false)
                .Where(s => string.IsNullOrEmpty(key) || s.Ten.Contains(key))
                .CountAsync();

            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<NhanHieu>>.Success(results, totalCount, ""));
        }
        // GET: api/ThuongHieus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanHieu>> GetThuongHieu(int id)
        {
            var thuongHieu = await _context.NhanHieus.FindAsync(id);
            if (thuongHieu == null)
            {
                return NotFound();
            }
            return Ok(Result<object>.Success(thuongHieu));
        }
        // PUT: api/ThuongHieus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNhanHieu(int id, [FromForm] UploadBrand upload)
        {
            NhanHieu thuonghieu = new NhanHieu();
            thuonghieu = await _context.NhanHieus.FirstOrDefaultAsync(c => c.Id == id);
            thuonghieu.Ten = upload.Ten;
            thuonghieu.DateCreate = DateTime.Now;
            _context.NhanHieus.Update(thuonghieu);
            try
            {
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThuongHieuExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
              Notification notification = new Notification()
            {
                TenSanPham = upload.Ten,
                TranType = "Edit"
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.BroadcastMessage();
            return NoContent();
        }
        // POST: api/ThuongHieus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NhanHieu>> PostNhanHieu([FromForm] UploadBrand upload)
        {
            // NhanHieu nhanhieu = new NhanHieu();
            // nhanhieu.Ten = upload.Name;
            //nhanhieu.DateCreate = DateTime.Now;
            // _context.NhanHieus.Add(nhanhieu);
            // Notification notification = new Notification()
            // {
            //     TenSanPham = upload.Name,
            //     TranType = "Add"
            // };
            // _context.Notifications.Add(notification);
            // await _context.SaveChangesAsync();
            // await _hubContext.Clients.All.BroadcastMessage();
            // return Ok();
            NhanHieu nhanhieu;
            if (upload.Id > 0)
            {
                nhanhieu = await _context.NhanHieus.FindAsync(upload.Id);
                nhanhieu.Ten = upload.Ten;
                nhanhieu.UpdatedDate = DateTime.Now;
                //_context.NhanHieus.Update(nhanhieu);
            }
            else
            {
                nhanhieu = new NhanHieu()
                {
                    Ten = upload.Ten,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDelete = false,
                };
                _context.NhanHieus.Add(nhanhieu);
            }
            await _context.SaveChangesAsync(); 
            return Ok(Result<NhanHieu>.Success(nhanhieu));

        }
        // DELETE: api/ThuongHieus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThuongHieu(int id)
        {
            //SanPham[] product;
            //product = await _context.SanPhams.Where(b => b.Id_NhanHieu == id).ToArrayAsync();
            //var brand = await _context.NhanHieus.FindAsync(id);
            //if (product == null)
            //{
            //    _context.NhanHieus.Remove(brand);
            //    Notification notification = new Notification()
            //    {
            //        TenSanPham = brand.Ten,
            //        TranType = "Delete"
            //    };
            //    _context.Notifications.Add(notification);
            //    await _hubContext.Clients.All.BroadcastMessage();
            //    await _context.SaveChangesAsync();
            //}
            //else
            //{
            //    _context.SanPhams.RemoveRange(product);
            //    await _context.SaveChangesAsync();
            //    _context.NhanHieus.Remove(brand);
            //    Notification notification = new Notification()
            //    {
            //        TenSanPham = brand.Ten,
            //        TranType = "Delete"
            //    };
            //    _context.Notifications.Add(notification);
            //    await _hubContext.Clients.All.BroadcastMessage();
            //    await _context.SaveChangesAsync();
            //}
            var thuonghieu = await _context.NhanHieus.FindAsync(id);
            thuonghieu.IsActive = false;
            thuonghieu.IsDelete = true;
            thuonghieu.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(Result<object>.Success(thuonghieu));
        }
        private bool ThuongHieuExists(int id)
        {
            return _context.NhanHieus.Any(e => e.Id == id);
        }
    }
}
