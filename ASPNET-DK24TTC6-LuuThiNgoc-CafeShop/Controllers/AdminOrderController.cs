using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models.Enums;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminOrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IEmailService _emailService;

    public AdminOrderController(IOrderService orderService, IEmailService emailService)
    {
        _orderService = orderService;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index(string? search = null)
    {
        var orders = await _orderService.GetAllAsync();
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            orders = orders.Where(o => 
                o.Id.ToString() == search || 
                (!string.IsNullOrEmpty(o.User?.FullName) && o.User.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)) || 
                (!string.IsNullOrEmpty(o.User?.Email) && o.User.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }
        
        ViewData["CurrentSearch"] = search;
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        var orderBeforeUpdate = await _orderService.GetByIdAsync(id);
        var result = await _orderService.UpdateStatusWithInventoryAsync(id, status);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Cập nhật trạng thái đơn hàng thành công!"
            : result.Message;

        if (result.IsSuccess && orderBeforeUpdate != null && orderBeforeUpdate.Status != status)
        {
            var orderAfterUpdate = await _orderService.GetByIdAsync(id);
            if (orderAfterUpdate?.User?.Email is { Length: > 0 } email)
            {
                await _emailService.SendAsync(
                    email,
                    $"[CoffeeShop] Đơn hàng #{orderAfterUpdate.Id} cập nhật trạng thái",
                    $"<p>Xin chào {orderAfterUpdate.User?.FullName ?? "khách hàng"},</p>" +
                    $"<p>Đơn hàng <strong>#{orderAfterUpdate.Id}</strong> của bạn đã được cập nhật sang trạng thái: <strong>{GetStatusLabel(status)}</strong>.</p>" +
                    "<p>Cảm ơn bạn đã mua sắm tại CoffeeShop.</p>");
            }
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private static string GetStatusLabel(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Pending => "Chờ xử lý",
            OrderStatus.Processing => "Đang xử lý",
            OrderStatus.Completed => "Hoàn thành",
            OrderStatus.Cancelled => "Đã hủy",
            _ => status.ToString()
        };
    }
}
