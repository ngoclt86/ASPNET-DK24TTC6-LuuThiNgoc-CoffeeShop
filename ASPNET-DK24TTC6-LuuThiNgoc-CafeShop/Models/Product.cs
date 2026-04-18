using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

public class Product
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
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giá (VNĐ)")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
    public decimal Price { get; set; }

    [StringLength(300)]
    [Display(Name = "Hình ảnh")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Tồn kho")]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; } = 0;

    [Display(Name = "Loại sản phẩm")]
    public int CategoryId { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Display(Name = "Hoạt động")]
    public bool IsActive { get; set; } = true;

    // Navigation
    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }
}
