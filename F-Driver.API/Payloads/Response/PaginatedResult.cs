namespace F_Driver.API.Payloads.Response
{
    public class PaginatedResult<T>
    {
        public int Page { get; set; }                  // Trang hiện tại
        public int PageSize { get; set; }              // Kích thước trang
        public int TotalItems { get; set; }            // Tổng số phần tử
        public int TotalPages { get; set; }            // Tổng số trang
        public IEnumerable<T> Data { get; set; }       // Dữ liệu của trang hiện tại
    }

}
