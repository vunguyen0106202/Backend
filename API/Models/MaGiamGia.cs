using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace API.Models
{
    public class MaGiamGia
    {
        public int Id { get; set; }
        public string Code { get; set; }     
        public decimal SoTienGiam { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public int? SoLuong { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
