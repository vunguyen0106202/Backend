using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace API.Dtos
{
    public class GiaSanPhamMauSacSanPhamSize
    {
        public int Id { get; set; }
        public string MaMau { get; set; }
        public string TenSanPham { get; set; }
        public string TenSize { get; set; }
        public int Id_SanPham { get; set; }
        public int Id_Mau { get; set; }
        public int Id_Size { get; set; }
        public int SoLuongTon { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
