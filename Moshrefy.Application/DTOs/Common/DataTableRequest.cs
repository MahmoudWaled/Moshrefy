namespace Moshrefy.Application.DTOs.Common
{
    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public Search? Search { get; set; }
        public List<Column>? Columns { get; set; }
        public List<Order>? Order { get; set; }
        
        // Custom Filters
        public string? FilterDeleted { get; set; }
        public string? ActiveFilter { get; set; }
        public string? FilterRole { get; set; }
        public string? FilterStatus { get; set; } // e.g. "active", "inactive"
        public int? AcademicYearId { get; set; }
        
        // Advanced Search specifics
        public string? CenterName { get; set; }
        public string? Email { get; set; }
        public string? CreatedByName { get; set; }
        public string? AdminName { get; set; }

        public int PageSize => Length > 0 ? Length : 10;
        public int PageNumber => (Start / PageSize) + 1;
        public int Skip => Start;
        public string? SearchValue => Search?.Value;
        public string? SortColumnName => Columns != null && Order != null && Order.Any() 
            ? Columns[Order[0].Column].Name 
            : null;
        public string? SortDirection => Order != null && Order.Any() 
            ? Order[0].Dir 
            : null;
    }

    public class Search
    {
        public string? Value { get; set; }
        public bool Regex { get; set; }
    }

    public class Column
    {
        public string? Data { get; set; }
        public string? Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public Search? Search { get; set; }
    }

    public class Order
    {
        public int Column { get; set; }
        public string? Dir { get; set; }
    }
}
