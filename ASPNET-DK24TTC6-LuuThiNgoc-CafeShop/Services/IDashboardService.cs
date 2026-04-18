using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardDataAsync(int? year = null);
}
