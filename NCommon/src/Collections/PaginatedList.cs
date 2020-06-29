using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCommon.Collections
{
    public class PaginatedList<T> : List<T>, IPaginatedList
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(IQueryable<T> source, int? pageIndex, int pageSize)
        {
            PageIndex = pageIndex ?? 1;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = ((TotalCount - 1) / PageSize) + 1;

            this.AddRange(source.Skip((PageIndex - 1) * PageSize).Take(PageSize));
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }
    }
}
