using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApp.Data;
using StoreApp.Models;
using StoreApp.ViewModels;

namespace StoreApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.Include(x=>x.OrderItems).Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .ToListAsync();
            var result = new List<OrderViewModel>();
            foreach (var order in orders)
            {
                result.Add(new OrderViewModel()
                {
                    Id = order.Id,
                    OrderTotal = order.OrderTotal,
                    OrderItems = order.OrderItems,
                    UserId = order.UserId,
                });
            }
            return View(result);
        }

        public async Task<IActionResult> Buy()
        {
            var products = await _context.ShoppingCartItems
                .Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync();
            if (products == null)
            {
                return null;
            }

            var applicationDbContext = await _context.Products.Where(x=> products.Select(x=>x.ProductId).Contains(x.Id)).Include(p => p.Category).ToListAsync();
            var shoppingCart = applicationDbContext.Select(product => new ShoppingCartViewModel
                {
                    Id = product.Id,
                    Title = product.Title,
                    Category = product.Category,
                    Price = product.Price,
                    Size = product.Size,
                    Color = product.Color,
                    ImageName = product.ImageName,
                    Count = products.Count(x => x.ProductId == product.Id)
                })
                .ToList();
            var orderTotal = shoppingCart.Select(x => x.Price * products.Count(xx => xx.ProductId == x.Id)).Sum();
            var newOrderItems = new List<OrderItem>();
            var productsList = await _context.Products.Where(x => products.Select(aa=>aa.ProductId).Contains(x.Id)).ToListAsync();
            foreach (var cartItem in shoppingCart)
            {
                newOrderItems.Add(new OrderItem()
                {
                    Product = productsList.FirstOrDefault(x => x.Id == cartItem.Id),
                });
            }

            await _context.Orders.AddAsync(new Order()
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                OrderTotal = orderTotal,
                OrderItems = newOrderItems,
            });
            _context.ShoppingCartItems.RemoveRange(products);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}