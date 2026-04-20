using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models.Enums;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

public class PaymentTransaction
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    [Required]
    [StringLength(30)]
    public string Provider { get; set; } = "VNPAY";

    [StringLength(50)]
    public string? TxnRef { get; set; }

    [StringLength(100)]
    public string? TransactionNo { get; set; }

    [StringLength(10)]
    public string ResponseCode { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Message { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Pending;

    [StringLength(2000)]
    public string? RawQuery { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey(nameof(OrderId))]
    public virtual Order? Order { get; set; }
}
