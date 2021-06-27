using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using StoreApp.Enums;
using StoreApp.Models;

namespace StoreApp.ViewModels
{
    public class ProductViewModel 
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Price { get; set; }
        [Required]
        public string ImageName { get; set; }
        public Size Size { get; set;}
        public Color Color { get; set;}
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        
        public IFormFile NewImage { get; set; }  
        
    }
}