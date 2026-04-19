using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

public class OrderHistoryViewModel
{
    public List<Order> Orders { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalOrders { get; set; }
    public int PageSize { get; set; } = 6;
}
