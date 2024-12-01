namespace API.Helper.Result
{
    public interface IResult<T>
    {
        bool Status { get; set; }

        T Data { get; set; }

        string Message { get; set; }

        int? TotalRecord { get; set; }
    }
}
