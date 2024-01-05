using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Cors;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("OpenCORSPolicy")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderServiceContext _context;

        public OrderController(OrderServiceContext context)
        {
            _context = context;
        }

        [HttpGet ("GetOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {        
            return await _context.Order.ToListAsync();          
        }

        [HttpGet ("GetProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Product.ToListAsync();    
        }
    }
}
