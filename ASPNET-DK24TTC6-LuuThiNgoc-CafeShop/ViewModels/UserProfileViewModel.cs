using System.ComponentModel.DataAnnotations;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

public class UserProfileViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [StringLength(100)]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime CreatedAt { get; set; }

    // Statistics
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }

    // Recent orders
    public List<Order> RecentOrders { get; set; } = new();
}
