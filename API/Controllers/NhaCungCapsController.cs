using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Models;
using API.Helper.SignalR;
using API.Helper.Result;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhaCungCapsController : ControllerBase
    {
        private readonly DPContext _context;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        public NhaCungCapsController(DPContext context, IHubContext<BroadcastHub, IHubClient> _hubContext)
        {
            this._context = context;
            this._hubContext = _hubContext;
        }
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<NhaCungCap>>> GetAllNhaCungCap()
        //{
        //    return await _context.NhaCungCaps.ToListAsync();
        //}
        // GET: api/Loais/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhaCungCap>> GetSupById(int id)
        {
            var nhacungcap = await _context.NhaCungCaps.FindAsync(id);
            if (nhacungcap == null)
            {
                return NotFound();
            }
            return Ok(Result<object>.Success(nhacungcap));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhaCungCap>>> Gets(string key, int offset, int limit = 20, string sortField = "", string sortOrder = "asc")
        {
            var query = _context.NhaCungCaps.Where(s => s.IsActive == true && s.IsDelete == false)
                 .Select(s => new NhaCungCap
                 {
                     Id = s.Id,
                     Ten = s.Ten,
                     SDT = s.SDT,
                     ThongTin = s.ThongTin,
                     DiaChi = s.DiaChi,
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

            var totalCount = await _context.NhaCungCaps
                .Where(s => s.IsActive == true && s.IsDelete == false)
                .Where(s => string.IsNullOrEmpty(key) || s.Ten.Contains(key))
                .CountAsync();

            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<NhaCungCap>>.Success(results, totalCount, ""));
        }

        [HttpPost]
        public async Task<ActionResult<NhaCungCap>> PostNhaCungCapAsync([FromForm] UploadNhaCungCap upload)
        {
            NhaCungCap nhacungcap;

            if (upload.Id > 0)
            {
                nhacungcap = await _context.NhaCungCaps.FindAsync(upload.Id);
                nhacungcap.Ten = upload.Ten;
                nhacungcap.SDT = upload.SDT;
                nhacungcap.ThongTin = upload.ThongTin;
                nhacungcap.DiaChi = upload.DiaChi;
                nhacungcap.UpdatedDate = DateTime.Now;
                //_context.NhaCungCaps.Update(nhacungcap);
            }
            else
            {
                nhacungcap = new NhaCungCap()
                {
                    Ten = upload.Ten,
                    SDT = upload.SDT,
                    ThongTin = upload.ThongTin,
                    DiaChi = upload.DiaChi,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDelete = false,
                };
                _context.NhaCungCaps.Add(nhacungcap);
            }
            await _context.SaveChangesAsync();
            //var nhacungcap = new NhaCungCap()
            //{
            //    Ten = upload.Ten,
            //    SDT = upload.SDT,
            //    ThongTin = upload.ThongTin,
            //    DiaChi = upload.DiaChi,
            //};
            //_context.NhaCungCaps.Add(nhacungcap);
            //await _context.SaveChangesAsync();
            //return Ok();
            return Ok(Result<NhaCungCap>.Success(nhacungcap));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNhaCungCapAsync([FromForm] UploadNhaCungCap upload,int id)
        {
            var ncungcap = await _context.NhaCungCaps.FindAsync(id);
            ncungcap.Ten = upload.Ten;
            ncungcap.SDT = upload.SDT;
            ncungcap.ThongTin = upload.ThongTin;
            ncungcap.DiaChi = upload.DiaChi;
            _context.NhaCungCaps.Update(ncungcap);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNhaCungCapAsync(int id)
        {
            //SanPham[] product;
            //product = await _context.SanPhams.Where(s => s.Id_Loai == id).ToArrayAsync();
            //var nhacungcap = await _context.NhaCungCaps.FindAsync(id);
            //if (product == null)
            //{
            //    _context.NhaCungCaps.Remove(nhacungcap);
            //    await _context.SaveChangesAsync();
            //    await _hubContext.Clients.All.BroadcastMessage();
            //}
            //else
            //{
            //    _context.SanPhams.RemoveRange(product);
            //    await _context.SaveChangesAsync();
            //    _context.NhaCungCaps.Remove(nhacungcap);
            //    _context.SaveChanges();
            //    await _hubContext.Clients.All.BroadcastMessage();
            //}
            var nhacungcap = await _context.NhaCungCaps.FindAsync(id);
            nhacungcap.IsActive = false;
            nhacungcap.IsDelete = true;
            nhacungcap.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(Result<object>.Success(nhacungcap));
        }
    }
}
