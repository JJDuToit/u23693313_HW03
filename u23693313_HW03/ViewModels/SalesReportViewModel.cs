using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u23693313_HW03.ViewModels
{
    public class SalesReportViewModel
    {
        public string CustomerName { get; set; }
        public string StaffName { get; set; }
        public string ProductName { get; set; }
        public int quantity { get; set; }
        public decimal list_price { get; set; }
        public decimal Total { get; set; }
        public DateTime order_date { get; set; }
    }
}