namespace Moshrefy.Domain.Paramter
{
    public class PaginationParamter
    {
        private const int MaxPageSize = 200;  // Increased from 40 to allow showing 100 records per page
        private int? pageSize;
        public int? PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
        public int? PageNumber { get; set; } = 1;

    }
}
