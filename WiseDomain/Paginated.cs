using System.Collections;

namespace WiseDomain
{
    public class Paginated<T> where T: IEnumerable
    {
        public T List { get; set; }

        public int Page { get; set; }

        /// <summary>
        /// Gets total amount of pages possible, 0 = if total amount is unknown
        /// </summary>
        public int TotalPages { get; set; }

        public int ItemsPerPage { get; set; }

        public int Skip => (Page - 1) * ItemsPerPage;

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage => Page > 1;
    }
}