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
    private readonly IPaymentTransactionService _paymentTransactionService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IOrderService orderService,
        ICartService cartService,
        ICouponService couponService,
        IProductService productService,
        IVnPayService vnPayService,
        IPaymentTransactionService paymentTransactionService,
        UserManager<ApplicationUser> userManager,
        ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _cartService = cartService;
        _couponService = couponService;
        _productService = productService;
        _vnPayService = vnPayService;
        _paymentTransactionService = paymentTransactionService;
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

        var codStatusResult = await _orderService.UpdateStatusWithInventoryAsync(order.Id, OrderStatus.Processing);
        if (!codStatusResult.IsSuccess)
        {
            await _orderService.UpdateStatusWithInventoryAsync(order.Id, OrderStatus.Cancelled);
            ModelState.AddModelError(string.Empty, codStatusResult.Message);
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
        return await HandleVnPayCallbackAsync(fromIpn: false);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> VnPayIpn()
    {
        return await HandleVnPayCallbackAsync(fromIpn: true);
    }

    public async Task<IActionResult> OrderSuccess(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null) return NotFound();
        if (User.Identity?.IsAuthenticated == true && !User.IsInRole("Admin"))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || order.UserId != user.Id)
            {
                return Forbid();
            }
        }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var order = await _orderService.GetByIdAsync(id);
        if (order == null || order.UserId != user.Id)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Pending)
        {
            TempData["Error"] = "Chỉ có thể hủy đơn ở trạng thái chờ xử lý.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var cancelResult = await _orderService.UpdateStatusWithInventoryAsync(order.Id, OrderStatus.Cancelled);
        TempData[cancelResult.IsSuccess ? "Success" : "Error"] = cancelResult.IsSuccess
            ? "Đơn hàng đã được hủy thành công."
            : cancelResult.Message;
        return RedirectToAction(nameof(History));
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

    private async Task<IActionResult> HandleVnPayCallbackAsync(bool fromIpn)
    {
        var result = _vnPayService.ProcessReturnResponse(Request.Query);

        if (!result.IsValidSignature)
        {
            _logger.LogWarning(
                "VNPAY signature invalid. TxnRef={TxnRef}, ResponseCode={ResponseCode}, ProvidedHash={ProvidedHash}, ExpectedHash={ExpectedHash}",
                result.OrderId, result.ResponseCode, result.ProvidedHash, result.ExpectedHash);
            return fromIpn
                ? Json(new { RspCode = "97", Message = "Invalid signature" })
                : RedirectWithError("Chữ ký VNPAY không hợp lệ.", "Index", "Cart");
        }

        if (!int.TryParse(result.OrderId, out var orderId))
        {
            return fromIpn
                ? Json(new { RspCode = "01", Message = "Order not found" })
                : RedirectWithError("Không xác định được đơn hàng thanh toán.", "Index", "Cart");
        }

        var order = await _orderService.GetByIdAsync(orderId);
        if (order == null)
        {
            return fromIpn
                ? Json(new { RspCode = "01", Message = "Order not found" })
                : RedirectWithError("Không tìm thấy đơn hàng cần cập nhật thanh toán.", "Index", "Cart");
        }

        await _paymentTransactionService.CreateAsync(new PaymentTransaction
        {
            OrderId = order.Id,
            Provider = "VNPAY",
            TxnRef = result.OrderId,
            TransactionNo = result.TransactionNo,
            ResponseCode = result.ResponseCode,
            Amount = order.TotalAmount,
            Status = result.IsSuccess ? PaymentTransactionStatus.Success : PaymentTransactionStatus.Failed,
            Message = result.IsSuccess ? "Payment success callback" : "Payment failed callback",
            RawQuery = Request.QueryString.HasValue ? Request.QueryString.Value : null
        });

        if (order.PaymentMethod != PaymentMethod.VnPay)
        {
            return fromIpn
                ? Json(new { RspCode = "02", Message = "Invalid payment method" })
                : RedirectWithError("Đơn hàng không sử dụng phương thức VNPAY.", nameof(Details), null, order.Id);
        }

        if (order.Status != OrderStatus.Pending)
        {
            return fromIpn
                ? Json(new { RspCode = "00", Message = "Order already processed" })
                : RedirectToAction(nameof(OrderSuccess), new { id = order.Id });
        }

        if (result.IsSuccess)
        {
            var statusResult = await _orderService.UpdateStatusWithInventoryAsync(order.Id, OrderStatus.Processing);
            if (!statusResult.IsSuccess)
            {
                await _orderService.UpdateStatusWithInventoryAsync(order.Id, OrderStatus.Cancelled);
                _logger.LogWarning("VNPAY paid but stock update failed. OrderId={OrderId}, Reason={Reason}", order.Id, statusResult.Message);
                return fromIpn
                    ? Json(new { RspCode = "99", Message = statusResult.Message })
                    : RedirectWithError(statusResult.Message, nameof(Checkout), null);
            }

            if (!string.IsNullOrWhiteSpace(order.CouponCode))
            {
                await _couponService.IncrementUsageAsync(order.CouponCode);
            }

            _cartService.ClearCart(HttpContext.Session);
            return fromIpn
                ? Json(new { RspCode = "00", Message = "Confirm Success" })
                : RedirectWithSuccess("Thanh toán VNPAY thành công.", nameof(OrderSuccess), null, order.Id);
        }

        await _orderService.UpdateStatusWithInventoryAsync(order.Id, OrderStatus.Cancelled);
        _logger.LogWarning(
            "VNPAY payment failed. OrderId={OrderId}, ResponseCode={ResponseCode}, TransactionNo={TransactionNo}",
            order.Id, result.ResponseCode, result.TransactionNo);

        return fromIpn
            ? Json(new { RspCode = "00", Message = "Confirm Success" })
            : RedirectWithError($"Thanh toán VNPAY thất bại (mã: {result.ResponseCode}).", nameof(Checkout), null);
    }

    private IActionResult RedirectWithError(string message, string action, string? controller = null, int? id = null)
    {
        TempData["Error"] = message;
        return RedirectToAction(action, controller, id.HasValue ? new { id } : null);
    }

    private IActionResult RedirectWithSuccess(string message, string action, string? controller = null, int? id = null)
    {
        TempData["Success"] = message;
        return RedirectToAction(action, controller, id.HasValue ? new { id } : null);
    }
}
