using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace API.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public virtual List<ImageBlog> ImageBlogs { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public string Image { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? FkAppUser_NguoiThem { get; set; }
        [ForeignKey("FkAppUser_NguoiThem")]
        public virtual AppUser AppUser { get; set; }
    }
}
