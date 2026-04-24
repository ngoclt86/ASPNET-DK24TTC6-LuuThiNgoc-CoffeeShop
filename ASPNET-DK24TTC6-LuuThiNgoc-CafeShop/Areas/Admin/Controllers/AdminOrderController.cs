using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models.Enums;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminOrderController : Controller
{
    private readonly IOrderService _orderService;

    public AdminOrderController(IOrderService orderService)
    {
        _orderService = orderService;
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
        var result = await _orderService.UpdateStatusWithInventoryAsync(id, status);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Cập nhật trạng thái đơn hàng thành công!"
            : result.Message;

        return RedirectToAction(nameof(Details), new { id });
    }
}
