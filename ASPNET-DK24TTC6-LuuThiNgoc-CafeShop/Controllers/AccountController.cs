using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOrderService _orderService;
    private readonly IEmailService _emailService;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOrderService orderService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _orderService = orderService;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        if (user.IsLocked)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa.");
            return View(model);
        }

        if (!user.EmailConfirmed)
        {
            ModelState.AddModelError(string.Empty, "Vui lòng xác thực email trước khi đăng nhập.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("Index", "AdminDashboard", new { area = "Admin" });
            }
            return RedirectToLocal(returnUrl);
        }

        ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Address = model.Address,
            EmailConfirmed = false,
            CreatedAt = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await SendEmailConfirmationAsync(user);
            TempData["Success"] = "Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản.";
            return RedirectToAction(nameof(Login));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        {
            TempData["Error"] = "Liên kết xác thực không hợp lệ.";
            return RedirectToAction(nameof(Login));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["Error"] = "Không tìm thấy tài khoản cần xác thực.";
            return RedirectToAction(nameof(Login));
        }

        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            TempData["Error"] = "Liên kết xác thực không hợp lệ.";
            return RedirectToAction(nameof(Login));
        }

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "Xác thực email thành công. Bạn có thể đăng nhập ngay bây giờ."
            : "Không thể xác thực email. Liên kết có thể đã hết hạn.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && user.EmailConfirmed)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var callbackUrl = Url.Action(
                nameof(ResetPassword),
                "Account",
                new { email = user.Email, token = encodedToken },
                Request.Scheme);

            if (!string.IsNullOrWhiteSpace(callbackUrl))
            {
                await _emailService.SendAsync(
                    user.Email!,
                    "Đặt lại mật khẩu CoffeeShop",
                    $"<p>Xin chào {user.FullName},</p><p>Bạn vừa yêu cầu đặt lại mật khẩu. Nhấn vào liên kết sau:</p><p><a href=\"{callbackUrl}\">Đặt lại mật khẩu</a></p><p>Nếu không phải bạn yêu cầu, hãy bỏ qua email này.</p>");
            }
        }

        TempData["Success"] = "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi liên kết đặt lại mật khẩu.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ResetPassword(string? email = null, string? token = null)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            TempData["Error"] = "Liên kết đặt lại mật khẩu không hợp lệ.";
            return RedirectToAction(nameof(Login));
        }

        return View(new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            TempData["Success"] = "Đặt lại mật khẩu thành công. Bạn có thể đăng nhập lại.";
            return RedirectToAction(nameof(Login));
        }

        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
        }
        catch (FormatException)
        {
            ModelState.AddModelError(string.Empty, "Liên kết đặt lại mật khẩu không hợp lệ.");
            return View(model);
        }

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
        if (result.Succeeded)
        {
            TempData["Success"] = "Đặt lại mật khẩu thành công. Bạn có thể đăng nhập lại.";
            return RedirectToAction(nameof(Login));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    // ===== Profile =====

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction(nameof(Login));

        var orders = await _orderService.GetByUserIdAsync(user.Id);

        var model = new UserProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt,
            TotalOrders = orders.Count,
            TotalSpent = orders.Sum(o => o.TotalAmount),
            RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5).ToList()
        };

        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction(nameof(Login));

        var model = new UserProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(UserProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction(nameof(Login));

        user.FullName = model.FullName;
        user.Address = model.Address;
        user.PhoneNumber = model.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            TempData["Success"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction(nameof(Profile));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction(nameof(Login));

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction(nameof(Profile));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    private async Task SendEmailConfirmationAsync(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var callbackUrl = Url.Action(
            nameof(ConfirmEmail),
            "Account",
            new { userId = user.Id, token = encodedToken },
            Request.Scheme);

        if (string.IsNullOrWhiteSpace(callbackUrl) || string.IsNullOrWhiteSpace(user.Email))
        {
            return;
        }

        await _emailService.SendAsync(
            user.Email,
            "Xác thực email CoffeeShop",
            $"<p>Xin chào {user.FullName},</p><p>Cảm ơn bạn đã đăng ký tài khoản. Nhấn liên kết dưới đây để xác thực email:</p><p><a href=\"{callbackUrl}\">Xác thực email</a></p>");
    }
}
