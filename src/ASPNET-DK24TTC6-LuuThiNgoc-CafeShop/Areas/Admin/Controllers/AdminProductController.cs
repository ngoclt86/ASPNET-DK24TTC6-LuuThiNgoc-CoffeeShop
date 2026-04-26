using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.ViewModels;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminProductController : Controller
{
    private const long MaxImageSizeBytes = 5 * 1024 * 1024;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IWebHostEnvironment _env;

    public AdminProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment env)
    {
        _productService = productService;
        _categoryService = categoryService;
        _env = env;
    }

    public async Task<IActionResult> Index(string? search, string? sort, int page = 1)
    {
        const int pageSize = 10;
        var requestedSort = string.IsNullOrWhiteSpace(sort) ? "newest" : sort.Trim().ToLowerInvariant();
        var currentSort = requestedSort is "newest" or "oldest" or "name_asc" or "name_desc" or "price_asc" or "price_desc"
            ? requestedSort
            : "newest";
        var currentSearch = search?.Trim() ?? string.Empty;

        var result = await _productService.GetAdminPagedAsync(currentSearch, currentSort, page, pageSize);
        var model = new AdminProductIndexViewModel
        {
            Items = result.Items,
            Search = currentSearch,
            Sort = currentSort,
            Page = page < 1 ? 1 : page,
            PageSize = pageSize,
            TotalItems = result.TotalItems
        };

        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            Stock = model.Stock,
            CategoryId = model.CategoryId,
            IsActive = model.IsActive
        };

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            product.ImageUrl = await SaveImageAsync(model.ImageFile);
        }
        else if (!string.IsNullOrWhiteSpace(model.ImageUrlInput))
        {
            var imagePath = await SaveImageFromUrlAsync(model.ImageUrlInput);
            if (imagePath == null)
            {
                ModelState.AddModelError(nameof(model.ImageUrlInput), "Không thể tải ảnh từ URL. Vui lòng kiểm tra link ảnh hợp lệ.");
                ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", model.CategoryId);
                return View(model);
            }

            product.ImageUrl = imagePath;
        }

        await _productService.CreateAsync(product);
        TempData["Success"] = "Thêm sản phẩm thành công!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            IsActive = product.IsActive,
            ExistingImageUrl = product.ImageUrl
        };

        ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", product.CategoryId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", model.CategoryId);
            return View(model);
        }

        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.Stock = model.Stock;
        product.CategoryId = model.CategoryId;
        product.IsActive = model.IsActive;

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            product.ImageUrl = await SaveImageAsync(model.ImageFile);
        }
        else if (!string.IsNullOrWhiteSpace(model.ImageUrlInput))
        {
            var imagePath = await SaveImageFromUrlAsync(model.ImageUrlInput);
            if (imagePath == null)
            {
                ModelState.AddModelError(nameof(model.ImageUrlInput), "Không thể tải ảnh từ URL. Vui lòng kiểm tra link ảnh hợp lệ.");
                ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", model.CategoryId);
                model.ExistingImageUrl = product.ImageUrl;
                return View(model);
            }

            product.ImageUrl = imagePath;
        }

        await _productService.UpdateAsync(product);
        TempData["Success"] = "Cập nhật sản phẩm thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        TempData["Success"] = "Xóa sản phẩm thành công!";
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> SaveImageAsync(IFormFile imageFile)
    {
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "products");
        if (!Directory.Exists(uploadsDir))
            Directory.CreateDirectory(uploadsDir);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(uploadsDir, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        return $"/uploads/products/{fileName}";
    }

    private async Task<string?> SaveImageFromUrlAsync(string imageUrl)
    {
        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
            return null;

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            return null;

        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
            return null;

        var mediaType = response.Content.Headers.ContentType?.MediaType;
        if (string.IsNullOrWhiteSpace(mediaType) || !mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return null;

        var contentLength = response.Content.Headers.ContentLength;
        if (contentLength.HasValue && contentLength.Value > MaxImageSizeBytes)
            return null;

        var bytes = await response.Content.ReadAsByteArrayAsync();
        if (bytes.Length == 0 || bytes.Length > MaxImageSizeBytes)
            return null;

        var extension = GetImageExtension(response.Content.Headers.ContentType);
        if (string.IsNullOrWhiteSpace(extension))
            return null;

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "products");
        if (!Directory.Exists(uploadsDir))
            Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsDir, fileName);
        await System.IO.File.WriteAllBytesAsync(filePath, bytes);

        return $"/uploads/products/{fileName}";
    }

    private static string? GetImageExtension(MediaTypeHeaderValue? contentType)
    {
        return contentType?.MediaType?.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            "image/bmp" => ".bmp",
            _ => null
        };
    }
}
