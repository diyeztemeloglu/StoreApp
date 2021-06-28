using System.ComponentModel.DataAnnotations;
using StoreApp.Enums;
using StoreApp.Models;

namespace StoreApp.ViewModels
{
    public class ShoppingCartViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string ImageName { get; set; }
        public Size Size { get; set;}
        public Color Color { get; set;}
        
        public Category Category { get; set; }

        public int Count { get; set; }


    }
}