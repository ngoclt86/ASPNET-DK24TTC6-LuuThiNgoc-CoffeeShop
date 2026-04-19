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

    public async Task<IActionResult> Index(string? search = null)
    {
        var query = _userManager.Users.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => (!string.IsNullOrEmpty(u.Email) && u.Email.Contains(search)) || 
                                     (!string.IsNullOrEmpty(u.FullName) && u.FullName.Contains(search)) || 
                                     (!string.IsNullOrEmpty(u.PhoneNumber) && u.PhoneNumber.Contains(search)));
        }
        
        var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
        ViewData["CurrentSearch"] = search;
        
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
