namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public class VnPayReturnResult
{
    public bool IsValidSignature { get; set; }
    public bool IsSuccess { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string TransactionNo { get; set; } = string.Empty;
    public string ResponseCode { get; set; } = string.Empty;
    public string ProvidedHash { get; set; } = string.Empty;
    public string ExpectedHash { get; set; } = string.Empty;
    public string HashData { get; set; } = string.Empty;
}
