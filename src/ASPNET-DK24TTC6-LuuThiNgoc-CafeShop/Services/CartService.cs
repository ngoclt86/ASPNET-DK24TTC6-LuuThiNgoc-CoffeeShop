using System.Text.Json;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public class CartService : ICartService
{
    private const string CartKey = "ShoppingCart";
    private const string CouponCodeKey = "AppliedCouponCode";
    private const string CouponDiscountPercentKey = "AppliedCouponDiscountPercent";

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
        RemoveCoupon(session);
    }

    public int GetCartCount(ISession session)
    {
        return GetCart(session).Sum(c => c.Quantity);
    }

    public decimal GetCartTotal(ISession session)
    {
        return GetCart(session).Sum(c => c.Total);
    }

    public void ApplyCoupon(ISession session, string code, int discountPercent)
    {
        session.SetString(CouponCodeKey, code.Trim().ToUpperInvariant());
        session.SetInt32(CouponDiscountPercentKey, discountPercent);
    }

    public void RemoveCoupon(ISession session)
    {
        session.Remove(CouponCodeKey);
        session.Remove(CouponDiscountPercentKey);
    }

    public (string? Code, int DiscountPercent) GetAppliedCoupon(ISession session)
    {
        var code = session.GetString(CouponCodeKey);
        var discountPercent = session.GetInt32(CouponDiscountPercentKey) ?? 0;
        return (code, discountPercent);
    }

    public decimal GetCartDiscountAmount(ISession session)
    {
        var (code, discountPercent) = GetAppliedCoupon(session);
        if (string.IsNullOrWhiteSpace(code) || discountPercent <= 0)
        {
            return 0;
        }

        return GetCartTotal(session) * discountPercent / 100m;
    }

    private void SaveCart(ISession session, List<CartItem> cart)
    {
        session.SetString(CartKey, JsonSerializer.Serialize(cart));
        if (cart.Count == 0)
        {
            RemoveCoupon(session);
        }
    }
}
