using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace API.Models
{
    [Table("NhanHieus")]
    public class NhanHieu
    {
        [Key]
        public int Id { get; set; }
        public string Ten { get; set; }
        public System.DateTime? DateCreate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
