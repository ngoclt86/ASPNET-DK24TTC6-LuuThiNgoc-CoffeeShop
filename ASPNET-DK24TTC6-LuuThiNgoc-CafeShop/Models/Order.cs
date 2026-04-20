using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models.Enums;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Display(Name = "Ngày đặt")]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tổng tiền")]
    public decimal TotalAmount { get; set; }

    [Display(Name = "Trạng thái")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
    [StringLength(300)]
    [Display(Name = "Địa chỉ giao hàng")]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? Notes { get; set; }

    [StringLength(50)]
    [Display(Name = "Mã giảm giá")]
    public string? CouponCode { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giảm giá")]
    public decimal DiscountAmount { get; set; } = 0;

    [Display(Name = "Phương thức thanh toán")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cod;

    [Display(Name = "Đã trừ tồn kho")]
    public bool IsStockDeducted { get; set; }

    // Navigation
    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
