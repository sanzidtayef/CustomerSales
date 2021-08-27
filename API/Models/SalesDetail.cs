using System;
using System.Collections.Generic;


namespace CustomerSales.Models
{
    public partial class SalesDetail
    {
        public int IAutoId { get; set; }
        public string VSalesDetailsId { get; set; }
        public string VSalesId { get; set; }
        public string VProductId { get; set; }
        public decimal NQuantity { get; set; }
        public decimal NProductPrice { get; set; }

        public virtual Product VProduct { get; set; }
    }
}
