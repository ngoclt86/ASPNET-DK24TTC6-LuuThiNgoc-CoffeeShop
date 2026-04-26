using Microsoft.AspNetCore.Mvc;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Controllers;

public class CartController : Controller
{
    private const decimal ShippingFee = 0m;
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private readonly ICouponService _couponService;

    public CartController(ICartService cartService, IProductService productService, ICouponService couponService)
    {
        _cartService = cartService;
        _productService = productService;
        _couponService = couponService;
    }

    public async Task<IActionResult> Index()
    {
        var cart = _cartService.GetCart(HttpContext.Session);
        var subtotal = _cartService.GetCartTotal(HttpContext.Session);
        var discount = _cartService.GetCartDiscountAmount(HttpContext.Session);
        var (couponCode, discountPercent) = _cartService.GetAppliedCoupon(HttpContext.Session);
        var productStocks = new Dictionary<int, int>();
        foreach (var item in cart)
        {
            var product = await _productService.GetByIdAsync(item.ProductId);
            if (product != null && !productStocks.ContainsKey(product.Id))
            {
                productStocks[product.Id] = product.Stock;
            }
        }

        ViewBag.Subtotal = subtotal;
        ViewBag.DiscountAmount = discount;
        ViewBag.ShippingFee = ShippingFee;
        ViewBag.Total = subtotal - discount + ShippingFee;
        ViewBag.CouponCode = couponCode;
        ViewBag.CouponDiscountPercent = discountPercent;
        ViewBag.ProductStocks = productStocks;
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var product = await _productService.GetByIdAsync(productId);
        if (product == null) return NotFound();
        if (product.Stock <= 0)
        {
            TempData["Error"] = $"'{product.Name}' hiện đã hết hàng.";
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        var requestedQuantity = Math.Max(quantity, 1);
        var existingCartItem = _cartService.GetCart(HttpContext.Session).FirstOrDefault(c => c.ProductId == productId);
        var existingQuantity = existingCartItem?.Quantity ?? 0;
        if (existingQuantity + requestedQuantity > product.Stock)
        {
            var availableToAdd = Math.Max(product.Stock - existingQuantity, 0);
            TempData["Error"] = availableToAdd == 0
                ? $"Giỏ hàng đã đạt tồn kho tối đa cho '{product.Name}'."
                : $"Chỉ có thể thêm tối đa {availableToAdd} sản phẩm '{product.Name}'.";
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        var item = new CartItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            Quantity = requestedQuantity
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
    public async Task<IActionResult> Update(int productId, int quantity)
    {
        if (quantity <= 0)
        {
            _cartService.UpdateQuantity(HttpContext.Session, productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        var product = await _productService.GetByIdAsync(productId);
        if (product == null)
        {
            TempData["CartToastError"] = "Không tìm thấy sản phẩm để cập nhật giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        if (product.Stock <= 0)
        {
            TempData["CartToastError"] = $"'{product.Name}' hiện đã hết hàng, vui lòng xóa khỏi giỏ.";
            return RedirectToAction(nameof(Index));
        }

        if (quantity > product.Stock)
        {
            TempData["CartToastError"] = $"Số lượng '{product.Name}' vượt tồn kho. Tối đa hiện tại: {product.Stock}.";
            return RedirectToAction(nameof(Index));
        }

        _cartService.UpdateQuantity(HttpContext.Session, productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult GetCount()
    {
        var count = _cartService.GetCartCount(HttpContext.Session);
        return Json(new { count });
    }

    [HttpPost]
    public async Task<IActionResult> ValidateStockBeforeCheckout()
    {
        var cart = _cartService.GetCart(HttpContext.Session);
        if (!cart.Any())
        {
            return Json(new
            {
                isValid = false,
                hasAdjusted = false,
                cartIsEmpty = true,
                issues = new[]
                {
                    new
                    {
                        productId = 0,
                        productName = "Giỏ hàng",
                        requested = 0,
                        available = 0,
                        message = "Giỏ hàng đang trống."
                    }
                },
                adjustments = Array.Empty<object>()
            });
        }

        var issues = await BuildStockIssuesAsync(cart);
        if (!issues.Any())
        {
            return Json(new
            {
                isValid = true,
                hasAdjusted = false,
                cartIsEmpty = false,
                issues = Array.Empty<object>(),
                adjustments = Array.Empty<object>()
            });
        }

        var adjustments = await AdjustCartToAvailableStockAsync(cart);
        var updatedCart = _cartService.GetCart(HttpContext.Session);
        var remainingIssues = await BuildStockIssuesAsync(updatedCart);

        return Json(new
        {
            isValid = remainingIssues.Count == 0 && updatedCart.Any(),
            hasAdjusted = adjustments.Count > 0,
            cartIsEmpty = !updatedCart.Any(),
            issues,
            adjustments
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyCoupon(string couponCode)
    {
        if (!_cartService.GetCart(HttpContext.Session).Any())
        {
            return Json(new { success = false, message = "Giỏ hàng trống, không thể áp dụng mã giảm giá." });
        }

        if (string.IsNullOrWhiteSpace(couponCode))
        {
            return Json(new { success = false, message = "Vui lòng nhập mã giảm giá." });
        }

        var (isValid, message, discountPercent) = await _couponService.ValidateAsync(couponCode.Trim());
        if (!isValid)
        {
            return Json(new { success = false, message });
        }

        _cartService.ApplyCoupon(HttpContext.Session, couponCode, discountPercent);
        return Json(new { success = true, message = $"Áp dụng mã thành công: giảm {discountPercent}%." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveCoupon()
    {
        _cartService.RemoveCoupon(HttpContext.Session);
        return Json(new { success = true, message = "Đã bỏ mã giảm giá." });
    }

    private async Task<List<object>> BuildStockIssuesAsync(List<CartItem> cart)
    {
        var issues = new List<object>();
        var productMap = new Dictionary<int, Product>();
        foreach (var item in cart)
        {
            if (productMap.ContainsKey(item.ProductId))
            {
                continue;
            }

            var product = await _productService.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                productMap[item.ProductId] = product;
            }
        }

        foreach (var item in cart)
        {
            if (!productMap.TryGetValue(item.ProductId, out var product))
            {
                issues.Add(new
                {
                    productId = item.ProductId,
                    productName = item.ProductName,
                    requested = item.Quantity,
                    available = 0,
                    message = $"Sản phẩm \"{item.ProductName}\" không còn tồn tại."
                });
                continue;
            }

            if (product.Stock < item.Quantity)
            {
                issues.Add(new
                {
                    productId = item.ProductId,
                    productName = product.Name,
                    requested = item.Quantity,
                    available = product.Stock,
                    message = $"Sản phẩm \"{product.Name}\" yêu cầu {item.Quantity}, hiện chỉ còn {product.Stock}."
                });
            }
        }

        return issues;
    }

    private async Task<List<object>> AdjustCartToAvailableStockAsync(List<CartItem> cart)
    {
        var adjustments = new List<object>();
        var processedProductIds = new HashSet<int>();

        foreach (var item in cart)
        {
            if (!processedProductIds.Add(item.ProductId))
            {
                continue;
            }

            var product = await _productService.GetByIdAsync(item.ProductId);
            if (product == null || product.Stock <= 0)
            {
                _cartService.RemoveFromCart(HttpContext.Session, item.ProductId);
                adjustments.Add(new
                {
                    productId = item.ProductId,
                    productName = item.ProductName,
                    action = "removed",
                    newQuantity = 0
                });
                continue;
            }

            if (item.Quantity > product.Stock)
            {
                _cartService.UpdateQuantity(HttpContext.Session, item.ProductId, product.Stock);
                adjustments.Add(new
                {
                    productId = item.ProductId,
                    productName = product.Name,
                    action = "reduced",
                    newQuantity = product.Stock
                });
            }
        }

        return adjustments;
    }
}
