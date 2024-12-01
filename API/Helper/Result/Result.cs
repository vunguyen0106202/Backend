using API.Dtos;
using API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Helper.Result
{
    public class Result<T> : IResult<T>
    {
        public bool Status { get; set; } = true;

        public T Data { get; set; }

        public string Message { get; set; }

        public int? TotalRecord { get; set; }

        public T DataTotal { get; set; }

        public int? Code { get; set; }

        public static Result<T> Success(T data, int totalRecord = 0, string message = "", T dataTotal = default(T))
        {
            return new Result<T>
            {
                Data = data,
                Message = message,
                TotalRecord = totalRecord,
                DataTotal = dataTotal
            };
        }

        public static Result<T> Error(string message = "")
        {
            return new Result<T>
            {
                Status = false,
                Message = message
            };
        }

        internal static object Success(List<PhieuNhapHangNhaCungCap> results, Task<int> totalCount, string v)
        {
            throw new NotImplementedException();
        }

        internal static object Success(Task<List<SanPhamLoaiThuongHieu>> r)
        {
            throw new NotImplementedException();
        }
    }
}
