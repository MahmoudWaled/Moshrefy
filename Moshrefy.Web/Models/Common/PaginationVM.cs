namespace Moshrefy.Web.Models.Common
{
    public class PaginationVM
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public string ActionName { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public string ItemName { get; set; } = "items"; // "centers", "students", etc.
        public int[] PageSizeOptions { get; set; } = [10, 20, 30, 50, 100];
        public bool ShowPageSizeDropdown { get; set; } = true;
        public string? searchQuery { get; set; }
    }
}