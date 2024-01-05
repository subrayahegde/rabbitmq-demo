using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OrderService.Data;
using OrderService.Models;
using Microsoft.AspNetCore.Cors;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("OpenCORSPolicy")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductServiceContext _context;

        public ProductController(ProductServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            return await _context.Product.ToListAsync();
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<Product>> GetProductById(int productId)
        {
            return await _context.Product.FindAsync(productId);
        }
        
        private void PublishToMessageQueue(string integrationEvent, string eventData)
        {
            // TOOO: Reuse and close connections and channel, etc, 
            var factory = new ConnectionFactory {
               HostName = "localhost", UserName="admin", Password="password"
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(eventData);
            channel.BasicPublish(exchange: "user",
                                             routingKey: integrationEvent,
                                             basicProperties: null,
                                             body: body);
        }
       

        [HttpPost ("AddProduct")]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            var integrationEventData = JsonConvert.SerializeObject(new
            {
                id = product.ID,
                name = product.Name,
                description = product.Description,
                price = product.Price
            });
           
            PublishToMessageQueue("user.add", integrationEventData);

            return CreatedAtAction("GetProduct", new { id = product.ID }, product);
        }
        
        [HttpPost ("BuyProducts")]
        public async Task<ActionResult<String>> BuyProducts(CartItems cartItems)
        {   
              Console.WriteLine("Inside product controller...>>>");
              List<CartItem> items = cartItems.cartItems;
              Console.WriteLine(items);
              
              foreach (CartItem item in items) {
                //Product pr = _context.Product.First(x => x.ID == item.Id);
                Product pr = _context.Product.Find(item.Id);   
                Console.WriteLine(pr);
                if (pr == null) {                  
                    return ("Invalid Product Id");
                };           
              }
        
            var integrationEventData = JsonConvert.SerializeObject(cartItems);           
            PublishToMessageQueue("user.update", integrationEventData);        
            return ("Product Ordered");
        }
    }
}
