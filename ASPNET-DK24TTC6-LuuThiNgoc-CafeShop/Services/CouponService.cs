using Microsoft.EntityFrameworkCore;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Data;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public class CouponService : ICouponService
{
    private readonly ApplicationDbContext _context;

    public CouponService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Coupon>> GetAllAsync()
    {
        return await _context.Coupons
            .OrderByDescending(c => c.ExpiryDate)
            .ToListAsync();
    }

    public async Task<Coupon?> GetByIdAsync(int id)
    {
        return await _context.Coupons.FindAsync(id);
    }

    public async Task<Coupon?> GetByCodeAsync(string code)
    {
        return await _context.Coupons
            .FirstOrDefaultAsync(c => c.Code.ToUpper() == code.ToUpper());
    }

    public async Task CreateAsync(Coupon coupon)
    {
        coupon.Code = coupon.Code.ToUpper();
        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Coupon coupon)
    {
        coupon.Code = coupon.Code.ToUpper();
        _context.Coupons.Update(coupon);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var coupon = await _context.Coupons.FindAsync(id);
        if (coupon != null)
        {
            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(bool IsValid, string Message, int DiscountPercent)> ValidateAsync(string code)
    {
        var coupon = await GetByCodeAsync(code);
        if (coupon == null)
            return (false, "Mã giảm giá không tồn tại", 0);
        if (!coupon.IsActive)
            return (false, "Mã giảm giá đã bị vô hiệu hóa", 0);
        if (coupon.ExpiryDate < DateTime.Now)
            return (false, "Mã giảm giá đã hết hạn", 0);
        if (coupon.CurrentUsage >= coupon.MaxUsage)
            return (false, "Mã giảm giá đã hết lượt sử dụng", 0);

        return (true, $"Giảm {coupon.DiscountPercent}%", coupon.DiscountPercent);
    }

    public async Task IncrementUsageAsync(string code)
    {
        var coupon = await GetByCodeAsync(code);
        if (coupon != null)
        {
            coupon.CurrentUsage++;
            await _context.SaveChangesAsync();
        }
    }
}
