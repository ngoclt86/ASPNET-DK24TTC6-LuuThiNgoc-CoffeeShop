using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminCouponController : Controller
{
    private readonly ICouponService _couponService;

    public AdminCouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<IActionResult> Index()
    {
        var coupons = await _couponService.GetAllAsync();
        return View(coupons);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Coupon coupon)
    {
        if (!ModelState.IsValid) return View(coupon);
        await _couponService.CreateAsync(coupon);
        TempData["Success"] = "Thêm mã giảm giá thành công!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var coupon = await _couponService.GetByIdAsync(id);
        if (coupon == null) return NotFound();
        return View(coupon);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Coupon coupon)
    {
        if (id != coupon.Id) return NotFound();
        if (!ModelState.IsValid) return View(coupon);
        await _couponService.UpdateAsync(coupon);
        TempData["Success"] = "Cập nhật mã giảm giá thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _couponService.DeleteAsync(id);
        TempData["Success"] = "Xóa mã giảm giá thành công!";
        return RedirectToAction(nameof(Index));
    }
}
