using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCommon.Collections
{
    public interface IPaginatedList
    {
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
    }
}
