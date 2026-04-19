using Microsoft.EntityFrameworkCore;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Data;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models.Enums;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .IgnoreQueryFilters()
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<List<Order>> GetByUserIdAsync(string userId)
    {
        return await _context.Orders
            .IgnoreQueryFilters()
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .IgnoreQueryFilters()
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        order.OrderDate = DateTime.Now;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task UpdateStatusAsync(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            order.Status = status;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<StockUpdateResult> DeductStockForOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return new StockUpdateResult
            {
                IsSuccess = false,
                Message = "Không tìm thấy đơn hàng để cập nhật tồn kho."
            };
        }

        if (!order.OrderDetails.Any())
        {
            return new StockUpdateResult
            {
                IsSuccess = false,
                Message = "Đơn hàng không có sản phẩm để cập nhật tồn kho."
            };
        }

        var productIds = order.OrderDetails
            .Select(od => od.ProductId)
            .Distinct()
            .ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();
        var productMap = products.ToDictionary(p => p.Id);

        foreach (var detail in order.OrderDetails)
        {
            if (!productMap.TryGetValue(detail.ProductId, out var product))
            {
                return new StockUpdateResult
                {
                    IsSuccess = false,
                    Message = $"Sản phẩm có mã {detail.ProductId} không tồn tại."
                };
            }

            if (product.Stock < detail.Quantity)
            {
                return new StockUpdateResult
                {
                    IsSuccess = false,
                    Message = $"Sản phẩm \"{product.Name}\" chỉ còn {product.Stock}, không đủ cho số lượng đặt {detail.Quantity}."
                };
            }
        }

        foreach (var detail in order.OrderDetails)
        {
            var product = productMap[detail.ProductId];
            product.Stock -= detail.Quantity;
        }

        await _context.SaveChangesAsync();
        return new StockUpdateResult
        {
            IsSuccess = true,
            Message = "Cập nhật tồn kho thành công."
        };
    }
}
