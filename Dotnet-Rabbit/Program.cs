using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using OrderService.Data;
using OrderService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

string CORSOpenPolicy = "OpenCORSPolicy";

var factory = new ConnectionFactory {
    HostName = "localhost", UserName="admin", Password="password"
};
var connection = factory.CreateConnection();
var channel = connection.CreateModel();
var consumer = new EventingBasicConsumer(channel);

                
    consumer.Received += (model, ea) =>  { 
    var contextOptions = new DbContextOptionsBuilder<OrderServiceContext>()
            .UseSqlite(@"Data Source=sample.db")
            .Options;
            
     var dbContext = new OrderServiceContext(contextOptions);        

        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(" [x] Received {0}", message);
        var data = JObject.Parse(message);
        var type = ea.RoutingKey;
       
         if (type == "user.add")
         {
            dbContext.Product.Add(new Product()
            {
                ID = data["id"].Value<int>(),
                Name = data["name"].Value<string>(),
                Description = data["description"].Value<string>(),
                Price = data["price"].Value<decimal>()
            });
            dbContext.SaveChanges();
         }
         else if (type == "user.update")
         {            
            var cartObject = JsonConvert.DeserializeObject<CartItems>(message);
            List<CartItem> items = cartObject.cartItems;            
           
            int maxOrder = dbContext.Order.Max(p => p.OrderId);        
            decimal TotalPrice = 0;          
           
            foreach (CartItem item in items) 
            { 
               //Console.WriteLine("ProductId: " + item.Id + ", Qty: " + item.Qty);
                Product pr = dbContext.Product.First(x => x.ID == item.Id);
               
                decimal price = pr.Price;
                TotalPrice +=item.Qty * price; 
           
	        Order or = new Order()
	        {
	           OrderId = maxOrder+1,
	           ProductId = item.Id,
	           Qty = item.Qty,
	           CreatedOn = DateTime.Now
	        };           
	        dbContext.Order.Add(or);        
	        dbContext.SaveChanges();
	        maxOrder += 1;    		    
            } 
            Console.WriteLine ("Total Order Cost is: $" + TotalPrice);            
        }
    };
             
        channel.BasicConsume(queue: "user.postservice",
                                     autoAck: true,
                                     consumer: consumer);
 
         channel.BasicConsume(queue: "user.otherservice",
                                     autoAck: true,
                                     consumer: consumer);
                                     
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>{options.AddPolicy(
  name: CORSOpenPolicy, 
  builder => {    builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
  });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen((c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService", Version = "v1" }); 
        c.SwaggerDoc("v2", new OpenApiInfo { Title = "UserService", Version = "v1" });
    })
);

builder.Services.AddDbContext<OrderServiceContext>(options =>
         options.UseSqlite(@"Data Source=sample.db"));

builder.Services.AddDbContext<ProductServiceContext>(
 options => options.UseSqlite(@"Data Source=sample.db"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CORSOpenPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
