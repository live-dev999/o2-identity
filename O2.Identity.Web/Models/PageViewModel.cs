using System;

namespace O2.Identity.Web.Models
{
    public class PageViewModel
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
 
        public PageViewModel(int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public int TotalCount { get; private set; }

        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }
 
        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
    }
}