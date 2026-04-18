using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderController(IOrderService orderService, ICartService cartService, ICouponService couponService, UserManager<ApplicationUser> userManager)
    {
        _orderService = orderService;
        _cartService = cartService;
        _couponService = couponService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        var cart = _cartService.GetCart(HttpContext.Session);
        if (!cart.Any())
        {
            TempData["Error"] = "Giỏ hàng trống!";
            return RedirectToAction("Index", "Cart");
        }

        var model = new CheckoutViewModel
        {
            CartItems = cart,
            SubTotal = cart.Sum(c => c.Total),
            Total = cart.Sum(c => c.Total)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var cart = _cartService.GetCart(HttpContext.Session);
        if (!cart.Any())
        {
            TempData["Error"] = "Giỏ hàng trống!";
            return RedirectToAction("Index", "Cart");
        }

        model.CartItems = cart;
        model.SubTotal = cart.Sum(c => c.Total);
        model.Total = model.SubTotal;

        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        decimal discountAmount = 0;
        if (!string.IsNullOrWhiteSpace(model.CouponCode))
        {
            var (isValid, message, discountPercent) = await _couponService.ValidateAsync(model.CouponCode);
            if (!isValid)
            {
                ModelState.AddModelError("CouponCode", message);
                return View(model);
            }
            discountAmount = model.SubTotal * discountPercent / 100;
        }

        var order = new Order
        {
            UserId = user.Id,
            ShippingAddress = model.ShippingAddress,
            PhoneNumber = model.PhoneNumber,
            Notes = model.Notes,
            CouponCode = model.CouponCode,
            DiscountAmount = discountAmount,
            TotalAmount = model.SubTotal - discountAmount,
            OrderDetails = cart.Select(c => new OrderDetail
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                UnitPrice = c.Price
            }).ToList()
        };

        await _orderService.CreateAsync(order);

        if (!string.IsNullOrWhiteSpace(model.CouponCode))
        {
            await _couponService.IncrementUsageAsync(model.CouponCode);
        }

        _cartService.ClearCart(HttpContext.Session);
        TempData["Success"] = "Đặt hàng thành công!";
        return RedirectToAction(nameof(OrderSuccess), new { id = order.Id });
    }

    public async Task<IActionResult> OrderSuccess(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null) return NotFound();
        return View(order);
    }

    public async Task<IActionResult> History()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var orders = await _orderService.GetByUserIdAsync(user.Id);
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var order = await _orderService.GetByIdAsync(id);
        if (order == null || order.UserId != user.Id) return NotFound();
        return View(order);
    }
}
