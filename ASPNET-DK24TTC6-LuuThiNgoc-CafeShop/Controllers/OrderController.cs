using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models.Enums;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;
    private readonly IProductService _productService;
    private readonly IVnPayService _vnPayService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IOrderService orderService,
        ICartService cartService,
        ICouponService couponService,
        IProductService productService,
        IVnPayService vnPayService,
        UserManager<ApplicationUser> userManager,
        ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _cartService = cartService;
        _couponService = couponService;
        _productService = productService;
        _vnPayService = vnPayService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = _cartService.GetCart(HttpContext.Session);
        if (!cart.Any())
        {
            TempData["Error"] = "Giỏ hàng trống!";
            return RedirectToAction("Index", "Cart");
        }

        var subtotal = cart.Sum(c => c.Total);
        var (couponCode, discountPercent) = _cartService.GetAppliedCoupon(HttpContext.Session);
        var discountAmount = _cartService.GetCartDiscountAmount(HttpContext.Session);
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var model = new CheckoutViewModel
        {
            ShippingAddress = user.Address ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            CartItems = cart,
            SubTotal = subtotal,
            DiscountAmount = discountAmount,
            Total = subtotal - discountAmount,
            AppliedCouponCode = couponCode,
            AppliedDiscountPercent = discountPercent
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
        model.DiscountAmount = 0;
        model.Total = model.SubTotal;

        var (appliedCouponCode, appliedDiscountPercent) = _cartService.GetAppliedCoupon(HttpContext.Session);
        model.AppliedCouponCode = appliedCouponCode;
        model.AppliedDiscountPercent = appliedDiscountPercent;

        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        decimal discountAmount = 0;
        string? couponCodeForOrder = null;
        if (!string.IsNullOrWhiteSpace(appliedCouponCode))
        {
            var (isValid, message, discountPercent) = await _couponService.ValidateAsync(appliedCouponCode);
            if (!isValid)
            {
                _cartService.RemoveCoupon(HttpContext.Session);
                model.AppliedCouponCode = null;
                model.AppliedDiscountPercent = 0;
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }
            discountAmount = model.SubTotal * discountPercent / 100;
            couponCodeForOrder = appliedCouponCode;
        }

        model.DiscountAmount = discountAmount;
        model.Total = model.SubTotal - discountAmount;

        var stockIssues = await BuildStockIssuesAsync(cart);
        if (stockIssues.Any())
        {
            var adjustments = AdjustCartToAvailableStockAsync(stockIssues);
            var updatedCart = _cartService.GetCart(HttpContext.Session);
            TempData["CheckoutStockAdjustedMessage"] = "Giỏ hàng đã được tự động cập nhật theo tồn kho hiện tại.";
            TempData["CheckoutStockAdjustedIssuesJson"] = JsonSerializer.Serialize(stockIssues.Select(issue => new
            {
                productName = issue.ProductName,
                requested = issue.Requested,
                available = issue.Available
            }));

            if (!updatedCart.Any())
            {
                TempData["Error"] = "Một số sản phẩm đã hết hàng và bị xóa khỏi giỏ. Giỏ hàng hiện đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            if (!adjustments.Any())
            {
                foreach (var issue in stockIssues)
                {
                    ModelState.AddModelError(string.Empty, $"{issue.ProductName}: yêu cầu {issue.Requested}, còn {issue.Available}.");
                }

                return View(model);
            }

            return RedirectToAction(nameof(Checkout));
        }

        var order = new Order
        {
            UserId = user.Id,
            ShippingAddress = model.ShippingAddress,
            PhoneNumber = model.PhoneNumber,
            Notes = model.Notes,
            CouponCode = couponCodeForOrder,
            DiscountAmount = discountAmount,
            TotalAmount = model.SubTotal - discountAmount,
            PaymentMethod = model.PaymentMethod,
            OrderDetails = cart.Select(c => new OrderDetail
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                UnitPrice = c.Price
            }).ToList()
        };

        await _orderService.CreateAsync(order);

        if (model.PaymentMethod == PaymentMethod.VnPay)
        {
            try
            {
                var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, order.Id, order.TotalAmount, $"Thanh toan don hang {order.Id}");
                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                await _orderService.UpdateStatusAsync(order.Id, OrderStatus.Cancelled);
                ModelState.AddModelError(string.Empty, $"Không thể khởi tạo thanh toán VNPAY: {ex.Message}");
                return View(model);
            }
        }

        var codStockResult = await _orderService.DeductStockForOrderAsync(order.Id);
        if (!codStockResult.IsSuccess)
        {
            await _orderService.UpdateStatusAsync(order.Id, OrderStatus.Cancelled);
            ModelState.AddModelError(string.Empty, codStockResult.Message);
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(couponCodeForOrder))
        {
            await _couponService.IncrementUsageAsync(couponCodeForOrder);
        }

        _cartService.ClearCart(HttpContext.Session);
        TempData["Success"] = "Đặt hàng thành công!";
        return RedirectToAction(nameof(OrderSuccess), new { id = order.Id });
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> VnPayReturn()
    {
        var result = _vnPayService.ProcessReturnResponse(Request.Query);
        if (!result.IsValidSignature)
        {
            _logger.LogWarning(
                "VNPAY signature invalid. TxnRef={TxnRef}, ResponseCode={ResponseCode}, ProvidedHash={ProvidedHash}, ExpectedHash={ExpectedHash}, HashData={HashData}",
                result.OrderId, result.ResponseCode, result.ProvidedHash, result.ExpectedHash, result.HashData);
            TempData["Error"] = "Chữ ký VNPAY không hợp lệ.";
            return RedirectToAction("Index", "Cart");
        }

        if (!int.TryParse(result.OrderId, out var orderId))
        {
            TempData["Error"] = "Không xác định được đơn hàng thanh toán.";
            return RedirectToAction("Index", "Cart");
        }

        var order = await _orderService.GetByIdAsync(orderId);
        if (order == null)
        {
            TempData["Error"] = "Không tìm thấy đơn hàng cần cập nhật thanh toán.";
            return RedirectToAction("Index", "Cart");
        }

        if (order.PaymentMethod != PaymentMethod.VnPay)
        {
            TempData["Error"] = "Đơn hàng không sử dụng phương thức VNPAY.";
            return RedirectToAction(nameof(Details), new { id = order.Id });
        }

        if (order.Status != OrderStatus.Pending)
        {
            return RedirectToAction(nameof(OrderSuccess), new { id = order.Id });
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "VNPAY payment success. OrderId={OrderId}, TransactionNo={TransactionNo}",
                order.Id, result.TransactionNo);

            var vnPayStockResult = await _orderService.DeductStockForOrderAsync(order.Id);
            if (!vnPayStockResult.IsSuccess)
            {
                await _orderService.UpdateStatusAsync(order.Id, OrderStatus.Cancelled);
                _logger.LogWarning(
                    "VNPAY paid but stock update failed. OrderId={OrderId}, Reason={Reason}",
                    order.Id, vnPayStockResult.Message);
                TempData["Error"] = vnPayStockResult.Message;
                return RedirectToAction(nameof(Checkout));
            }

            await _orderService.UpdateStatusAsync(order.Id, OrderStatus.Processing);
            if (!string.IsNullOrWhiteSpace(order.CouponCode))
            {
                await _couponService.IncrementUsageAsync(order.CouponCode);
            }

            _cartService.ClearCart(HttpContext.Session);
            TempData["Success"] = "Thanh toán VNPAY thành công.";
            return RedirectToAction(nameof(OrderSuccess), new { id = order.Id });
        }

        await _orderService.UpdateStatusAsync(order.Id, OrderStatus.Cancelled);
        _logger.LogWarning(
            "VNPAY payment failed. OrderId={OrderId}, ResponseCode={ResponseCode}, TransactionNo={TransactionNo}",
            order.Id, result.ResponseCode, result.TransactionNo);
        TempData["Error"] = $"Thanh toán VNPAY thất bại (mã: {result.ResponseCode}).";
        return RedirectToAction(nameof(Checkout));
    }

    public async Task<IActionResult> OrderSuccess(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null) return NotFound();
        return View(order);
    }

    public async Task<IActionResult> History(int page = 1)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        const int pageSize = 6;
        var allOrders = await _orderService.GetByUserIdAsync(user.Id);
        var sorted = allOrders.OrderByDescending(o => o.OrderDate).ToList();
        var totalOrders = sorted.Count;
        var totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);
        page = Math.Clamp(page, 1, Math.Max(totalPages, 1));

        var model = new OrderHistoryViewModel
        {
            Orders = sorted.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
            CurrentPage = page,
            TotalPages = totalPages,
            TotalOrders = totalOrders,
            PageSize = pageSize
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var order = await _orderService.GetByIdAsync(id);
        if (order == null || order.UserId != user.Id) return NotFound();
        return View(order);
    }

    private async Task<List<StockIssue>> BuildStockIssuesAsync(List<CartItem> cart)
    {
        var issues = new List<StockIssue>();
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
                issues.Add(new StockIssue(item.ProductId, item.ProductName, item.Quantity, 0));
                continue;
            }

            if (product.Stock < item.Quantity)
            {
                issues.Add(new StockIssue(item.ProductId, product.Name, item.Quantity, product.Stock));
            }
        }

        return issues;
    }

    private List<StockAdjustment> AdjustCartToAvailableStockAsync(List<StockIssue> stockIssues)
    {
        var adjustments = new List<StockAdjustment>();
        foreach (var issue in stockIssues)
        {
            if (issue.Available <= 0)
            {
                _cartService.RemoveFromCart(HttpContext.Session, issue.ProductId);
                adjustments.Add(new StockAdjustment(issue.ProductId, issue.ProductName, 0, "removed"));
                continue;
            }

            _cartService.UpdateQuantity(HttpContext.Session, issue.ProductId, issue.Available);
            adjustments.Add(new StockAdjustment(issue.ProductId, issue.ProductName, issue.Available, "reduced"));
        }

        return adjustments;
    }

    private sealed record StockIssue(int ProductId, string ProductName, int Requested, int Available);
    private sealed record StockAdjustment(int ProductId, string ProductName, int NewQuantity, string Action);
}
