using System;
using System.Collections.Generic;


namespace CustomerSales.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Sales = new HashSet<Sale>();
        }

        public string VCustomerId { get; set; }
        public string VCustomerFirstName { get; set; }
        public string VCustomerLastName { get; set; }
        public string VCustomerAddress { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
