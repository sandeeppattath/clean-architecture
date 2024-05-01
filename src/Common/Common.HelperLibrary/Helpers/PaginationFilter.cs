namespace Common.HelperLibrary.Helpers
{
    public class PaginationBase
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public string Search { get; set; }
        public PaginationBase()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        public PaginationBase(int pageNumber, int pageSize, string sort, string search)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 10 ? 10 : pageSize;
            Sort = sort;
            Search = search;
        }
    }

    public class PaginationList: PaginationBase
    {
        public int? Id { get; set; }
    }
    public class PaginationFilter: PaginationBase
    {
        public int? Id { get; set; }
        public string Search { get; set; }
        public List<FilterUtility.FilterParams>? filterParams { get; set; }
        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        public PaginationFilter(string sort, string search, List<FilterUtility.FilterParams> filterParams, int? id)
        {          
            this.filterParams = filterParams;
        }
    }
}
