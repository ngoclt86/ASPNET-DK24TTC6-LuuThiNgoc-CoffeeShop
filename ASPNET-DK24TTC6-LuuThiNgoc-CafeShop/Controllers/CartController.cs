using Microsoft.AspNetCore.Mvc;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IProductService _productService;

    public CartController(ICartService cartService, IProductService productService)
    {
        _cartService = cartService;
        _productService = productService;
    }

    public IActionResult Index()
    {
        var cart = _cartService.GetCart(HttpContext.Session);
        ViewBag.Total = _cartService.GetCartTotal(HttpContext.Session);
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var product = await _productService.GetByIdAsync(productId);
        if (product == null) return NotFound();

        var item = new CartItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            Quantity = quantity
        };

        _cartService.AddToCart(HttpContext.Session, item);
        TempData["Success"] = $"Đã thêm '{product.Name}' vào giỏ hàng!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Remove(int productId)
    {
        _cartService.RemoveFromCart(HttpContext.Session, productId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Update(int productId, int quantity)
    {
        _cartService.UpdateQuantity(HttpContext.Session, productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult GetCount()
    {
        var count = _cartService.GetCartCount(HttpContext.Session);
        return Json(new { count });
    }
}
