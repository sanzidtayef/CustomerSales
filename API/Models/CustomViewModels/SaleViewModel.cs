using CustomerSales.Models;
using System.Collections.Generic;


namespace CustomerSales.Models.CustomViewModels
{
    public class SaleViewModel : Sale
    {
        public List<SalesDetail> SalesDetails {get; set;}
    }
}