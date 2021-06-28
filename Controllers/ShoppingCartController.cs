using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApp.Data;
using StoreApp.Models;
using StoreApp.ViewModels;

namespace StoreApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.ShoppingCartItems
                .Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .Select(x => x.ProductId).ToListAsync();
            if (products == null)
            {
                return null;
            }

            var applicationDbContext = await _context.Products.Where(x=> products.Contains(x.Id)).Include(p => p.Category).ToListAsync();
            var result = applicationDbContext.Select(product => new ShoppingCartViewModel
                {
                    Id = product.Id,
                    Title = product.Title,
                    Category = product.Category,
                    Price = product.Price,
                    Size = product.Size,
                    Color = product.Color,
                    ImageName = product.ImageName,
                    Count = products.Count(x => x == product.Id)
                })
                .ToList();
            return View(result);
        }
        [HttpPost, ActionName("GetShoppingCartCount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetShoppingCartCount()
        {
            var count = await _context.ShoppingCartItems.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).CountAsync();
            return new ObjectResult(new { status = "success", count });               
        }

        public async Task<IActionResult> RemoveFromCart(int Id)
        {
            var product = await _context.ShoppingCartItems.FirstOrDefaultAsync(x => 
                x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value 
                && x.ProductId == Id);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }
            _context.ShoppingCartItems.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        

        [HttpPost, ActionName("AddToCard")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCard(int productId, string userId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            { _context.ShoppingCartItems.Add(new ShoppingCartItems(){ProductId = productId, UserId = userId });
                await _context.SaveChangesAsync();
                var count = _context.ShoppingCartItems.Count(x=> x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value );
                return new ObjectResult(new { status = "success", count });
            }
            return new ObjectResult(new { status = "fail" });
        }
    }
}