namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

public class DashboardViewModel
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
    public int TotalUsers { get; set; }
    public List<MonthlyRevenueItem> MonthlyRevenue { get; set; } = new();
    public List<CategoryRevenueItem> RevenueByCategory { get; set; } = new();
}

public class MonthlyRevenueItem
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}

public class CategoryRevenueItem
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int ProductCount { get; set; }
}
