using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace API.Models
{
    public class Loai
    {
        [Key]
        public int Id { get; set; }
        public string Ten { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public virtual ICollection<MauSac> MauSacs { get; set; }
        public virtual ICollection<SanPham> SanPhams { get; set; }
        public virtual ICollection<Size> Sizes { get; set; }
    }
}
