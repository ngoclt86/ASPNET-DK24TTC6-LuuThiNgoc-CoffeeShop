using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminDashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public AdminDashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index(int? year, DateTime? fromDate, DateTime? toDate)
    {
        var viewModel = await _dashboardService.GetDashboardDataAsync(year, fromDate, toDate);
        ViewData["SelectedYear"] = year ?? DateTime.Now.Year;
        ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
        ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> ExportRevenueCsv(DateTime? fromDate, DateTime? toDate)
    {
        var items = await _dashboardService.GetRevenueExportAsync(fromDate, toDate);
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("OrderId,OrderDate,CustomerName,CustomerEmail,PaymentMethod,TotalAmount,Status");
        foreach (var item in items)
        {
            csvBuilder.AppendLine(
                $"{item.OrderId},{item.OrderDate:yyyy-MM-dd HH:mm:ss},\"{item.CustomerName}\",\"{item.CustomerEmail}\",{item.PaymentMethod},{item.TotalAmount:0.##},{item.Status}");
        }

        var fileName = $"revenue-report-{DateTime.Now:yyyyMMddHHmmss}.csv";
        return File(Encoding.UTF8.GetBytes(csvBuilder.ToString()), "text/csv", fileName);
    }
}
