# CafeShop - Ứng Dụng Quản Lý Quán Cà Phê

## Giới thiệu
CafeShop là một ứng dụng web thương mại điện tử chuyên cung cấp trải nghiệm mua sắm và quản lý bán hàng cho hệ thống quán cà phê. Dự án được xây dựng với **ASP.NET Core 8 MVC**, áp dụng mô hình kiến trúc 3 lớp (3-tier architecture). Hệ thống được chia thành các phân hệ Giao diện Người dùng (User) và Giao diện Quản trị viên (Admin) nhằm đáp ứng nhu cầu cho cả khách hàng xem/mua sản phẩm, quản lý giỏ hàng, đặt hàng, lẫn luồng quản trị viên quản lý danh mục, sản phẩm, và đơn hàng.

## Công nghệ sử dụng
Hệ thống sử dụng các công nghệ hiện đại thuộc hệ sinh thái .NET và các công cụ liên quan:
- **Ngôn ngữ lập trình:** C#
- **Web Framework:** ASP.NET Core 8 MVC (Model-View-Controller)
- **Database:** Microsoft SQL Server
- **ORM (Object-Relational Mapping):** Entity Framework Core 8
- **Authentication/Authorization:** ASP.NET Core Identity (Quản lý User & Role định tuyến cho Admin/User)
- **Giao diện (Frontend):** HTML5, CSS3, JavaScript tương thích đa thiết bị theo hướng đáp ứng (Responsive Layout) kèm các View components của Razor Pages.
- **Triển khai (Containerization):** Docker & Docker Compose (cho SQL Server)

## Hướng dẫn cài đặt và chạy chạy dự án

### 1. Yêu cầu hệ thống (Prerequisites)
- [**.NET 8.0 SDK**](https://dotnet.microsoft.com/download/dotnet/8.0) trở lên
- [**Docker Desktop**](https://www.docker.com/products/docker-desktop) (nếu bạn muốn chạy Database qua Container có sẵn) hoặc bản cài đặt Microsoft SQL Server cục bộ.
- Một IDE hoặc Code Editor (khuyến nghị **Visual Studio 2022**, **JetBrains Rider**, hoặc **VS Code**).

### 2. Thiết lập Database (Docker)
Dự án đã sử dụng file `compose.yaml` ở thư mục gốc để giả lập Database. Mở terminal, đi tới thư mục chứa file `compose.yaml` và chạy:
```bash
docker compose up -d
```
Container `sqlserver-cafeshop` sẽ được kích hoạt tại cổng `1433`.

### 3. Cài đặt các gói phụ thuộc (Dependencies)
Truy cập vào trong thư mục giải pháp chứa file `.csproj` và chạy lệnh phục hồi gói:
```bash
cd ASPNET-DK24TTC6-LuuThiNgoc-CafeShop
dotnet restore
```

### 4. Cấu hình Chuỗi kết nối (Connection String)
Trong thư mục chứa source (`ASPNET-DK24TTC6-LuuThiNgoc-CafeShop`), bạn cần kiểm tra file `appsettings.json` và `appsettings.Development.json`. 

- *Lưu ý: Mặc định server được đặt là `sqlserver-cafeshop` (container name). Khi chạy project trực tiếp bằng IDE trên máy của bạn (mà không đưa app vào docker) kết nối qua Docker port forward, hãy chắc chắn file cấu hình được sửa để trỏ về máy chủ ảo (ví dụ: đổi `Server=sqlserver-cafeshop,1433;` thành `Server=localhost,1433;`).*

### 5. Cập nhật Database (Migrations)
Sử dụng Entity Framework Core tool để tạo cấu trúc bảng thông qua migrations:
```bash
dotnet ef database update
```
*(Nếu gặp lỗi thiếu lệnh `ef`, tiến hành cài đặt toàn cục: `dotnet tool install --global dotnet-ef`)*

### 6. Khởi chạy Ứng dụng
Cuối cùng, gõ lệnh dưới đây để chạy app (có hỗ trợ hot-reload):
```bash
dotnet watch run
```
Hoặc trực tiếp nhấn nút **Play / Debug (F5)** trên Visual Studio / Rider. 

Website sẽ bắt đầu tiếp nhận request ở địa chỉ được ánh xạ trên terminal (thường là `https://localhost:7198` hoặc `http://localhost:5222`).

---
✨ *Chúc các bạn trải nghiệm các tính năng của dự án CafeShop thành công!*
