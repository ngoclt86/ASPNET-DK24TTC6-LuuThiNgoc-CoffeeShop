using System.ComponentModel.DataAnnotations;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên loại sản phẩm")]
    [StringLength(100)]
    [Display(Name = "Tên loại")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    // Navigation
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
