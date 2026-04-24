using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminUserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminUserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
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
        var userRows = new List<AdminUserItemViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRows.Add(new AdminUserItemViewModel
            {
                User = user,
                CurrentRole = roles.FirstOrDefault() ?? "User"
            });
        }

        var availableRoles = await _roleManager.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => r.Name!)
            .ToListAsync();

        return View(new AdminUserViewModel
        {
            Users = userRows,
            Search = search,
            AvailableRoles = availableRoles
        });
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(string id, string role)
    {
        if (string.IsNullOrWhiteSpace(role) || !await _roleManager.RoleExistsAsync(role))
        {
            TempData["Error"] = "Vai trò không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null && currentUser.Id == user.Id && !string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Bạn không thể tự gỡ quyền Admin của chính mình.";
            return RedirectToAction(nameof(Index));
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                TempData["Error"] = "Không thể cập nhật quyền người dùng.";
                return RedirectToAction(nameof(Index));
            }
        }

        var addResult = await _userManager.AddToRoleAsync(user, role);
        TempData[addResult.Succeeded ? "Success" : "Error"] = addResult.Succeeded
            ? $"Đã cập nhật quyền {role} cho {user.Email}."
            : "Không thể cập nhật quyền người dùng.";
        return RedirectToAction(nameof(Index));
    }
}
