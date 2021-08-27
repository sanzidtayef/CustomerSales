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
    [Route("api/Customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerSalesDBContext _CustomerSalesDBContext;
        public CustomerController(CustomerSalesDBContext CustomerSalesDBContext)
        {
            _CustomerSalesDBContext = CustomerSalesDBContext;
        }

        #region Customer Start
        [HttpGet("getCustomerList")]
        public async Task<IActionResult> GetCustomerList()
        {
            try
            {
                var customerlist = await (from c in _CustomerSalesDBContext.Customers
                                        select new {
                                            c.VCustomerId,
                                            c.VCustomerFirstName,
                                            c.VCustomerLastName,
                                            c.VCustomerAddress
                                        }).ToListAsync();
                var data = new { CUSTOMERLIST = customerlist };
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

        [HttpPost("saveCustomer")]
        public async Task<IActionResult> saveCustomer(Customer data)
        {
            using (var dbContextTransaction = _CustomerSalesDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (data.VCustomerId == null)
                    {
                        data.VCustomerId = Guid.NewGuid().ToString();
                        _CustomerSalesDBContext.Customers.Add(data);
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

        [HttpGet("getCustomerById")]
        public async Task<IActionResult> GetCustomerById(string Id)
        {
            try
            {
                var customer = await (from c in _CustomerSalesDBContext.Customers
                                                where c.VCustomerId == Id
                                                select new {
                                                    c.VCustomerId,
                                                    c.VCustomerFirstName,
                                                    c.VCustomerLastName,
                                                    c.VCustomerAddress
                                                }).ToListAsync();
                var data = new { CUSTOMER = customer };
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

        [HttpGet("deleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(string Id)
        {
            using (var dbContextTransaction = _CustomerSalesDBContext.Database.BeginTransaction())
            {
                try
                {
                    _CustomerSalesDBContext.Customers.RemoveRange(_CustomerSalesDBContext.Customers.Where(x => x.VCustomerId == Id));
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