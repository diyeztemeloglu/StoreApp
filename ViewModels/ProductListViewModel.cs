using System.Collections.Generic;

namespace StoreApp.ViewModels
{
    public class ProductListViewModel : ProductFilters
    {
        public List<ProductViewModel> ProductList { get; set; } = new();
    }

    public class ProductFilters
    {
        public decimal PriceLower { get; set; }
        public decimal PriceUpper{ get; set; }
        public int Size{ get; set; }
        public int Color{ get; set; }
        public int CategoryId{ get; set; }
        public string SearchText{ get; set; }
    }

}