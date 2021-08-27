using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CustomerSales.Models;



namespace CustomerSales.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CustomerSalesDBContext _CustomerSalesDBContext;
        public ProductController(CustomerSalesDBContext CustomerSalesDBContext)
        {
            _CustomerSalesDBContext = CustomerSalesDBContext;
        }
        #region Product Start
        [HttpGet("getProductList")]
        public async Task<IActionResult> GetProductList()
        {
            try
            {
                var productlist = await (from pro in _CustomerSalesDBContext.Products
                                        select new {
                                            pro.VProductId,
                                            pro.VProductName,
                                            pro.NProductPrice
                                        }).ToListAsync();
                var data = new { PRODUCTLIST = productlist };
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

        [HttpPost("saveProduct")]
        public async Task<IActionResult> saveProduct(Product data)
        {
            using (var dbContextTransaction = _CustomerSalesDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (data.VProductId == null)
                    {
                        data.VProductId = Guid.NewGuid().ToString();
                        _CustomerSalesDBContext.Products.Add(data);
                        _CustomerSalesDBContext.SaveChanges();
                    }
                    else
                    {
                        _CustomerSalesDBContext.Entry(data).State = EntityState.Modified;
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

        [HttpGet("getProductById")]
        public async Task<IActionResult> GetProductById(string Id)
        {
            try
            {
                var product = await (from pro in _CustomerSalesDBContext.Products
                                                where pro.VProductId == Id
                                                select new {
                                                    pro.VProductId,
                                                    pro.VProductName,
                                                    pro.NProductPrice
                                                }).ToListAsync();
                var data = new { PRODUCT = product };
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

        [HttpGet("deleteProduct")]
        public async Task<IActionResult> deleteProduct(string Id)
        {
            using (var dbContextTransaction = _CustomerSalesDBContext.Database.BeginTransaction())
            {
                try
                {
                    _CustomerSalesDBContext.Products.RemoveRange(_CustomerSalesDBContext.Products.Where(x => x.VProductId == Id));
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