namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models.Configurations;

public class EmailOptions
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "CoffeeShop";
    public bool EnableSsl { get; set; } = true;
}
