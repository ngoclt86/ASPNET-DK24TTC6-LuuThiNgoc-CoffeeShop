using System.Text.Json;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public class CartService : ICartService
{
    private const string CartKey = "ShoppingCart";

    public List<CartItem> GetCart(ISession session)
    {
        var json = session.GetString(CartKey);
        return json == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }

    public void AddToCart(ISession session, CartItem item)
    {
        var cart = GetCart(session);
        var existing = cart.FirstOrDefault(c => c.ProductId == item.ProductId);
        if (existing != null)
        {
            existing.Quantity += item.Quantity;
        }
        else
        {
            cart.Add(item);
        }
        SaveCart(session, cart);
    }

    public void RemoveFromCart(ISession session, int productId)
    {
        var cart = GetCart(session);
        cart.RemoveAll(c => c.ProductId == productId);
        SaveCart(session, cart);
    }

    public void UpdateQuantity(ISession session, int productId, int quantity)
    {
        var cart = GetCart(session);
        var item = cart.FirstOrDefault(c => c.ProductId == productId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                cart.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
        }
        SaveCart(session, cart);
    }

    public void ClearCart(ISession session)
    {
        session.Remove(CartKey);
    }

    public int GetCartCount(ISession session)
    {
        return GetCart(session).Sum(c => c.Quantity);
    }

    public decimal GetCartTotal(ISession session)
    {
        return GetCart(session).Sum(c => c.Total);
    }

    private void SaveCart(ISession session, List<CartItem> cart)
    {
        session.SetString(CartKey, JsonSerializer.Serialize(cart));
    }
}
