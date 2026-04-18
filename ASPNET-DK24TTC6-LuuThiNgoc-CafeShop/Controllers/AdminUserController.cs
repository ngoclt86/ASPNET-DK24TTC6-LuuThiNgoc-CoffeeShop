using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminUserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminUserController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.IsLocked = !user.IsLocked;
        await _userManager.UpdateAsync(user);

        TempData["Success"] = user.IsLocked
            ? $"Đã khóa tài khoản {user.Email}"
            : $"Đã mở khóa tài khoản {user.Email}";

        return RedirectToAction(nameof(Index));
    }
}
