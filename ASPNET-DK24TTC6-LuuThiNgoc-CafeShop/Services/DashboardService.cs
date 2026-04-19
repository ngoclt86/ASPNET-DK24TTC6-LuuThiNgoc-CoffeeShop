using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Data;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models.Enums;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync(int? year = null)
    {
        var targetYear = year ?? DateTime.Now.Year;
        var completedOrders = _context.Orders
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Completed);

        var viewModel = new DashboardViewModel
        {
            TotalRevenue = await completedOrders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
            TotalOrders = await _context.Orders.CountAsync(),
            TotalProducts = await _context.Products.CountAsync(),
            TotalUsers = await _userManager.Users.AsNoTracking().CountAsync()
        };

        // Monthly revenue for the target year
        var monthlyData = await completedOrders
            .Where(o => o.OrderDate.Year == targetYear)
            .GroupBy(o => o.OrderDate.Month)
            .Select(g => new MonthlyRevenueItem
            {
                Month = g.Key.ToString(),
                Revenue = g.Sum(o => o.TotalAmount),
                OrderCount = g.Count()
            })
            .OrderBy(m => m.Month)
            .ToListAsync();

        // Fill all 12 months
        for (int i = 1; i <= 12; i++)
        {
            if (!monthlyData.Any(m => m.Month == i.ToString()))
            {
                monthlyData.Add(new MonthlyRevenueItem
                {
                    Month = i.ToString(),
                    Revenue = 0,
                    OrderCount = 0
                });
            }
        }
        viewModel.MonthlyRevenue = monthlyData.OrderBy(m => int.Parse(m.Month)).ToList();

        // Revenue by category
        viewModel.RevenueByCategory = await _context.OrderDetails
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(od => od.Order)
            .Include(od => od.Product)
                .ThenInclude(p => p!.Category)
            .Where(od => od.Order!.Status == OrderStatus.Completed)
            .GroupBy(od => od.Product!.Category!.Name)
            .Select(g => new CategoryRevenueItem
            {
                CategoryName = g.Key,
                Revenue = g.Sum(od => od.UnitPrice * od.Quantity),
                ProductCount = g.Select(od => od.ProductId).Distinct().Count()
            })
            .OrderByDescending(c => c.Revenue)
            .ToListAsync();

        return viewModel;
    }
}
