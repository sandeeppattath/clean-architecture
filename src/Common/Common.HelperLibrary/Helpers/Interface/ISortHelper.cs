namespace Common.HelperLibrary.Helpers.Interface
{
    public interface ISortHelper<T>
    {
        IQueryable<T> ApplySort(IQueryable<T> entities, string orderByQueryString);
        List<T> ApplySort(List<T> entities, string orderByQueryString);

    }
}