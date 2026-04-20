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

    public async Task<StockUpdateResult> UpdateStatusWithInventoryAsync(int id, OrderStatus status)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return new StockUpdateResult
            {
                IsSuccess = false,
                Message = "Không tìm thấy đơn hàng."
            };
        }

        if (order.Status == status)
        {
            return new StockUpdateResult
            {
                IsSuccess = true,
                Message = "Trạng thái đơn hàng đã được cập nhật trước đó."
            };
        }

        if ((status == OrderStatus.Processing || status == OrderStatus.Completed) && !order.IsStockDeducted)
        {
            var deductResult = await DeductStockForOrderAsync(order.Id);
            if (!deductResult.IsSuccess)
            {
                return deductResult;
            }
        }

        if (status == OrderStatus.Cancelled && order.IsStockDeducted)
        {
            var restoreResult = await RestoreStockForOrderAsync(order.Id);
            if (!restoreResult.IsSuccess)
            {
                return restoreResult;
            }
        }

        order.Status = status;
        await _context.SaveChangesAsync();
        return new StockUpdateResult
        {
            IsSuccess = true,
            Message = "Cập nhật trạng thái đơn hàng thành công."
        };
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

        if (order.IsStockDeducted)
        {
            return new StockUpdateResult
            {
                IsSuccess = true,
                Message = "Đơn hàng đã được trừ tồn kho trước đó."
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

        order.IsStockDeducted = true;
        await _context.SaveChangesAsync();
        return new StockUpdateResult
        {
            IsSuccess = true,
            Message = "Cập nhật tồn kho thành công."
        };
    }

    public async Task<StockUpdateResult> RestoreStockForOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return new StockUpdateResult
            {
                IsSuccess = false,
                Message = "Không tìm thấy đơn hàng để hoàn kho."
            };
        }

        if (!order.IsStockDeducted)
        {
            return new StockUpdateResult
            {
                IsSuccess = true,
                Message = "Đơn hàng chưa trừ kho, không cần hoàn kho."
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
                    Message = $"Không tìm thấy sản phẩm {detail.ProductId} để hoàn kho."
                };
            }

            product.Stock += detail.Quantity;
        }

        order.IsStockDeducted = false;
        await _context.SaveChangesAsync();
        return new StockUpdateResult
        {
            IsSuccess = true,
            Message = "Hoàn kho thành công."
        };
    }
}
