using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

public class ProductIndexViewModel
{
    public List<Product> Items { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public int? CurrentCategory { get; set; }
    public string? CurrentSearch { get; set; }
    public string? CurrentSort { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 6;
    public int TotalItems { get; set; }
    public int TotalPages => TotalItems == 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
}
