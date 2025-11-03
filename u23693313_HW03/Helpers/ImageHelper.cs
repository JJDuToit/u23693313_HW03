using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u23693313_HW03.Helpers
{
    public static class ImageHelper
    {
        private static readonly Dictionary<string, string> BrandImageMap = new Dictionary<string, string>
        {
            { "Electra", "Electra_Cruiser 1.jpeg" },
            { "Haro", "Haro_Bikes1.jpeg" },
            { "Heller", "Heller_bike1.jpeg" },
            { "Pure Cycles", "Pure_Cycles1.jpeg" },
            { "Ritchey", "Riychey_bike1.jpeg" },
            { "Strider", "Strider_balance_bike1.jpeg" },
            { "Sun Bicycles", "Sun_Bicycles1.jpeg" },
            { "Surly", "Surly_bike.jpeg" },
            { "Trek", "Trek_Marlin_bike.jpeg" }
        };

        public static string GetImagePath(string brandName)
        {
            if (brandName != null && BrandImageMap.TryGetValue(brandName, out string filename))
            {
                return $"~/Content/Images/Bikes/{filename}";
            }

            return "~/Content/Images/Bikes/default.jpeg";
        }
    }
}