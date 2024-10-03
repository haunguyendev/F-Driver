namespace F_Driver.Service.BuisnessModels.QueryParameters
{
    public class TripRequestQueryParameters
    {
        public int? UserId { get; set; }
        public int? FromZoneId { get; set; }
        public int? ToZoneId { get; set; }
        public DateOnly? TripDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public int? Slot { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Sorting properties
        public string SortBy { get; set; } = "CreatedAt"; // Default sorting field
        public bool IsAscending { get; set; } = true;

        public int Page { get; set; } = 1;                  // Trang (mặc định: 1)
        public int PageSize { get; set; } = 10;
    }
}
