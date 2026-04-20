namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
}
