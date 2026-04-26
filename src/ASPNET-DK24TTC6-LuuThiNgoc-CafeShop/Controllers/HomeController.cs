using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ICouponService _couponService;

    public HomeController(ILogger<HomeController> logger, IProductService productService, ICategoryService categoryService, ICouponService couponService)
    {
        _logger = logger;
        _productService = productService;
        _categoryService = categoryService;
        _couponService = couponService;
    }

    public async Task<IActionResult> Index()
    {
        var featuredProducts = await _productService.GetFeaturedAsync(8);
        var categories = await _categoryService.GetAllAsync();
        ViewBag.Categories = categories;
        return View(featuredProducts);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Terms()
    {
        return View();
    }

    public IActionResult Shipping()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public async Task<IActionResult> Promotions()
    {
        var coupons = await _couponService.GetAllAsync();
        var availableCoupons = coupons
            .Where(c => !c.IsDeleted && c.IsActive && c.ExpiryDate >= DateTime.Now)
            .OrderBy(c => c.ExpiryDate)
            .ToList();

        return View(availableCoupons);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}