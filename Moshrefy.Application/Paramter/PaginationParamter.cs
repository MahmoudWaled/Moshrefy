namespace Moshrefy.Domain.Paramter
{
    public class PaginationParamter
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 30;
        private const int DefaultPageNumber = 1;

        private int pageSize = DefaultPageSize;
        private int pageNumber = DefaultPageNumber;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? DefaultPageSize : value);
        }

        public int PageNumber
        {
            get => pageNumber;
            set => pageNumber = value < 1 ? DefaultPageNumber : value;
        }
    }
}

