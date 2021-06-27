using System.Collections;
using System.Collections.Generic;

namespace StoreApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal OrderTotal { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}