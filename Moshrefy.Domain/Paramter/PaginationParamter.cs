namespace Moshrefy.Domain.Paramter
{
    public class PaginationParamter
    {
        private const int MaxPageSize = 40;
        private int? pageSize;
        public int? PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
        public int? PageNumber { get; set; } = 1;

    }
}
