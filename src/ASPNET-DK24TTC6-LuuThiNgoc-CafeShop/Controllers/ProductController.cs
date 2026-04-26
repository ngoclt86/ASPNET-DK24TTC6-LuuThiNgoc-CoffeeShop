using Microsoft.AspNetCore.Mvc;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

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

    public async Task<IActionResult> Index(int? categoryId, string? search, string? sort, int page = 1)
    {
        var categories = await _categoryService.GetAllAsync();
        const int pageSize = 6;

        var (products, totalItems) = await _productService.GetActivePagedAsync(
            categoryId,
            search,
            sort,
            page,
            pageSize);

        var viewModel = new ProductIndexViewModel
        {
            Items = products,
            Categories = categories,
            CurrentCategory = categoryId,
            CurrentSearch = search,
            CurrentSort = sort,
            CurrentPage = page < 1 ? 1 : page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        if (viewModel.TotalPages > 0 && viewModel.CurrentPage > viewModel.TotalPages)
        {
            return RedirectToAction(nameof(Index), new
            {
                categoryId,
                search,
                sort,
                page = viewModel.TotalPages
            });
        }

        return View(viewModel);
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
