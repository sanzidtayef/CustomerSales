using System;
using System.Collections.Generic;


namespace CustomerSales.Models
{
    public partial class Product
    {
        public Product()
        {
            SalesDetails = new HashSet<SalesDetail>();
        }

        public string VProductId { get; set; }
        public string VProductName { get; set; }
        public decimal NProductPrice { get; set; }

        public virtual ICollection<SalesDetail> SalesDetails { get; set; }
    }
}
