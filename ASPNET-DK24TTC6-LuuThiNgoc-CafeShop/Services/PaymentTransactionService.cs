using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Data;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public class PaymentTransactionService : IPaymentTransactionService
{
    private readonly ApplicationDbContext _context;

    public PaymentTransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(PaymentTransaction transaction)
    {
        _context.PaymentTransactions.Add(transaction);
        await _context.SaveChangesAsync();
    }
}
