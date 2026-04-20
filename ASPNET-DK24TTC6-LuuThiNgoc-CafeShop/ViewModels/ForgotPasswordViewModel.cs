using System.ComponentModel.DataAnnotations;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string Email { get; set; } = string.Empty;
}
