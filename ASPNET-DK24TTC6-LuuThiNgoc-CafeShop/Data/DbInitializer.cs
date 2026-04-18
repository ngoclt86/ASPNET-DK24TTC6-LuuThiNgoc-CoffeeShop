using Microsoft.AspNetCore.Identity;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Seed Roles
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed Admin User
        var adminEmail = "admin@cafeshop.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Administrator",
                Address = "Hà Nội, Việt Nam",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed Categories
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new() { Name = "Cà phê", Description = "Các loại cà phê truyền thống và hiện đại" },
                new() { Name = "Trà", Description = "Trà các loại, trà sữa, trà trái cây" },
                new() { Name = "Nước ép", Description = "Nước ép trái cây tươi" },
                new() { Name = "Sinh tố", Description = "Sinh tố các loại trái cây" },
                new() { Name = "Đá xay", Description = "Đồ uống đá xay mát lạnh" }
            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Seed Products
        if (!context.Products.Any())
        {
            var caPheCat = context.Categories.First(c => c.Name == "Cà phê");
            var traCat = context.Categories.First(c => c.Name == "Trà");
            var nuocEpCat = context.Categories.First(c => c.Name == "Nước ép");
            var sinhToCat = context.Categories.First(c => c.Name == "Sinh tố");
            var daXayCat = context.Categories.First(c => c.Name == "Đá xay");

            var products = new List<Product>
            {
                // Cà phê
                new() { Name = "Cà phê đen đá", Description = "Cà phê đen truyền thống pha phin, đậm đà hương vị Việt Nam", Price = 25000, Stock = 100, CategoryId = caPheCat.Id, ImageUrl = "/uploads/products/ca-phe-den.jpg" },
                new() { Name = "Cà phê sữa đá", Description = "Cà phê sữa đá thơm ngon, béo ngậy", Price = 29000, Stock = 100, CategoryId = caPheCat.Id, ImageUrl = "/uploads/products/ca-phe-sua.jpg" },
                new() { Name = "Bạc xỉu", Description = "Cà phê sữa nhẹ nhàng, phù hợp cho người mới bắt đầu", Price = 29000, Stock = 100, CategoryId = caPheCat.Id, ImageUrl = "/uploads/products/bac-xiu.jpg" },
                new() { Name = "Cappuccino", Description = "Cappuccino thơm ngon theo phong cách Ý", Price = 45000, Stock = 100, CategoryId = caPheCat.Id, ImageUrl = "/uploads/products/cappuccino.jpg" },
                new() { Name = "Latte", Description = "Latte mềm mại với lớp foam mịn", Price = 45000, Stock = 100, CategoryId = caPheCat.Id, ImageUrl = "/uploads/products/latte.jpg" },
                new() { Name = "Americano", Description = "Cà phê Americano đậm đà, thanh nhẹ", Price = 39000, Stock = 100, CategoryId = caPheCat.Id, ImageUrl = "/uploads/products/americano.jpg" },

                // Trà
                new() { Name = "Trà đào cam sả", Description = "Trà đào kết hợp cam và sả thơm mát", Price = 35000, Stock = 100, CategoryId = traCat.Id, ImageUrl = "/uploads/products/tra-dao.jpg" },
                new() { Name = "Trà sữa trân châu", Description = "Trà sữa truyền thống với trân châu dẻo mềm", Price = 35000, Stock = 100, CategoryId = traCat.Id, ImageUrl = "/uploads/products/tra-sua.jpg" },
                new() { Name = "Trà vải", Description = "Trà vải thơm ngát, ngọt tự nhiên", Price = 32000, Stock = 100, CategoryId = traCat.Id, ImageUrl = "/uploads/products/tra-vai.jpg" },
                new() { Name = "Trà matcha latte", Description = "Trà xanh matcha kết hợp sữa tươi", Price = 45000, Stock = 100, CategoryId = traCat.Id, ImageUrl = "/uploads/products/matcha-latte.jpg" },

                // Nước ép
                new() { Name = "Nước ép cam", Description = "Nước ép cam tươi nguyên chất, giàu vitamin C", Price = 30000, Stock = 80, CategoryId = nuocEpCat.Id, ImageUrl = "/uploads/products/nuoc-ep-cam.jpg" },
                new() { Name = "Nước ép dưa hấu", Description = "Nước ép dưa hấu mát lạnh giải nhiệt", Price = 28000, Stock = 80, CategoryId = nuocEpCat.Id, ImageUrl = "/uploads/products/nuoc-ep-dua-hau.jpg" },

                // Sinh tố
                new() { Name = "Sinh tố bơ", Description = "Sinh tố bơ béo ngậy, bổ dưỡng", Price = 35000, Stock = 80, CategoryId = sinhToCat.Id, ImageUrl = "/uploads/products/sinh-to-bo.jpg" },
                new() { Name = "Sinh tố xoài", Description = "Sinh tố xoài ngọt tự nhiên", Price = 32000, Stock = 80, CategoryId = sinhToCat.Id, ImageUrl = "/uploads/products/sinh-to-xoai.jpg" },
                new() { Name = "Sinh tố dâu", Description = "Sinh tố dâu tây tươi mát", Price = 35000, Stock = 80, CategoryId = sinhToCat.Id, ImageUrl = "/uploads/products/sinh-to-dau.jpg" },

                // Đá xay
                new() { Name = "Chocolate đá xay", Description = "Chocolate đá xay mát lạnh, ngọt ngào", Price = 45000, Stock = 80, CategoryId = daXayCat.Id, ImageUrl = "/uploads/products/choco-da-xay.jpg" },
                new() { Name = "Matcha đá xay", Description = "Matcha đá xay Nhật Bản chính hiệu", Price = 49000, Stock = 80, CategoryId = daXayCat.Id, ImageUrl = "/uploads/products/matcha-da-xay.jpg" },
            };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        // Seed Coupons
        if (!context.Coupons.Any())
        {
            var coupons = new List<Coupon>
            {
                new() { Code = "WELCOME10", DiscountPercent = 10, ExpiryDate = DateTime.Now.AddMonths(3), MaxUsage = 100, IsActive = true },
                new() { Code = "CAFE20", DiscountPercent = 20, ExpiryDate = DateTime.Now.AddMonths(1), MaxUsage = 50, IsActive = true }
            };
            context.Coupons.AddRange(coupons);
            await context.SaveChangesAsync();
        }
    }
}
