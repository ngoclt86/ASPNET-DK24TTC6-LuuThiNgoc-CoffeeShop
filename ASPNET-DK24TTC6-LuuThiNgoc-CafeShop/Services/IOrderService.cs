using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models.Enums;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync();
    Task<List<Order>> GetByUserIdAsync(string userId);
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateAsync(Order order);
    Task UpdateStatusAsync(int id, OrderStatus status);
    Task<StockUpdateResult> DeductStockForOrderAsync(int orderId);
}
