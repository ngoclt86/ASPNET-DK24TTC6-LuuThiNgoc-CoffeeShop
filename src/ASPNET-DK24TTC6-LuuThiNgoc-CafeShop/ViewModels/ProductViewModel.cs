using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    [StringLength(200)]
    [Display(Name = "Tên sản phẩm")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập giá")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
    [Display(Name = "Giá (VNĐ)")]
    public decimal Price { get; set; }

    [Display(Name = "Hình ảnh")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "URL hình ảnh")]
    [StringLength(1000)]
    [Url(ErrorMessage = "URL hình ảnh không hợp lệ")]
    public string? ImageUrlInput { get; set; }

    public string? ExistingImageUrl { get; set; }

    [Display(Name = "Tồn kho")]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
    [Display(Name = "Loại sản phẩm")]
    public int CategoryId { get; set; }

    [Display(Name = "Hoạt động")]
    public bool IsActive { get; set; } = true;
}
