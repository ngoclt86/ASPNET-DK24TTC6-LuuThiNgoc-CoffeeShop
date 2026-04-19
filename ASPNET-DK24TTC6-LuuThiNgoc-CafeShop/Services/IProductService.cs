using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetActiveAsync();
    Task<(List<Product> Items, int TotalItems)> GetActivePagedAsync(int? categoryId, string? search, string? sort, int page, int pageSize);
    Task<List<Product>> GetByCategoryAsync(int categoryId);
    Task<List<Product>> SearchAsync(string keyword);
    Task<List<Product>> GetFeaturedAsync(int count = 8);
    Task<Product?> GetByIdAsync(int id);
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
