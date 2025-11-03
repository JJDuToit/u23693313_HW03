using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u23693313_HW03.ViewModels
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }

        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}