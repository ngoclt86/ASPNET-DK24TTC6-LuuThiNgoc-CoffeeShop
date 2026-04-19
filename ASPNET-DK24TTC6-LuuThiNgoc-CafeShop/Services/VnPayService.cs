using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models.Configurations;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public class VnPayService : IVnPayService
{
    private readonly VnPayOptions _options;

    public VnPayService(IOptions<VnPayOptions> options)
    {
        _options = options.Value;
    }

    public string CreatePaymentUrl(HttpContext httpContext, int orderId, decimal amount, string orderInfo)
    {
        ValidateConfig();

        var now = DateTime.Now;
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var txnRef = orderId.ToString();
        var amountValue = ((long)Math.Round(amount * 100, 0)).ToString(CultureInfo.InvariantCulture);

        var requestData = new SortedDictionary<string, string>
        {
            ["vnp_Amount"] = amountValue,
            ["vnp_Command"] = _options.Command,
            ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss"),
            ["vnp_CurrCode"] = _options.CurrCode,
            ["vnp_IpAddr"] = ipAddress,
            ["vnp_Locale"] = _options.Locale,
            ["vnp_OrderInfo"] = orderInfo,
            ["vnp_OrderType"] = "other",
            ["vnp_ReturnUrl"] = _options.ReturnUrl,
            ["vnp_TmnCode"] = _options.TmnCode,
            ["vnp_TxnRef"] = txnRef,
            ["vnp_Version"] = _options.Version
        };

        var hashData = BuildRequestHashData(requestData);
        var secureHash = ComputeHmacSha512(_options.HashSecret, hashData);
        var queryData = BuildQueryData(requestData);
        return $"{_options.BaseUrl}?{queryData}&vnp_SecureHashType=HmacSHA512&vnp_SecureHash={secureHash}";
    }

    public VnPayReturnResult ProcessReturnResponse(IQueryCollection queryCollection)
    {
        var data = queryCollection
            .Where(x => x.Key.StartsWith("vnp_", StringComparison.Ordinal))
            .ToDictionary(x => x.Key, x => x.Value.ToString());

        data.TryGetValue("vnp_SecureHash", out var secureHash);
        data.Remove("vnp_SecureHash");
        data.Remove("vnp_SecureHashType");

        var hashData = BuildReturnHashData(new SortedDictionary<string, string>(data));
        var expectedHash = ComputeHmacSha512(_options.HashSecret, hashData);
        var isSuccess =
            data.TryGetValue("vnp_ResponseCode", out var responseCode) && responseCode == "00" &&
            data.TryGetValue("vnp_TransactionStatus", out var transactionStatus) && transactionStatus == "00";

        return new VnPayReturnResult
        {
            IsValidSignature = string.Equals(expectedHash, secureHash, StringComparison.OrdinalIgnoreCase),
            IsSuccess = isSuccess,
            ResponseCode = data.TryGetValue("vnp_ResponseCode", out var code) ? code : string.Empty,
            OrderId = data.TryGetValue("vnp_TxnRef", out var txnRef) ? txnRef : string.Empty,
            TransactionNo = data.TryGetValue("vnp_TransactionNo", out var txnNo) ? txnNo : string.Empty,
            ProvidedHash = secureHash ?? string.Empty,
            ExpectedHash = expectedHash,
            HashData = hashData
        };
    }

    private void ValidateConfig()
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl) ||
            string.IsNullOrWhiteSpace(_options.TmnCode) ||
            string.IsNullOrWhiteSpace(_options.HashSecret) ||
            string.IsNullOrWhiteSpace(_options.ReturnUrl))
        {
            throw new InvalidOperationException("Cấu hình VNPAY chưa đầy đủ.");
        }
    }

    private static string BuildRequestHashData(SortedDictionary<string, string> data)
    {
        return string.Join("&",
            data.Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => $"{x.Key}={EncodeData(x.Value)}"));
    }

    private static string BuildReturnHashData(SortedDictionary<string, string> data)
    {
        return string.Join("&",
            data.Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => $"{x.Key}={x.Value}"));
    }

    private static string BuildQueryData(SortedDictionary<string, string> data)
    {
        return string.Join("&",
            data.Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => $"{x.Key}={EncodeData(x.Value)}"));
    }

    private static string EncodeData(string value)
    {
        return WebUtility.UrlEncode(value).Replace("%20", "+");
    }

    private static string ComputeHmacSha512(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
