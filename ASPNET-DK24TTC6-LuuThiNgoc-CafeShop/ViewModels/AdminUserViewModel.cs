using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

public class AdminUserViewModel
{
    public List<AdminUserItemViewModel> Users { get; set; } = new();
    public string? Search { get; set; }
    public List<string> AvailableRoles { get; set; } = new();
}

public class AdminUserItemViewModel
{
    public ApplicationUser User { get; set; } = default!;
    public string CurrentRole { get; set; } = "User";
}
