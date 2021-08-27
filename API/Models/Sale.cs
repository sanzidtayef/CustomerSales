using System;
using System.Collections.Generic;


namespace CustomerSales.Models
{
    public partial class Sale
    {
        public string VSalesId { get; set; }
        public string VCustomerId { get; set; }
        public DateTime DDate { get; set; }
        public decimal NInvoiceNo { get; set; }

        public virtual Customer VCustomer { get; set; }
    }
}
