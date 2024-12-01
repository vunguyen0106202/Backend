using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Models;
using API.Helper;
using API.Helper.SignalR;
using API.Helper.Result;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaGiamGiasController : ControllerBase
    {
        private readonly DPContext _context;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        public MaGiamGiasController(DPContext context, IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            this._hubContext = hubContext;
            this._context = context;
            this._serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaGiamGia>>> GetMaGiamGias(string key, int offset, int limit = 20, string sortField = "" , string sortOrder = "asc")
        {
            var query = _context.MaGiamGias.Where(s => s.IsActive == true && s.IsDelete == false)
                .Select(s => new MaGiamGia
                {
                    Id = s.Id,
                    Code = s.Code,
                    SoTienGiam = s.SoTienGiam,
                    SoLuong=s.SoLuong,
                    IsDelete = s.IsDelete,
                    IsActive = s.IsActive,
                    CreatedDate = s.CreatedDate,
                    UpdatedDate = s.UpdatedDate,
                    DeletedDate = s.DeletedDate,
                }).Where(s => string.IsNullOrEmpty(key) ||  s.SoTienGiam.ToString().Contains(key));

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField)
                {
                    case "soTienGiam":
                        query = sortOrder == "asc" ? query.OrderBy(s => s.SoTienGiam) : query.OrderByDescending(s => s.SoTienGiam);
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

            var totalCount = await _context.MaGiamGias
                .Where(s => s.IsActive == true && s.IsDelete == false)
                .Where(s => string.IsNullOrEmpty(key) || s.Code.Contains(key))
                .CountAsync();

            var results = await query.ToListAsync();

            return Ok(Result<IEnumerable<MaGiamGia>>.Success(results, totalCount, ""));
        }
        [HttpPost]
        public async Task<ActionResult<MaGiamGia>> TaoMaGiamGia([FromForm] UploadMaGiamGia uploadMaGiamGia )
        {
            MaGiamGia magiamgia;
            if (uploadMaGiamGia.Id > 0)
            {
                magiamgia = await _context.MaGiamGias.FindAsync(uploadMaGiamGia.Id);
                magiamgia.SoTienGiam = uploadMaGiamGia.SoTienGiam;
                magiamgia.SoLuong = uploadMaGiamGia.SoLuong;
                magiamgia.UpdatedDate = DateTime.Now;
                //_context.MaGiamGias.Update(magiamgia);
            }
            else
            {
                magiamgia = new MaGiamGia()
                {
                    SoTienGiam = uploadMaGiamGia.SoTienGiam,
                    SoLuong=uploadMaGiamGia.SoLuong,
                    Code = StringHelper.RandomString(5),
                    IsActive = true,
                    IsDelete = false,
                    CreatedDate = DateTime.Now,
                };
                _context.Add(magiamgia);
            }
            await _context.SaveChangesAsync();
            //MaGiamGia maGiamGia = new MaGiamGia();
            //maGiamGia.Code= StringHelper.RandomString(5);
            //maGiamGia.SoTienGiam= uploadMaGiamGia.SoTienGiam;
            //_context.Add(maGiamGia);
            //await _context.SaveChangesAsync();
            //await _hubContext.Clients.All.BroadcastMessage();
            //return Ok();
            return Ok(Result<MaGiamGia>.Success(magiamgia));
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> SuaMaGiamGia([FromForm] UploadMaGiamGia uploadMaGiamGia,int id)
        {
            MaGiamGia maGiamGia;
            maGiamGia = await _context.MaGiamGias.FindAsync(id);
            maGiamGia.Code = StringHelper.RandomString(5);
            maGiamGia.SoTienGiam = uploadMaGiamGia.SoTienGiam;
            maGiamGia.SoLuong = uploadMaGiamGia.SoLuong;
            _context.Update(maGiamGia);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.BroadcastMessage();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMaGiamGias(int id)
        {
            //MaGiamGia mgg;
            //mgg = await _context.MaGiamGias.FindAsync(id);
            //_context.MaGiamGias.Remove(mgg);
            //await _hubContext.Clients.All.BroadcastMessage();
            //await _context.SaveChangesAsync();
            //return Ok();
            var mgg = await _context.MaGiamGias.FindAsync(id);
            mgg.IsActive = false;
            mgg.IsDelete = true;
            mgg.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(Result<object>.Success(mgg));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaGiamGia>> GetById(int id)
        {
            var ma = await _context.MaGiamGias.FindAsync(id);
            if (ma == null)
            {
                return NotFound();
            }
            return Ok(Result<object>.Success(ma));
        }
    }
}
