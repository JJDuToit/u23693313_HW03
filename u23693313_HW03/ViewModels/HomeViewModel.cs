using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using u23693313_HW03.Models;

namespace u23693313_HW03.ViewModels
{
    public class HomeViewModel
    {
        public staff StaffMember { get; set; }
        public customer Customer { get; set; }
        public product Product { get; set; }

        public IEnumerable<order_items> StaffSales { get; set; }
        public IEnumerable<order_items> CustomerPurchases { get; set; }

        public PagingInfo StaffPaging { get; set; }
        public PagingInfo CustomerPaging { get; set; }
        public PagingInfo ProductPaging { get; set; }

        public IEnumerable<SelectListItem> Brands { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}