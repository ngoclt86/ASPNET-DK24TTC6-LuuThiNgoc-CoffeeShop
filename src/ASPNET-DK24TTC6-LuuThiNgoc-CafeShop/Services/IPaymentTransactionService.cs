using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public interface IPaymentTransactionService
{
    Task CreateAsync(PaymentTransaction transaction);
}
