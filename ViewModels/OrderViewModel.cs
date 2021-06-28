using System.Collections.Generic;
using StoreApp.Models;

namespace StoreApp.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal OrderTotal { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}