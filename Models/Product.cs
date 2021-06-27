using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StoreApp.Enums;
using StoreApp.Models;

public class Product
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
}