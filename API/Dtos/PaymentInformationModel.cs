using System;

namespace API.Dtos
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
        public string GhiChu { get; set; } //ghi chu
        public int? TrangThai { get; set; }
        public int? LoaiThanhToan { get; set; } // kiểu thanh toán
        public bool? IsPayed { get; set; } // đã thanh toán
        public decimal TongTien { get; set; }
        public string Tinh { get; set; }
        public string Huyen { get; set; }
        public string Xa { get; set; }
        public string DiaChi { get; set; }
        public string? Id_User { get; set; }
    }
}
