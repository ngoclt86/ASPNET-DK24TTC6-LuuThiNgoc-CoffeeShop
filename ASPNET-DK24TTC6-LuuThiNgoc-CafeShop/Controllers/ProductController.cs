using Microsoft.AspNetCore.Mvc;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(int? categoryId, string? search)
    {
        var categories = await _categoryService.GetAllAsync();
        ViewBag.Categories = categories;
        ViewBag.CurrentCategory = categoryId;
        ViewBag.CurrentSearch = search;

        var products = categoryId.HasValue
            ? await _productService.GetByCategoryAsync(categoryId.Value)
            : !string.IsNullOrWhiteSpace(search)
                ? await _productService.SearchAsync(search)
                : await _productService.GetActiveAsync();

        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null || !product.IsActive) return NotFound();

        // Related products in same category
        var related = await _productService.GetByCategoryAsync(product.CategoryId);
        ViewBag.RelatedProducts = related.Where(p => p.Id != id).Take(4).ToList();

        return View(product);
    }
}
