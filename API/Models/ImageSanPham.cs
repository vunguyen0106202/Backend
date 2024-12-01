using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace API.Models
{
    public class ImageSanPham
    {
        public int? Id { get; set; }
        //public string? ImageName { get; set; }
        public string ImageName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? IdSanPham { get; set; }
        [ForeignKey("IdSanPham")]
        public virtual SanPham? SanPham { get; set; }
    }
}
