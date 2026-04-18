using System.ComponentModel.DataAnnotations;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

public class Coupon
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
    [StringLength(50)]
    [Display(Name = "Mã giảm giá")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập phần trăm giảm")]
    [Range(1, 100, ErrorMessage = "Phần trăm giảm từ 1-100")]
    [Display(Name = "Phần trăm giảm (%)")]
    public int DiscountPercent { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày hết hạn")]
    [Display(Name = "Ngày hết hạn")]
    [DataType(DataType.Date)]
    public DateTime ExpiryDate { get; set; }

    [Display(Name = "Số lần sử dụng tối đa")]
    [Range(1, int.MaxValue)]
    public int MaxUsage { get; set; } = 100;

    [Display(Name = "Đã sử dụng")]
    public int CurrentUsage { get; set; } = 0;

    [Display(Name = "Hoạt động")]
    public bool IsActive { get; set; } = true;
}
