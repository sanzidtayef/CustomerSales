using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CustomerSales.Models;
using CustomerSales.Models.CustomViewModels;



namespace CustomerSales.Controllers
{
    [Route("api/Sale")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly CustomerSalesDBContext _CustomerSalesDBContext;
        public SalesController(CustomerSalesDBContext CustomerSalesDBContext)
        {
            _CustomerSalesDBContext = CustomerSalesDBContext;
        }

        #region Sales Start
        [HttpGet("getSaleList")]
        public async Task<IActionResult> GetSaleList()
        {
            try
            {
                var salelist = await (from sl in _CustomerSalesDBContext.Sales
                                                    join df in _CustomerSalesDBContext.Customers on sl.VCustomerId equals df.VCustomerId
                                                    select new {
                                                        sl.VSalesId,
                                                        sl.VCustomerId,
                                                        df.VCustomerFirstName,
                                                        sl.DDate,
                                                        sl.NInvoiceNo
                                                    }).OrderBy(x => x.VCustomerFirstName).ToListAsync();
                var customerlist = await (from c in _CustomerSalesDBContext.Customers
                                        select new {
                                            c.VCustomerId,
                                            c.VCustomerFirstName
                                        }).ToListAsync();
                var productlist = await (from pro in _CustomerSalesDBContext.Products
                                        select new {
                                            pro.VProductId,
                                            pro.VProductName
                                        }).ToListAsync();
                var data = new { SALELIST = salelist, CUSTOMERLIST = customerlist, PRODUCTLIST = productlist };
                return Ok(data);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return BadRequest(ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost("saveSale")]
        public async Task<IActionResult> SaveSale(SaleViewModel data)
        {
            using (var dbContextTransaction = _CustomerSalesDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                     
                        return BadRequest("Invalid");
                    }
                    if (data.VSalesId == null)
                    {
                        data.VSalesId = Guid.NewGuid().ToString();
                        _CustomerSalesDBContext.Sales.Add(data);
                        _CustomerSalesDBContext.SaveChanges();
                    }
                    else
                    {
                        _CustomerSalesDBContext.Entry(data).State = EntityState.Modified;
                        _CustomerSalesDBContext.SaveChanges();
                    }

                    _CustomerSalesDBContext.SalesDetails.RemoveRange(_CustomerSalesDBContext.SalesDetails.Where(x => x.VSalesId == data.VSalesId));
                    _CustomerSalesDBContext.SaveChanges();

                    foreach (var sd in data.SalesDetails)
                    {
                        sd.VSalesDetailsId = Guid.NewGuid().ToString();
                        sd.VSalesId = data.VSalesId;
                        _CustomerSalesDBContext.SalesDetails.Add(sd);
                        _CustomerSalesDBContext.SaveChanges();
                    }

                    dbContextTransaction.Commit();
                    return Ok();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    if (ex.InnerException != null)
                    {
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        [HttpGet("getSaleById")]
        public async Task<IActionResult> GetSaleById(string Id)
        {
            try
            {
                var sale = await (from sl in _CustomerSalesDBContext.Sales
                                                where sl.VSalesId == Id
                                                select new {
                                                    sl.VSalesId,
                                                    sl.DDate,
                                                    sl.NInvoiceNo,
                                                    sl.VCustomerId,
                                                    SalesDetail = (from sd in _CustomerSalesDBContext.SalesDetails
                                                                                where sd.VSalesId == Id
                                                                                select new {
                                                                                    sd.IAutoId,
                                                                                    sd.VSalesDetailsId,
                                                                                    sd.VSalesId,
                                                                                    sd.VProductId,
                                                                                    sd.NQuantity,
                                                                                    sd.NProductPrice
                                                                                }).OrderBy(x => x.IAutoId).ToList()
                                                }).ToListAsync();
                var data = new { SALE = sale };
                return Ok(data);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return BadRequest(ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet("deleteSale")]
        public async Task<IActionResult> DeleteSale(string Id)
        {
            using (var dbContextTransaction = _CustomerSalesDBContext.Database.BeginTransaction())
            {
                try
                {
                    _CustomerSalesDBContext.SalesDetails.RemoveRange(_CustomerSalesDBContext.SalesDetails.Where(x => x.VSalesId == Id));
                    _CustomerSalesDBContext.SaveChanges();

                    _CustomerSalesDBContext.Sales.RemoveRange(_CustomerSalesDBContext.Sales.Where(x => x.VSalesId == Id));
                    _CustomerSalesDBContext.SaveChanges();

                    dbContextTransaction.Commit();
                    return Ok();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    if (ex.InnerException != null)
                    {
                        return BadRequest(ex.InnerException.Message);
                    }
                    else
                    {
                        return BadRequest(ex.Message);
                    }
                }
            }
        }
        #endregion
    }
}