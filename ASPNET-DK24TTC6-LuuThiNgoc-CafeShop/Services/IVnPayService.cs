using Microsoft.AspNetCore.Http;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public interface IVnPayService
{
    string CreatePaymentUrl(HttpContext httpContext, int orderId, decimal amount, string orderInfo);
    VnPayReturnResult ProcessReturnResponse(IQueryCollection queryCollection);
}
