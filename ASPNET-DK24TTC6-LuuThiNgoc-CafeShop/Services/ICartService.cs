using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public interface ICartService
{
    List<CartItem> GetCart(ISession session);
    void AddToCart(ISession session, CartItem item);
    void RemoveFromCart(ISession session, int productId);
    void UpdateQuantity(ISession session, int productId, int quantity);
    void ClearCart(ISession session);
    int GetCartCount(ISession session);
    decimal GetCartTotal(ISession session);
}
