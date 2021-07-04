using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreApp.Data;
using StoreApp.Enums;
using StoreApp.Models;
using StoreApp.ViewModels;

namespace StoreApp.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        
        [AllowAnonymous]
        public async Task<IActionResult> Index(ProductFilters searchQuery)
        {
            var viewModel = new List<ProductViewModel>();
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            foreach (var product in products)
            {
                if (searchQuery.PriceLower > 0 && product.Price < searchQuery.PriceLower)
                {
                    continue;
                }

                if (searchQuery.PriceUpper > 0 && product.Price > searchQuery.PriceUpper)
                {
                    continue;
                }

                if (searchQuery.Size > 0 && searchQuery.Size != (int)product.Size)
                {
                    continue;
                }
                
                if (searchQuery.Color > 0 && searchQuery.Color != (int)product.Color)
                {
                    continue;
                }

                if (searchQuery.CategoryId > 0)
                {
                    if (product.CategoryId != searchQuery.CategoryId)
                    {
                        continue;
                    }
                }
                

                if (!string.IsNullOrWhiteSpace(searchQuery.SearchText))
                {
                    string[] words = searchQuery.SearchText.Split(' ');

                    var pass = false;
                    foreach (var word in words)
                    {
                        if (product.Title.Contains(word))
                        {
                            pass = true;
                        }

                        if (product.Description.Contains(word))
                        {
                            pass = true;
                        }
                    }

                    if (!pass)
                    {
                        continue;
                    }
                }


                viewModel.Add(new ProductViewModel
                    {
                        Id = product.Id,
                        Title = product.Title,
                        Description = product.Description,
                        Price = product.Price,
                        Size = product.Size,
                        Color = product.Color,
                        Category = product.Category,
                        CategoryId = product.CategoryId,
                        ImageName = product.ImageName,
                    }
                );

            }

            var defaultCategory = new Category
            {
                Id = 0,
                Title = "All"
            };
            var categories = await _context.Categories.ToListAsync();
            categories.Insert(0, defaultCategory);
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Title");
            
            var sizes = Enum.GetValues(typeof(Size)).Cast<Size>().Select(v => new SelectListItem
            {
                Text = v.ToString().Replace("_", ""),
                Value = ((int) v).ToString()
            }).ToList();
            var defaultFilterValue = new SelectListItem
            {
                Text = "All",
                Value = "0"
            };
            sizes.Insert(0, defaultFilterValue);
            ViewData["Size"] = new SelectList(sizes,"Value","Text");

            var colors = Enum.GetValues(typeof(Color)).Cast<Color>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int) v).ToString()
            }).ToList();
            colors.Insert(0, defaultFilterValue);
            ViewData["Color"] = new SelectList(colors,"Value","Text");
            return View( new ProductListViewModel()
            {
                ProductList = viewModel,
                SearchText = searchQuery.SearchText,
                PriceUpper = searchQuery.PriceUpper,
                PriceLower = searchQuery.PriceLower,
                Color = searchQuery.Color,
                Size = searchQuery.Size,
                
            });
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            
            return View(new ProductViewModel()
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Size = product.Size,
                Color = product.Color,
                Category = product.Category,
                CategoryId = product.CategoryId,
                ImageName = product.ImageName,
            });
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title");
            ViewData["Size"] = new SelectList(Enum.GetValues(typeof(Size)).Cast<Size>().Select(v => new SelectListItem
            {
                Text = v.ToString().Replace("_",""),
                Value = ((int)v).ToString()
            }).ToList(),"Value","Text");
            ViewData["Color"] = new SelectList(Enum.GetValues(typeof(Color)).Cast<Color>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList(),"Value","Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var product = new Product()
                {
                    Title = productViewModel.Title,
                    Description = productViewModel.Description,
                    Price = productViewModel.Price,
                    Size = productViewModel.Size,
                    Color = productViewModel.Color,
                    Category = productViewModel.Category,
                    CategoryId = productViewModel.CategoryId,
                    ImageName = productViewModel.ImageName,
                };
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", productViewModel.CategoryId);
            ViewData["Size"] = new SelectList(Enum.GetValues(typeof(Size)).Cast<Size>().Select(v => new SelectListItem
            {
                Text = v.ToString().Replace("_",""),
                Value = ((int)v).ToString()
            }).ToList(),"Value","Text", productViewModel.Size);
            ViewData["Color"] = new SelectList(Enum.GetValues(typeof(Color)).Cast<Color>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList(),"Value","Text", productViewModel.Color);
            return View(productViewModel);
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", product.CategoryId);
            ViewData["Size"] = new SelectList(Enum.GetValues(typeof(Size)).Cast<Size>().Select(v => new SelectListItem
            {
                Text = v.ToString().Replace("_",""),
                Value = ((int)v).ToString()
            }).ToList(),"Value","Text", product.Size);
            ViewData["Color"] = new SelectList(Enum.GetValues(typeof(Color)).Cast<Color>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList(),"Value","Text", product.Color);
            return View(new ProductViewModel()
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Size = product.Size,
                Color = product.Color,
                Category = product.Category,
                CategoryId = product.CategoryId,
                ImageName = product.ImageName,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products
                        .Include(p => p.Category)
                        .FirstOrDefaultAsync(m => m.Id == productViewModel.Id);
                    
                    product.Title = productViewModel.Title;
                    product.Description = productViewModel.Description;
                    product.Price = productViewModel.Price;
                    product.Size = productViewModel.Size;
                    product.Color = productViewModel.Color;
                    product.Category = productViewModel.Category;
                    product.CategoryId = productViewModel.CategoryId;
                    product.ImageName = productViewModel.ImageName;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(productViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", productViewModel.CategoryId);
            ViewData["Size"] = new SelectList(Enum.GetValues(typeof(Size)).Cast<Size>().Select(v => new SelectListItem
            {
                Text = v.ToString().Replace("_",""),
                Value = ((int)v).ToString()
            }).ToList(),"Value","Text", productViewModel.Size);
            ViewData["Color"] = new SelectList(Enum.GetValues(typeof(Color)).Cast<Color>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList(),"Value","Text", productViewModel.Color);
            return View(productViewModel);
        }

        
        public IActionResult OnPostMyUploader(IFormFile myUploader)
        {
            if (myUploader != null)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + myUploader.FileName;  
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    myUploader.CopyTo(fileStream);
                }
                return new ObjectResult(new { status = "success", uniqueFileName });
            }
            return new ObjectResult(new { status = "fail" });

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
