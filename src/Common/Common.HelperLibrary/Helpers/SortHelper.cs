using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using Common.HelperLibrary.Helpers.Interface;

namespace Common.HelperLibrary.Helpers
{
    public class SortHelper<T> : ISortHelper<T>
	{
		public  IQueryable<T> ApplySort(IQueryable<T> entities, string orderByQueryString)
		{
            

            if (!entities.Any())
				return entities;

			if (string.IsNullOrWhiteSpace(orderByQueryString))
			{
				return entities;
			}

			var orderParams = orderByQueryString.Trim().Split(',');
			var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var orderQueryBuilder = new StringBuilder();

			foreach (var param in orderParams)
			{
				if (string.IsNullOrWhiteSpace(param))
					continue;

				var propertyFromQueryName = param.Split(" ")[0];
				var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

				if (objectProperty == null)
					continue;

				var sortingOrder = param.EndsWith("desc") ? "descending" : "ascending";

				orderQueryBuilder.Append($"{objectProperty.Name} {sortingOrder}, ");
			}

			var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

			return !string.IsNullOrEmpty(orderQuery)?    entities.OrderBy(orderQuery): entities;
		}

        public List<T> ApplySort(List<T> entities, string orderByQueryString)
        {
            string sortName = "";
            string sortingOrder = "";
            List<T> sortedList = new List<T>();
            if (!entities.Any())
                return entities;

            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                orderByQueryString = "UpdatedDate desc";
            }
            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();
            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty == null)
                    continue;

                sortingOrder = param.EndsWith("desc") ? "descending" : "ascending";
                sortName = objectProperty.Name;
                orderQueryBuilder.Append($"{objectProperty.Name} {sortingOrder}, ");
            }

            entities.Sort(new GenericComparer<T>(sortName, sortingOrder == "descending" ? GenericComparer<T>.SortOrder.Descending : GenericComparer<T>.SortOrder.Ascending));
            return entities;
        }


    }

    public  class GenericComparer<T> : IComparer<T>
    {
        public enum SortOrder { Ascending, Descending };
        #region member variables
        private string sortColumn;
        private SortOrder sortingOrder;
        #endregion
        #region constructor
        public GenericComparer(string sortColumn, SortOrder sortingOrder)
        {
            this.sortColumn = sortColumn;
            this.sortingOrder = sortingOrder;
        }
        #endregion
        #region public property
        /// <summary>
        /// Column Name(public property of the class) to be sorted.
        /// </summary>
        public string SortColumn
        {
            get { return sortColumn; }
        }
        /// <summary>
        /// Sorting order.
        /// </summary>
        public SortOrder SortingOrder
        {
            get { return sortingOrder; }
        }
        #endregion
        #region public methods
        /// <summary>
        /// Compare interface implementation
        /// </summary>
        /// <param name="x">custom Object</param>
        /// <param name="y">custom Object</param>
        /// <returns>int</returns>
        public int Compare(T x, T y)
        {
            PropertyInfo propertyInfo = typeof(T).GetProperty(sortColumn);
            IComparable obj1 = (IComparable)propertyInfo.GetValue(x, null);
            IComparable obj2 = (IComparable)propertyInfo.GetValue(y, null);
            if (sortingOrder == SortOrder.Ascending)
            {
                return (obj1.CompareTo(obj2));
            }
            else
            {
                return (obj2.CompareTo(obj1));
            }
        }
        #endregion
    }

}
