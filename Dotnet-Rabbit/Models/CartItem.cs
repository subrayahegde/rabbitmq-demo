using Microsoft.EntityFrameworkCore;

namespace OrderService.Models
{
    public class CartItem
    {     
        public int Id { get; set; }
        public int Qty { get; set; }
    }
}
